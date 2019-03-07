export class MapData {
  mapData: number[][];
  uniqueTiles: MapTile[];
}

export class MapTile {
  id: number;
  iconUri: string;
  isBlocking: boolean;
}
