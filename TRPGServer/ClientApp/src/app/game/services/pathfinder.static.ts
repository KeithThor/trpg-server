import { Coordinate } from "../model/coordinate.model";
import { MapTile, MapData } from "../model/map-data.model";
import { PriorityQueue, Dictionary, KeyValuePair } from "../../shared/static/data-structures.static";

/**Class responsible for finding a path to a target position on the map. */
export class Pathfinder {
  /**
   * Finds a path to the target coordinate from the start coordinate on the provided map.
   * @param start The starting coordinate.
   * @param target The target coordinate.
   * @param map The mapdata for the map to traverse.
   */
  public static findPath(start: Coordinate, target: Coordinate, map: MapData): Coordinate[] {
    let coordHasher = (coord: Coordinate) => coord.positionX + "," + coord.positionY;

    // Dictionary of evaluated nodes
    let closedSet: Dictionary<Coordinate, boolean> = new Dictionary(coordHasher);

    // Queue of unevaluated nodes neighboring previously evaluated nodes sorted from highest priority to lowest
    let openSet: PriorityQueue<KeyValuePair<Coordinate, number>> = new PriorityQueue((item, comp) => comp.value - item.value);

    // Dictionary of unevaluated nodes neighboring previously evaluated nodes, keeps track of Coordinates in openSet
    let openSetDictionary: Dictionary<Coordinate, number> = new Dictionary(coordHasher);

    // Insert start node
    openSet.insert(new KeyValuePair<Coordinate, number>({ key: start, value: 0 }));

    // Dictionary containing a Coordinate as well as the neighboring Coordinate that must be traversed to reach that Coordinate
    let cameFrom: Dictionary<Coordinate, Coordinate> = new Dictionary(coordHasher);

    // Contains the gScore of each Coordinate, gScore being the amount of moves it takes to reach that node from the start
    let gScore: Dictionary<Coordinate, number> = new Dictionary(coordHasher);

    // Contains the fScore of each Coordinate, fScore being the gScore of a coordinate and the heuristic of that coordinate
    // to the target coordinate
    let fScore: Dictionary<Coordinate, number> = new Dictionary(coordHasher);

    gScore.setValue(start, 0);
    fScore.setValue(start, this.calculateHeuristic(start, target));
    let distanceBetweenNeighbors = 1;

    while (openSet.length > 0) {
      // Take highest priority Coordinate from the queue
      let current = openSet.dequeue().key;
      openSetDictionary.remove(current);

      // Found shortest path
      if (current.positionX === target.positionX && current.positionY === target.positionY) {
        return this.reconstructPath(cameFrom, current);
      }

      closedSet.setValue(current, true);
      let neighbors: Coordinate[] = this.createNeighborCoords(current, map)
                                        .filter(coord => closedSet.getValue(coord) == null);

      neighbors.forEach(coord => {
        // Distance from start to the current neighbor node
        let tentativeGScore = gScore.getValue(current) + distanceBetweenNeighbors;
        let neighborFScore = tentativeGScore + this.calculateHeuristic(coord, target);

        if (openSetDictionary.getValue(coord) == null) {
          openSetDictionary.setValue(coord, neighborFScore);
          openSet.insert(new KeyValuePair({ key: coord, value: neighborFScore }));
        }
        else if (tentativeGScore >= gScore.getValue(coord)) return;

        cameFrom.setValue(coord, current);
        gScore.setValue(coord, tentativeGScore);
        fScore.setValue(coord, neighborFScore);
      });
    }

    throw new Error("Could not find path from (" + start.positionX + "," + start.positionY + ") to ("
                    + target.positionX + "," + target.positionY + "!");
  }

  /**
   * Creates Coordinate objects that neighbor a given Coordinate.
   *
   * Avoids going out of bounds on the map.
   * @param coordinate The Coordinate to create neighboring Coordinates for.
   * @param map The map whose data is used to prevent going out of bounds.
   */
  private static createNeighborCoords(coordinate: Coordinate, map: MapData): Coordinate[] {
    let coords: Coordinate[] = [];
    coords.push(new Coordinate({ positionX: coordinate.positionX + 1, positionY: coordinate.positionY }));
    coords.push(new Coordinate({ positionX: coordinate.positionX - 1, positionY: coordinate.positionY }));
    coords.push(new Coordinate({ positionX: coordinate.positionX, positionY: coordinate.positionY + 1 }));
    coords.push(new Coordinate({ positionX: coordinate.positionX, positionY: coordinate.positionY - 1 }));

    return coords.filter(coord => this.isValidLocation(coord, map));
  }

  /**
   * Returns true if a given coordinate on the map is a valid location to move to.
   * @param coordinate The coordinate to check validity for.
   * @param map The map data of the map to traverse.
   */
  private static isValidLocation(coordinate: Coordinate, map: MapData): boolean {
    if (coordinate.positionX >= map.mapData.length) return false;
    if (coordinate.positionY >= map.mapData[0].length) return false;
    if (coordinate.positionX < 0 || coordinate.positionY < 0) return false;

    let tile: MapTile = map.uniqueTiles.find(unique => unique.id === map.mapData[coordinate.positionX][coordinate.positionY]);
    return !tile.isBlocking;
  }

  /**
   * Reconstructs the path found into an array of Coordinates sorted from the start to the target destination.
   * @param cameFrom A PathDictionary containing the Coordinate of a node that led to another node.
   * @param current The Coordinate of the target node for pathfinding.
   */
  private static reconstructPath(cameFrom: Dictionary<Coordinate, Coordinate>, current: Coordinate): Coordinate[] {
    let path: Coordinate[] = [];
    path.push(current);

    while (cameFrom.getValue(current) != null) {
      current = cameFrom.getValue(current);
      path.push(current);
    }

    return path.reverse();
  }

  /**
   * Calculates an estimate of how many moves it might take to move from the start coordinate to the target
   * coordinate.
   * @param start The coordinate used as the starting point.
   * @param target The coordinate used as the end point.
   */
  private static calculateHeuristic(start: Coordinate, target: Coordinate): number {
    let dx = Math.abs(start.positionX - target.positionX);
    let dy = Math.abs(start.positionY - target.positionY);

    // If difficult terrain is ever added, should add a value here to multiply movement cost
    return dx + dy;
  }
}
