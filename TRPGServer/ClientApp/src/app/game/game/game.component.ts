import { Component, HostListener, OnInit, ViewChild, ViewChildren } from "@angular/core";
import { Coordinate } from "../model/coordinate.model";
import { Router, NavigationStart } from "@angular/router";
import { Subscription } from "rxjs/Subscription";
import { ChatboxComponent } from "../chatbox/chatbox.component";
import { GameStateService } from "../services/game-state.service";
import { MapTile } from "../model/map-data.model";
import { WorldEntity } from "../model/world-entity.model";
import { WorldEntityAnimationConstants, WorldEntityComponent } from "../worldEntity/world-entity.component";
import { QueryList } from "@angular/core";
import { EntityLocation } from "../model/entity-location.model";
import { WorldEntityService } from "../services/world-entity.service";
import { TileNodeComponent } from "./tile-node.component";

@Component({
  selector: 'app-game-component',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css'],
})
export class GameComponent implements OnInit {
  constructor(private gameStateService: GameStateService,
    private worldEntityService: WorldEntityService,
    private router: Router) {

  }
  @ViewChild("chatbox") chatbox: ChatboxComponent;
  @ViewChildren(TileNodeComponent) tileNodeComponents: QueryList<TileNodeComponent>;
  private subscriptions: Subscription[];
  private entityLocations: EntityLocation[];
  private entities: WorldEntity[];
  private updateQueue: EntityLocation[][];
  private isAnimating: boolean;
  private missingEntityIds: number[];
  private requestedIds: number[];

