import { Injectable } from "@angular/core";
import { MapService } from "./map.service";
import { WorldEntityService } from "./world-entity.service";
import { MapTile, MapData } from "../model/map-data.model";
import { Coordinate } from "../model/coordinate.model";
import { ChatService } from "./chat.service";

/** Service responsible for handling and relaying data between other game related services. */
@Injectable()
export class GameStateService {
  constructor(private mapService: MapService,
    private worldEntityService: WorldEntityService,
    private chatService: ChatService) {
    this.initialized = false;
    this.mapId = -1;
  }
  private mapId: number;
  public initialized: boolean;
  public onMapLoaded: () => void;

  /**
   * Called to initialize all game related services asynchronously.
   */
  public async initializeAsync(): Promise<void> {
    await Promise.all([
      this.mapService.loadMapAsync(),
      this.chatService.initializeAsync(),
      this.worldEntityService.initializeAsync()
    ]);

    this.worldEntityService.onChangeMap.subscribe({
      next: this.changeMapsHandlerAsync.bind(this)
    });
  }

  /** Called to begin playing the game after all services are initialized. */
  public async beginPlayAsync(): Promise<void> {
    await this.worldEntityService.beginPlayAsync();
    this.initialized = true;
  }

  /**
   * Handles map change events by calling map changes in other game-related services.
   * @param newMapId The id of the map the player's entity changed to.
   */
  private async changeMapsHandlerAsync(newMapId: number): Promise<void> {
    this.initialized = false;
    await Promise.all([
      this.mapService.loadMapAsync(newMapId),
      this.chatService.changeMapGroupAsync(this.mapId),
      this.worldEntityService.beginPlayAsync()
    ])
    this.mapId = newMapId;
    this.initialized = true;
    if (this.onMapLoaded != null) this.onMapLoaded();
  }

  /** Sends a request to the server to change maps. The server decides on which map is changed to based on the
   * location of the player's entity. */
  public changeMapsAsync(): Promise<void> {
    return this.worldEntityService.changeMapAsync();
  }

  /** Called to end all server connections for game related services asynchronously. */
  public async endConnectionsAsync(): Promise<void> {
    await Promise.all([
      this.worldEntityService.endConnectionAsync(),
      this.chatService.endConnectionAsync()
    ]);
    this.initialized = false;
  }

  /** Gets the id of the map the player is currently in. */
  public getCurrentMapId(): number {
    return this.mapId;
  }

  /** Gets the map grid containing the ids  */
  public getMapTileIds(): number[][] {
    if (!this.initialized) return null;
    return this.mapService.mapTileIds;
  }

  /**Gets the MapData for the currently loaded map. */
  public getMapData(): MapData {
    return this.mapService.mapData;
  }

  /** Gets all the unique tile data for the current map. */
  public getUniqueTiles(): MapTile[] {
    if (!this.initialized) return null;
    return this.mapService.uniqueTiles;
  }
}
