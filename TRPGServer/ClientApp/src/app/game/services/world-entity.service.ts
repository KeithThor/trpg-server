import { Injectable } from "@angular/core";
import { HubConnectionBuilder, HubConnection } from '@aspnet/signalr';
import { WorldEntity } from "../model/world-entity.model";
import { Coordinate } from "../model/coordinate.model";
import { LocalStorageConstants } from "../../constants";
import { EntityLocation } from "../model/entity-location.model";

@Injectable()
export class WorldEntityService {
  constructor() {
    this.entityLocations = [];
    this.entities = [];
    this.entityIds = [];
    this.isRequestingData = false;
    this.requestIds = [];
    this.queuedRequestIds = [];
    this.failedReconnections = 0;
    this.changeMapCallbacks = [];
  }

  private connection: HubConnection;
  public entityLocations: number[][];
  public entities: WorldEntity[];
  private entityIds: number[];
  private requestIds: number[];
  private queuedRequestIds: number[];
  private isRequestingData: boolean;
  private failedReconnections: number;
  private changeMapCallbacks: { (newMapId: number): Promise<void>; }[]
  private updateLocationsCallbacks: { (mapEntities: number[][], entities: EntityLocation[]): void; }[]

  /** Initializes this service and starts the connection to the server. */
  public async initializeAsync(): Promise<void> {
    this.entityLocations = [];
    this.changeMapCallbacks = [];
    this.updateLocationsCallbacks = [];
    this.connection = new HubConnectionBuilder().withUrl("/hubs/worldentities",
      {
        accessTokenFactory: () => localStorage.getItem(LocalStorageConstants.authToken)
      })
      .build();
    this.connection.on("updateEntities",
      (locations: number[][], entities: EntityLocation[]) => this.updateEntities(locations, entities));
    this.connection.on("addEntities", (entities: WorldEntity[]) => this.addEntities(entities));
    this.connection.on("removeEntities", (ids: number[]) => this.removeEntities(ids));
    this.connection.on("updateMovement", this.updateMovement);
    this.connection.on("receiveMissingEntities", (entities: WorldEntity[]) => this.receiveMissingEntities(entities));
    this.connection.on("changeMapsSuccess", (newMapId: number) => this.changeMapsSuccess(newMapId));
    this.connection.onclose(this.closeConnection);

    await this.connection.start()
      .catch(async () => {
        this.failedReconnections++;
        if (this.failedReconnections <= 5) {
          console.log("Reconnecting to server");
          await new Promise(() => 
            setTimeout(() => this.connection.start(), 250))
        }
        else {
          throw "Could not connect to the server.";
        }
      })
      .then(async () => {
        await this.connection.send("StartConnection");
      });
  }

  /**
   * Sends a message to the server to start the game for the user.
   */
  public beginPlayAsync(): Promise<void> {
    if (this.connection == undefined) throw new Error("The connection hasn't been started yet.");
    console.log("Beginning play");
    return this.connection.send("BeginPlay").catch(err => console.log(err));
  }

  /** Registers a callback function to be called whenever a request to change maps is successfully performed. */
  public onChangeMaps(callback: (newMapId: number) => Promise<void>): void {
    this.changeMapCallbacks.unshift(callback);
  }

  /**
   * Registers a callback function to be called whenever there is an update to entity locations from the server.
   * @param callback The function called whenever there is an update to entity locations from the server.
   */
  public onUpdateLocations(callback: (mapEntities: number[][], entities: EntityLocation[]) => void): void {
    this.updateLocationsCallbacks.push(callback);
  }

  /**
   * Moves the player entity an amount from it's current position.
   * @param deltaPosition The change in X and Y coordinates to move the player entity.
   */
  public moveEntity(deltaPosition: Coordinate) {
    if (this.connection == undefined) throw new Error("The connection hasn't been started yet.");

    this.connection.send("MoveEntity", deltaPosition);
  }

  /**
   * Changes the current map to the map with the provided id asynchronously.
   * @param mapId The id of the map to switch to.
   */
  public changeMapAsync(mapId?: number): Promise<void> {
    if (this.connection == undefined) throw new Error("The connection hasn't been started yet.");
    
    if (mapId == null) {
      return this.connection.send("ChangeMaps");
    }
    return this.connection.send("ChangeMaps", mapId);
  }