  ngOnInit(): void {
    this.subscriptions = [];
    this.entityLocations = [];
    this.entities = [];
    this.updateQueue = [];
    this.missingEntityIds = [];
    this.requestedIds = [];
    this.isAnimating = false;
    this.initialize();
    this.subscriptions.push(this.router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        this.endConnections();
      }
    }));
  }

  @HostListener('document: keydown', ['$event'])
  public onKey(event: KeyboardEvent): void {
    if (this.gameStateService.initialized) {
      if (event.key === "ArrowLeft") {
        let movement = new Coordinate();
        movement.positionY = -1;
        this.gameStateService.moveEntity(movement);
      }
      if (event.key === "ArrowRight") {
        let movement = new Coordinate();
        movement.positionY = 1;
        this.gameStateService.moveEntity(movement);
      }
      if (event.key === "ArrowUp") {
        let movement = new Coordinate();
        movement.positionX = -1;
        this.gameStateService.moveEntity(movement);
      }
      if (event.key === "ArrowDown") {
        let movement = new Coordinate();
        movement.positionX = 1;
        this.gameStateService.moveEntity(movement);
      }
      if (event.key === "`") {
        this.gameStateService.changeMapsAsync();
      }
    }
  }

  @HostListener('window:beforeunload', ['$event'])
  public onBeginUnload(event: Event): void {
    this.endConnections();
  }

  @HostListener('contextmenu', ['$event'])
  public openContextMenu(event: Event): void {
    event.preventDefault();
    console.log("Context menu opens");
  }

  /**
   * Initializes the services and connections for the game component.
   */
  private async initialize(): Promise<void> {
    await this.gameStateService.initializeAsync()
      .catch(err => console.log(err));
    this.worldEntityService.onUpdateLocations(this.onUpdateLocation.bind(this));
    this.worldEntityService.canStartBattleHandler = this.canStartBattleAsync.bind(this);
    this.worldEntityService.addEntitiesCallback = this.onAddEntities.bind(this);
    this.worldEntityService.onChangeMaps(this.onChangeMaps.bind(this));
    await this.gameStateService.beginPlayAsync();
  }

  private async canStartBattleAsync(): Promise<void> {
    let accept = confirm("Do you want to start a battle?");
    if (accept) {
      await this.worldEntityService.initiateBattleAsync();
    }
  }

  /** Ends connections with each of the game services and this components subscriptions. */
  private endConnections(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
    this.gameStateService.endConnectionsAsync();
  }

  /** Returns true if the game map is ready to be shown to the player. */
  public showMap(): boolean {
    return this.gameStateService.initialized && this.gameStateService.getMapTileIds() != null;
  }

  public getTileIds(): number[][] {
    let tileIds = this.gameStateService.getMapTileIds();
    return tileIds;
  }

  /**
   * Gets a map tile instance from the game service given the id of the map tile.
   * @param mapTileId The id of the map tile to retrieve.
   */
  public getMapTile(mapTileId: number): MapTile {
    return this.gameStateService.getUniqueTiles()
      .find(tile => tile.id === mapTileId);
  }

  /**
   * Returns true if an entity exists in a location on the map grid and can be displayed.
   * @param positionX The row of the map the entity resides on.
   * @param positionY The column of the map the entity resides on.
   */
  public showEntity(positionX: number, positionY: number): boolean {
    if (!this.gameStateService.initialized) return false;
    if (this.entityLocations == null || this.entityLocations.length === 0) return false;

    let i = 0;
    let found: boolean = false;
    while (i < this.entityLocations.length && !found) {
      found = this.entityLocations[i].location.positionX === positionX
        && this.entityLocations[i].location.positionY === positionY;
      if (!found) {
        i++;
      }
    }
    
    return found;
  }

  /**
   * Returns the entity data of the WorldEntity that exists in a location on the map grid.
   * @param positionX The row of the map the entity resides on.
   * @param positionY The column of the map the entity resides on.
   */
  public getEntity(positionX: number, positionY: number): WorldEntity {
    if (positionX === 0 && positionY === 0) {
      if (this.missingEntityIds.length > 0) this.worldEntityService.requestEntityData(this.missingEntityIds);
      this.missingEntityIds = [];
    }

    let entity: EntityLocation = this.entityLocations.find(e =>
      e.location.positionX === positionX && e.location.positionY === positionY);

    if (entity == null) return null;

    //let entityId = this.gameStateService.getEntityLocations()[positionX][positionY];
    let foundEntity = this.entities.find(e => e.id === entity.id);
    if (foundEntity == null && this.missingEntityIds.indexOf(entity.id) === -1) this.missingEntityIds.push(entity.id);
    return foundEntity;
  }

  /**
   * Creates and returns a Coordinate based off of the given positions.
   * @param positionX The x position of the coordinate.
   * @param positionY The y position of the coordinate.
   */
  public getLocation(positionX: number, positionY: number): Coordinate {
    let location = new Coordinate();
    location.positionX = positionX;
    location.positionY = positionY;

    return location;
  }

  /**
   * Whenever there is a new update for entity locations from the server, push the new update to the updateQueue and call animateEntities
   * if the GameComponent isn't already animating.
   * @param entities A grid showing which entity occupies which space.
   * @param entityLocations An array of entity locations, giving the id of the entity and its current position on the map.
   */
  private onUpdateLocation(entityLocations: EntityLocation[]): void {
    if (this.entityLocations == null || this.entityLocations.length === 0) {
      this.entityLocations = entityLocations;
    }
    else {
      this.updateQueue.push(entityLocations);
      if (!this.isAnimating) {
        this.animateEntities();
      }
    }
  }

  /**
   * Whenever new entities are added, remove them from the request ids if they were being requested.
   * @param entities The new entities to add.
   */
  private onAddEntities(entities: WorldEntity[]): void {
    entities.forEach(e => {
      this.entities.push(e);
      var index = this.requestedIds.indexOf(e.id);
      if (index !== -1) this.requestedIds.splice(index, 1);

      var inde = this.missingEntityIds.indexOf(e.id);
      if (inde !== -1) this.missingEntityIds.splice(inde, 1);
    });
  }

  /**
   * Resets the entity locations stored by the GameComponent whenever a change map request has been successfully
   * approved by the server.
   * @param newMapId
   */
  private onChangeMaps(newMapId: number): Promise<void> {
    this.entityLocations = [];
    this.requestedIds = [];
    this.entities = [];
    this.updateQueue = [];
    this.missingEntityIds = [];
    return;
  }

  /** Detects changes from the current state of WorldEntities on the grid and the first update on the queue and
   * animates those changes on the current state before swapping entity locations.
   * 
   * Will run recursively until there are no more updates in the queue. */
  private animateEntities(): void {
    if (this.isAnimating || this.updateQueue.length === 0) return;
    
    this.isAnimating = true;
    let entityLocations = this.updateQueue.shift();

    entityLocations.forEach((entity, index) => {
      if (index === entityLocations.length - 1) {
        setTimeout((() => {
          this.entityLocations = entityLocations;
          this.isAnimating = false;
          if (this.updateQueue.length > 0) {
            setTimeout(() => this.animateEntities(), 0);
          }
        }).bind(this), 100);
      }

      let oldEntityLocation: EntityLocation = this.entityLocations.find(e => e.id === entity.id);
      if (oldEntityLocation == null) {
        if (index === entityLocations.length - 1) {
          this.entityLocations = entityLocations;
        }
        return;
      }

      let component = this.tileNodeComponents
        .find(c => c.entity != null && c.entity.id === entity.id);
      let worldEntityComponent: WorldEntityComponent;

      if (component != null) worldEntityComponent = component.worldEntityComponent;

      if (component == null || worldEntityComponent == null) {
        if (index === entityLocations.length - 1) {
          this.entityLocations = entityLocations;
        }
        return;
      }

      if (entity.location.positionX - oldEntityLocation.location.positionX > 0) {
        worldEntityComponent.animationState = WorldEntityAnimationConstants.moveDown;
      }
      else if (entity.location.positionX - oldEntityLocation.location.positionX < 0) {
        worldEntityComponent.animationState = WorldEntityAnimationConstants.moveUp;
      }
      else if (entity.location.positionY - oldEntityLocation.location.positionY > 0) {
        worldEntityComponent.animationState = WorldEntityAnimationConstants.moveRight;
      }
      else if (entity.location.positionY - oldEntityLocation.location.positionY < 0) {
        worldEntityComponent.animationState = WorldEntityAnimationConstants.moveLeft;
      }
      else {
        worldEntityComponent.animationState = WorldEntityAnimationConstants.stationary;
      }
    });
  }
}
