import { Injectable, OnDestroy } from "@angular/core";
import { WorldEntityService } from "../../services/world-entity.service";
import { Subscription } from "rxjs";
import { EntityLocation } from "../../model/entity-location.model";
import { Coordinate } from "../../model/coordinate.model";
import { MapService } from "../../services/map.service";
import { Pathfinder } from "../../services/pathfinder.static";
import { Dictionary } from "../../../shared/static/data-structures.static";
import { WorldEntity } from "../../model/world-entity.model";
import { MapTile } from "../../model/map-data.model";

/**Manager responsible for moving the player's WorldEntity around the map. */
@Injectable()
export class MovementManager implements OnDestroy {
  public constructor(private worldEntityService: WorldEntityService,
                     private mapService: MapService) {
    this.subscriptions = [];

    this.initialize();
  }

  private initialize(): void {
    this.entityLocations = new Dictionary((item) => item.toString());
    this.currentPath = new Dictionary((coord) => coord.positionX + "," + coord.positionY);

    this.subscriptions.push(this.worldEntityService.onUpdateLocations.subscribe({
      next: (locations: EntityLocation[]) => {
        this.updateTracking(locations);
      }
    }));

    this.subscriptions.push(this.worldEntityService.onRemoveEntities.subscribe({
      next: (entityIds: number[]) => {
        entityIds.forEach(id => this.entityLocations.remove(id));
      }
    }));

    this.subscriptions.push(this.worldEntityService.onMovementStopped.subscribe({
      next: (isStopped: boolean) => {
        this.onMovementStopped();
      }
    }));
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  private subscriptions: Subscription[];
  private trackedEntity: WorldEntity;
  private trackedEntityLocation: Coordinate;
  private entityLocations: Dictionary<number, Coordinate>;
  private trackedAction: string;

  // Only used to highlight map tile nodes, MovementManager does not care about path order
  public currentPath: Dictionary<Coordinate, boolean>;

  /**Resets the tracking that the MovementManager has placed on a given WorldEntity. */
  private resetTracking(): void {
    this.trackedEntity = null;
    this.trackedEntityLocation = null;
    this.trackedAction = null;
  }

  /**
   * Updates the locations of all WorldEntities.
   *
   * If the user has targeted a WorldEntity and that entity has moved, will find a path to the
   * new target location and try to act on it.
   * @param locations The locations of all WorldEntities on the map.
   */
  private updateTracking(locations: EntityLocation[]): void {
    let trackedEntityMoved = false;
    let playerEntityId = this.worldEntityService.playerEntityId;

    locations.forEach(location => {
      if (location.id === playerEntityId) {
        this.currentPath.remove(location.location);
      }
      if (this.trackedEntity != null && location.id === this.trackedEntity.id) {
        // If tracked entity moved
        if (location.location.positionX !== this.trackedEntityLocation.positionX
          || location.location.positionY !== this.trackedEntityLocation.positionY) {
          trackedEntityMoved = true;
          this.trackedEntityLocation = location.location;
        }
      }

      this.entityLocations.setValue(location.id, location.location);
    });

    // Tracked entity moved, get new path to target
    if (trackedEntityMoved) {
      this.actOnEntity(this.trackedEntity, this.trackedAction);
    }
  }

  /**Called when the player's WorldEntity has stopped moving.
   *
   * This can be because the WorldEntity has reached the target location or because there are no
   * valid movements left in the path that was provided.
   *
   * Tries to act on a target WorldEntity if there was a target.*/
  private onMovementStopped(): void {
    // This was a move order, do nothing after
    if (this.trackedAction == null || this.trackedAction === MovementConstants.move) {
      return;
    }
    // Change maps after reaching destination
    else if (this.trackedAction === MovementConstants.changeMaps) {
      this.worldEntityService.changeMapAsync();
      return;
    }
    else if (this.trackedEntity == null) return;

    this.actOnEntity(this.trackedEntity, this.trackedAction);
  }

  /**Returns the Coordinate location of the player's WorldEntity on the current map. */
  private getPlayerLocation(): Coordinate {
    let entityId = this.worldEntityService.playerEntityId;
    return this.entityLocations.getValue(entityId);
  }

  /**
   * Returns an array of Coordinates that form a path from the player's WorldEntity to the given
   * target coordinate.
   * @param target The coordinate to create a path to.
   */
  private getPath(target: Coordinate): Coordinate[] {
    let playerEntityLocation = this.getPlayerLocation();
    // Starting position and target are the same positions
    if (playerEntityLocation.positionX === target.positionX
      && playerEntityLocation.positionY === target.positionY) return [];

    let mapData = this.mapService.mapData;

    // Todo: use this path to highlight the map to show where the entity will move
    try {
      return Pathfinder.findPath(playerEntityLocation, target, mapData);
    }
    catch (ex) {
      return [];
    }
  }

  /**
   * Moves the player's WorldEntity to the given target Coordinate.
   * @param target The target Coordinate to move to.
   */
  public moveEntity(target: Coordinate): void {
    this.resetTracking();

    let path = this.getPath(target);
    if (path != null) this.storePath(path);
    
    let steppedTileId = this.mapService.mapData.mapData[target.positionX][target.positionY];
    let steppedTile: MapTile = this.mapService.mapData.uniqueTiles.find(tile => tile.id === steppedTileId);

    // If this tile is a door, automatically change maps
    if (steppedTile.canTransport) this.trackedAction = MovementConstants.changeMaps;
    else this.trackedAction = MovementConstants.move;

    this.worldEntityService.moveEntity(path);
  }

  /**
   * Attempts to perform an action on the given WorldEntity.
   * Will move to the target entity's position before performing the action.
   * @param entity The entity being targeted.
   * @param action The action to perform.
   *
   * Import Movement Constants to see the available actions.
   */
  public actOnEntity(entity: WorldEntity, action: string): void {
    this.resetTracking();

    let location = this.entityLocations.getValue(entity.id);
    if (location == null) return;

    this.trackedAction = action;
    this.trackedEntity = entity;
    this.trackedEntityLocation = location;

    let path = this.getPath(location);
    this.storePath(path);

    // Perform action
    this.worldEntityService.queueAction(entity.id, entity.ownerId, action, path);
  }

  /**
   * Stores the given path into the currentPath dictionary.
   * @param path An array of Coordinates.
   */
  private storePath(path: Coordinate[]): void {
    this.currentPath.clear();
    path.forEach(coord => this.currentPath.setValue(coord, true));
  }
}

export class MovementConstants {
  public static attack: string = "attack";
  public static join: string = "join";
  public static move: string = "move";
  public static changeMaps: string = "changemaps";
}
