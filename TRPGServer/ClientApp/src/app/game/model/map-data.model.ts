export class MapData {
  mapData: number[][];
  uniqueTiles: MapTile[];
}

export class MapTile {
  id: number;
  iconUris: string[];
  isBlocking: boolean;
}
