import { Injectable } from "@angular/core";
import { HubConnectionBuilder, HubConnection } from '@aspnet/signalr';
import { WorldEntity } from "../model/world-entity.model";
import { Coordinate } from "../model/coordinate.model";
import { LocalStorageConstants } from "../../constants";
import { EntityLocation } from "../model/entity-location.model";
import { StateHandlerService } from "./state-handler.service";
import { PlayerStateConstants } from "../player-state-constants.static";
import { ReplaySubject } from "rxjs";

/**Service responsible for sending and receiving WorldEntity data to and from the server. */
@Injectable()
export class WorldEntityService {
  constructor(private stateHandler: StateHandlerService) {
    this.isRequestingData = false;
    this.requestIds = [];
    this.queuedRequestIds = [];
    this.failedReconnections = 0;
  }

  private connection: HubConnection;

  private _playerEntityId: number;
  public get playerEntityId(): number {
    if (this._playerEntityId == null && this.canRequestData) {
      this.requestPlayerEntity();
      return null;
    }
    else return this._playerEntityId;
  }

  private requestIds: number[];
  private queuedRequestIds: number[];
  private isRequestingData: boolean;
  private failedReconnections: number;
  private canRequestData: boolean;

  public onChangeMap: ReplaySubject<number> = new ReplaySubject(1);
  public onUpdateLocations: ReplaySubject<EntityLocation[]> = new ReplaySubject(1);
  public onAddEntities: ReplaySubject<WorldEntity[]> = new ReplaySubject(5);
  public onReceiveMissingEntities: ReplaySubject<WorldEntity[]> = new ReplaySubject(5);
  public onRemoveEntities: ReplaySubject<number[]> = new ReplaySubject(5);
  public onCanStartBattle: ReplaySubject<boolean> = new ReplaySubject(1);
  public onMovementStopped: ReplaySubject<boolean> = new ReplaySubject(1);

  /** Initializes this service and starts the connection to the server. */
  public async initializeAsync(): Promise<void> {
    this.canRequestData = false;
    this.connection = new HubConnectionBuilder().withUrl("/hubs/worldentities",
      {
        accessTokenFactory: () => sessionStorage.getItem(LocalStorageConstants.authToken)
      })
      .build();
    this.connection.on("updateEntities",
      (entities: EntityLocation[]) => this.onUpdateLocations.next(entities));
    this.connection.on("addEntities", (entities: WorldEntity[]) => this.onAddEntities.next(entities));
    this.connection.on("removeEntities", (ids: number[]) => this.onRemoveEntities.next(ids));
    this.connection.on("battleInitiated", async () => await this.onBattleInitiatedAsync());
    this.connection.on("canStartBattle", () => this.onCanStartBattle.next(true))
    this.connection.on("invalidState", async (state: string) => await this.invalidStateAsync(state));
    this.connection.on("receiveMissingEntities", (entities: WorldEntity[]) => this.receiveMissingEntities(entities));
    this.connection.on("changeMapsSuccess", (newMapId: number) => this.onChangeMap.next(newMapId));
    this.connection.on("getMyEntity", (entityId: number) => this.getMyEntity(entityId));
    this.connection.on("movementStopped", () => this.onMovementStopped.next(true));

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
        setTimeout(() => this.canRequestData = true, 500);
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

  public async initiateBattleAsync(): Promise<void> {
    await this.connection.send("initiateBattle");
  }

  public async joinBattleAsync(toJoinId: string, isAttacker: boolean): Promise<void> {
    await this.connection.send("joinBattle", toJoinId, isAttacker);
  }

  ///**
  // * Moves the player entity an amount from it's current position.
  // * @param deltaPosition The change in X and Y coordinates to move the player entity.
  // */
  //public moveEntity(deltaPosition: Coordinate) {
  //  if (this.connection == undefined) throw new Error("The connection hasn't been started yet.");

  //  this.connection.send("MoveEntity", deltaPosition);
  //}

  /**
   * Moves the player's WorldEntity along the given path.
   * @param path The path to move the player's WorldEntity.
   */
  public moveEntity(path: Coordinate[]): Promise<void> {
    if (this.connection == null) throw new Error("The connection hasn't been started yet.");

    return this.connection.send("MoveEntity", path);
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
    if (!this.isRequestingData && this.canRequestData) {
      this.requestIds = entityIds;
      this.canRequestData = false;
      this.isRequestingData = true;
      this.connection.send("RequestEntityData", entityIds);
      setTimeout(() => this.canRequestData = true, 500);
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

  /**Sends a request to the server to get the WorldEntity that belongs to the player. */
  public requestPlayerEntity(): Promise<void> {
    this.canRequestData = false;
    setTimeout(() => this.canRequestData = true, 500);
    return this.connection.send("RequestPlayerEntity");
  }

  /** Called by the server whenever the player has successfully initiated combat. */
  private async onBattleInitiatedAsync(): Promise<void> {
    await this.stateHandler.handleStateAsync(PlayerStateConstants.inCombat);
  }

  /**
   * Called by the server if the player should not be in the Game map.
   * @param currentState The actual state the player is in.
   */
  private async invalidStateAsync(currentState: string): Promise<void> {
    await this.stateHandler.handleStateAsync(currentState);
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
   * Called by the server to receive the id of the player's entity.
   * @param entityId The id of the player's entity.
   */
  private getMyEntity(entityId: number): void {
    this._playerEntityId = entityId;
  }

  /**
   * Called whenever missing data is successfully retrieved by the server.
   * @param entities An array containing the missing WorldEntity data.
   */
  private receiveMissingEntities(entities: WorldEntity[]): void {
    this.onReceiveMissingEntities.next(entities);
    if (this.queuedRequestIds.length > 0 && this.canRequestData) {
      this.canRequestData = false;
      this.requestIds = this.queuedRequestIds;
      this.queuedRequestIds = [];
      this.connection.send("requestEntityData", this.requestIds);
      setTimeout(() => this.canRequestData = true, 500);
    }
    else {
      this.requestIds = [];
      this.isRequestingData = false;
    }
  }
}