  /**
   * Ends the connection between the world entity client service and the server.
   */
  public async endConnectionAsync(): Promise<void> {
    if (this.connection != undefined || this.connection.state.valueOf() === 1) {
      this.entityLocations = [];
      this.entities = [];
      this.entityIds = [];
      this.failedReconnections = 0;
      this.isRequestingData = false;
      this.queuedRequestIds = [];
      this.requestIds = [];
      await this.connection.send("EndConnection");
      await this.connection.stop();
    }
  }
  
  /**
   * Sends a request to the server to return entity data that the client is missing.
   * @param entityIds The ids of the entities to return data for.
   */
  public requestEntityData(entityIds: number[]): void {
    if (!this.isRequestingData) {
      this.requestIds = entityIds;
      this.isRequestingData = true;
      this.connection.send("RequestEntityData", entityIds);
      console.log("Requesting entity data");
    }
    else {
      entityIds.forEach(id => {
        if (this.requestIds.indexOf(id) !== -1 && this.queuedRequestIds.indexOf(id) !== -1) {
          this.queuedRequestIds.push(id);
        }
      });
    }
  }

  /**
   * Closes the connection to the server.
   * @param error Represents the error object, if any.
   */
  private closeConnection(error): void {
    if (error != undefined) {
      console.log(error);
    }

    console.log("Connection closing");
    this.connection.send("EndConnection");
  }

  /**
   * Called whenever there is an update for world entity locations available from the server.
   * @param mapEntities A 2d array containing the locations of entities represented by their ids.
   */
  private updateEntities(mapEntities: number[][], entities: EntityLocation[]): void {
    //if (entities != null) {
    //  this.entityIds = entities;
    //  setTimeout(() => this.verifyEntities(), 500);
    //}
    this.entityLocations = mapEntities;
    this.updateLocationsCallbacks.forEach(callback => callback(mapEntities, entities));
  }

  /**
   * Checks to see if there are any missing entity data, if so calls requestEntityData with the ids of the missing entities.
   */
  private verifyEntities(): void {
    let missingIds: number[] = [];
    if (this.entities.length > this.entityIds.length) {
      this.entities.forEach(entity => {
        if (this.entityIds.indexOf(entity.id) === -1) {
          console.log("Found missing entity id" + entity.id);
          missingIds.push(entity.id);
        }
      });
    }
    else {
      this.entityIds.forEach(id => {
        if (!this.entities.some(entity => entity.id === id)) {
          missingIds.push(id);
        }
      });
    }

    if (missingIds.length > 0) {
      console.log("Missing entitys, calling requestEntityData");
      this.requestEntityData(missingIds);
    }
  }

  /**
   * Called whenever there are one or more entities spawned on the map.
   * @param entities The new entities added by the server.
   */
  private addEntities(entities: WorldEntity[]): void {
    entities.forEach(entity => {
      this.entities.push(entity);
    });
    console.log(this.entities);
  }

  /**
   * Called whenever one or more entities should be removed from the map.
   * @param entityIds The ids of the entities to be removed.
   */
  private removeEntities(entityIds: number[]): void {
    for (var i = this.entities.length - 1; i <= 0; i++) {
      if (entityIds.indexOf(this.entities[i].id) !== -1) {
        this.entities.splice(i, 1);
      }
    }
  }

  /**
   * Called whenever a request to change maps has been completed successfully by the server.
   * @param newMapId The id of the map the player was switched to.
   */
  private changeMapsSuccess(newMapId: number): void {
    this.entities = [];
    this.entityIds = [];
    this.entityLocations = [];
    this.requestIds = [];
    this.queuedRequestIds = [];
    this.isRequestingData = false;
    this.changeMapCallbacks.forEach(callback => callback(newMapId));
  }

  /**
   * Called after a successful request for movement by the client.
   * @param location The new location occupied by the client's entity.
   */
  private updateMovement(location: Coordinate): void {
  }

  /**
   * Called whenever missing data is successfully retrieved by the server.
   * @param entities An array containing the missing WorldEntity data.
   */
  private receiveMissingEntities(entities: WorldEntity[]): void {
    this.addEntities(entities);
    if (this.queuedRequestIds.length > 0) {
      this.requestIds = this.queuedRequestIds;
      this.queuedRequestIds = [];
      this.connection.send("requestEntityData", this.requestIds);
    }
    else {
      this.requestIds = [];
      this.isRequestingData = false;
    }
  }
}
