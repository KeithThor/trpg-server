import { Component, HostListener, OnInit, ViewChild, ViewChildren, OnDestroy } from "@angular/core";
import { Coordinate } from "../model/coordinate.model";
import { Subscription } from "rxjs/Subscription";
import { ChatboxComponent } from "../chatbox/chatbox.component";
import { GameStateService } from "../services/game-state.service";
import { MapTile } from "../model/map-data.model";
import { WorldEntity } from "../model/world-entity.model";
import { WorldEntityAnimationConstants } from "../worldEntity/world-entity.component";
import { QueryList } from "@angular/core";
import { EntityLocation } from "../model/entity-location.model";
import { WorldEntityService } from "../services/world-entity.service";
import { TileNodeComponent, ContextData } from "./tile-node.component";
import { MovementManager, MovementConstants } from "./services/movement-manager.service";

@Component({
  selector: 'app-game-component',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css'],
  providers: [
    MovementManager
  ]
})
export class GameComponent implements OnInit, OnDestroy {
  constructor(private gameStateService: GameStateService,
    private worldEntityService: WorldEntityService,
    private movementManager: MovementManager) {

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

  public contextX: number;
  public contextY: number;
  public showContextMenu: boolean;
  public contextEntities: WorldEntity[];
  public contextLocation: Coordinate;

  ngOnInit(): void {
    this.subscriptions = [];
    this.entityLocations = [];
    this.entities = [];
    this.updateQueue = [];
    this.missingEntityIds = [];
    this.requestedIds = [];
    this.isAnimating = false;
    this.showContextMenu = false;
    this.initialize();
  }

  ngOnDestroy(): void {
    this.endConnections();
  }

  /**
   * Initializes the services and connections for the game component.
   */
  private async initialize(): Promise<void> {
    await this.gameStateService.initializeAsync()
      .catch(err => console.log(err));

    this.subscriptions.push(this.worldEntityService.onUpdateLocations.subscribe({
      next: (locations: EntityLocation[]) => {
        this.onUpdateLocation(locations);
      }
    }));

    this.subscriptions.push(this.worldEntityService.onCanStartBattle.subscribe({
      next: (canStartBattle: boolean) => {
        this.canStartBattleAsync();
      }
    }));

    this.subscriptions.push(this.worldEntityService.onAddEntities.subscribe({
      next: (entities: WorldEntity[]) => {
        this.onAddEntities(entities);
      }
    }));

    this.subscriptions.push(this.worldEntityService.onChangeMap.subscribe({
      next: (mapId: number) => {
        this.onChangeMaps(mapId);
      }
    }));

    this.subscriptions.push(this.worldEntityService.onRemoveEntities.subscribe({
      next: (entityIds: number[]) => {
        this.onRemoveEntities(entityIds);
      }
    }));

    this.subscriptions.push(this.worldEntityService.onReceiveMissingEntities.subscribe({
      next: (entities: WorldEntity[]) => {
        this.onAddEntities(entities);
        // Remove added entities from requested ids array
        this.requestedIds = this.requestedIds.filter(id => entities.some(entity => entity.id === id));
      }
    }));

    await this.gameStateService.beginPlayAsync();
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

  /**
   * Invoked whenever the user right clicks on any TileNodeComponents in the template.
   *
   * Moves the user's WorldEntity to the coordinate of the TileNode that was right clicked if there are no
   * WorldEntities that exist in that given spot.
   *
   * Shows the context menu if there are one or more WorldEntities that exist at the coordinate of the
   * TileNode.
   * @param data Contains data about the TileNodeComponent that was right-clicked.
   */
  public openContextMenu(data: ContextData): void {
    event.preventDefault();
    this.contextLocation = data.location;
    // If there are no entities at the given coordinate, do a simple move here
    if (data.entity == null) {
      this.onMoveContext();
      return;
    }

    this.contextEntities = this.getEntitiesAtCoordinate(data.location);
    this.contextX = data.contextEvent.clientX;
    this.contextY = data.contextEvent.clientY;
    this.showContextMenu = true;
  }

  /**
   * Returns all of the WorldEntities that exist at the given Coordinate on the map.
   * @param coordinate The Coordinate to get the WorldEntities from.
   */
  private getEntitiesAtCoordinate(coordinate: Coordinate): WorldEntity[] {
    let entityLocations = this.entityLocations.filter(eloc => eloc.location.positionX === coordinate.positionX &&
                                                              eloc.location.positionY === coordinate.positionY);

    return this.entities.filter(entity => entityLocations.some(eloc => entity.id === eloc.id));
  }

  /**
   * Invoked whenever the user selects the Attack button from the context menu.
   * @param entity The WorldEntity that the user selected to Attack.
   */
  public onAttackContext(entity: WorldEntity): void {
    this.movementManager.actOnEntity(entity, MovementConstants.attack);
  }

  /**
   * Invoked whenever the user selects the Join button from the context menu.
   * @param entity The WorldEntity that the user selected to join.
   */
  public onJoinContext(entity: WorldEntity): void {
    this.movementManager.actOnEntity(entity, MovementConstants.join);
  }

  /**Invoked whenever the user selects the Move button from the context menu. */
  public onMoveContext(): void {
    this.movementManager.moveEntity(this.contextLocation);
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
    // Request data if there are any entities that are missing from memory
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
   * Whenever entities are removed from the map, remove them from memory.
   * @param entityIds The ids of the entities to remove from memory.
   */
  private onRemoveEntities(entityIds: number[]): void {
    for (var i = this.entities.length - 1; i <= 0; i++) {
      if (entityIds.indexOf(this.entities[i].id) !== -1) {
        this.entities.splice(i, 1);
      }
    }
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
    if (this.getTileIds() == null) return;

    // Too far behind on updates, just skip to newest update
    if (this.updateQueue.length >= 5) {
      let locations = this.updateQueue[this.updateQueue.length - 1];
      this.entityLocations = locations;
      this.updateQueue = [];
      return;
    }
    
    this.isAnimating = true;
    let entityLocations = this.updateQueue.shift();
    let rowLength = this.getTileIds()[0].length;

    this.tileNodeComponents.forEach((component, index) => {
      // On last index, start next recursive loop after a delay equal to animation time
      if (index === this.tileNodeComponents.length - 1) {
        setTimeout((() => {
          this.entityLocations = entityLocations;
          this.isAnimating = false;
          if (this.updateQueue.length > 0) {
            // Adds recursion to call stack to avoid blocking the rest of the application
            setTimeout(() => this.animateEntities(), 0);
          }
        }).bind(this), 100);
      }

      if (component.entity == null) return;

      let eLocation: EntityLocation = entityLocations.find(eLoc => eLoc.id === component.entity.id);
      if (eLocation == null) return;

      let worldEntityComponent = component.worldEntityComponent;
      if (worldEntityComponent == null) return;

      let coord = new Coordinate();
      coord.positionX = Math.floor(index / rowLength);
      coord.positionY = Math.floor(index % rowLength);

      if (eLocation.location.positionX - coord.positionX > 0) {
        worldEntityComponent.animationState = WorldEntityAnimationConstants.moveDown;
      }
      else if (eLocation.location.positionX - coord.positionX < 0) {
        worldEntityComponent.animationState = WorldEntityAnimationConstants.moveUp;
      }
      else if (eLocation.location.positionY - coord.positionY > 0) {
        worldEntityComponent.animationState = WorldEntityAnimationConstants.moveRight;
      }
      else if (eLocation.location.positionY - coord.positionY < 0) {
        worldEntityComponent.animationState = WorldEntityAnimationConstants.moveLeft;
      }
      else {
        worldEntityComponent.animationState = WorldEntityAnimationConstants.stationary;
      }
    });
  }
}
