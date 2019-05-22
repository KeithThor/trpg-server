import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { MapTile, MapData } from "../model/map-data.model";

/**
 * Service responsible for getting and returning static map data from the server.
 */
@Injectable()
export class MapService {
  constructor(private http: HttpClient) {
    this.mapTileIds = [];
    this.uniqueTiles = [];
    this.mapId = -1;
  }

  public mapId: number;
  public mapTileIds: number[][];
  public uniqueTiles: MapTile[];
  public mapData: MapData;
  private failedReconnects: number;

  /**
   * Loads static map data from the server asynchronously.
   * @param mapId The id of the map to return from the server.
   */
  public loadMapAsync(mapId?: number): Promise<void> {
    if (mapId === this.mapId) return;

    return this.getMapDataAsync(mapId)
      .then(data => {
        let mapData = data as MapData;
        this.mapData = mapData;
        this.mapTileIds = mapData.mapData;
        this.uniqueTiles = mapData.uniqueTiles;

        if (!this.verifyMapTiles()) {
          return this.loadMapAsync(mapData.mapId);
        }
        this.mapId = mapData.mapId;
      });
  }

  /**
   * Sends a request to the server to retrieve map data asynchronously.
   * @param mapId The id of the map to retrieve from the server.
   */
  private getMapDataAsync(mapId?: number): Promise<MapData> {
    let params = new HttpParams();
    if (mapId != null) params.set("mapId", mapId.toString());

    return this.http.get<MapData>("/api/mapdata/map", { params: params })
      .toPromise<MapData>()
      .catch(err => {
        console.log(err);
        this.failedReconnects++;

        if (this.failedReconnects <= 5) {
          return new Promise(() => setTimeout(() => this.loadMapAsync(mapId), 250));
        }
        else {
          throw "Could not connect to the Map Server";
        }
      });
  }

  /**
   * Checks map data to verify that there are no missing tile data for each unique tile id.
   */
  private verifyMapTiles(): boolean {
    let tileIds: number[] = [];

    for (var i = 0; i < this.mapTileIds.length; i++) {
      for (var j = 0; j < this.mapTileIds[i].length; j++) {
        if (tileIds.indexOf(this.mapTileIds[i][j]) === -1) {
          tileIds.push(this.mapTileIds[i][j]);
        }
      }
    }

    tileIds.forEach(id => {
      if (!this.uniqueTiles.some(tile => tile.id === id)) {
        return false;
      }
    });

    console.log("Successfully verified map tiles.");
    return true;
  }
}
