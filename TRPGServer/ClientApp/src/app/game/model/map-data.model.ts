/**An object containing static map data. */
export class MapData {
  mapData: number[][];
  mapId: number;
  uniqueTiles: MapTile[];
}

/**Represents a tile on the map. */
export class MapTile {
  id: number;
  iconUris: string[];
  isBlocking: boolean;
  canTransport: boolean;
}
