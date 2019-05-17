"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var coordinate_model_1 = require("../model/coordinate.model");
/**Class responsible for finding a path to a target position on the map. */
var Pathfinder = /** @class */ (function () {
    function Pathfinder() {
    }
    /**
     * Finds a path to the target coordinate from the start coordinate on the provided map.
     * @param start The starting coordinate.
     * @param target The target coordinate.
     * @param map The mapdata for the map to traverse.
     */
    Pathfinder.findPath = function (start, target, map) {
        var closedSet = new PathDictionary(map.mapData.length, map.mapData[0].length);
        var openSet = [];
        openSet.push(start);
        var cameFrom = new PathDictionary(map.mapData.length, map.mapData[0].length);
        var gScore = new PathDictionary(map.mapData.length, map.mapData[0].length);
        var fScore = new PathDictionary(map.mapData.length, map.mapData[0].length);
        gScore.setValue(start, 0);
        fScore.setValue(start, this.calculateHeuristic(start, target));
        while (openSet.length > 0) {
            var current = openSet.pop();
            // Found shortest path
            if (current.positionX === target.positionX && current.positionY === target.positionY) {
                return this.reconstructPath(cameFrom, current);
            }
            closedSet.setValue(current, true);
            var evaluatedNeighbors = false;
            while (!evaluatedNeighbors) {
            }
        }
    };
    /**
     * Creates Coordinate objects that neighbor a given Coordinate.
     *
     * Avoids going out of bounds on the map.
     * @param coordinate The Coordinate to create neighboring Coordinates for.
     * @param map The map whose data is used to prevent going out of bounds.
     */
    Pathfinder.createNeighborCoords = function (coordinate, map) {
        var coords = [];
        coords.push(new coordinate_model_1.Coordinate({ positionX: coordinate.positionX + 1, positionY: coordinate.positionY }));
        coords.push(new coordinate_model_1.Coordinate({ positionX: coordinate.positionX - 1, positionY: coordinate.positionY }));
        coords.push(new coordinate_model_1.Coordinate({ positionX: coordinate.positionX, positionY: coordinate.positionY + 1 }));
        coords.push(new coordinate_model_1.Coordinate({ positionX: coordinate.positionX, positionY: coordinate.positionY - 1 }));
        return coords;
    };
    /**
     * Reconstructs the path found into an array of Coordinates sorted from the start to the target destination.
     * @param cameFrom A PathDictionary containing the Coordinate of a node that led to another node.
     * @param current The Coordinate of the target node for pathfinding.
     */
    Pathfinder.reconstructPath = function (cameFrom, current) {
        var path = [];
        path.push(current);
        while (cameFrom.getValue(current) != null) {
            current = cameFrom.getValue(current);
            path.push(current);
        }
        return path.reverse();
    };
    /**
     * Calculates an estimate of how many moves it might take to move from the start coordinate to the target
     * coordinate.
     * @param start The coordinate used as the starting point.
     * @param target The coordinate used as the end point.
     */
    Pathfinder.calculateHeuristic = function (start, target) {
        var dx = Math.abs(start.positionX - target.positionX);
        var dy = Math.abs(start.positionY - target.positionY);
        // If difficult terrain is ever added, should add a value here to multiply movement cost
        return dx + dy;
    };
    return Pathfinder;
}());
exports.Pathfinder = Pathfinder;
/**Dictionary that is used to hold path data used for pathfinding. */
var PathDictionary = /** @class */ (function () {
    function PathDictionary(xSize, ySize) {
        this.dictionary = [];
        this.filledCoordinates = {};
        for (var i = 0; i < xSize; i++) {
            var row = [];
            for (var j = 0; j < ySize; j++) {
                row.push(null);
            }
            this.dictionary.push(row);
        }
    }
    /**
     * Gets stored data that corresponds to the given coordinate key value.
     * @param coordinate The coordinate used as the key to retrieve a value from the dictionary.
     */
    PathDictionary.prototype.getValue = function (coordinate) {
        if (this.dictionary[coordinate.positionX][coordinate.positionY] == null)
            return null;
        return this.dictionary[coordinate.positionX][coordinate.positionY].data;
    };
    /**
     * Gets stored data that corresponds to the coordinate of the given x and y positions.
     * @param posX The x position of the coordinate.
     * @param posY The y position of the coordinate.
     */
    PathDictionary.prototype.getValueNumeric = function (posX, posY) {
        if (this.dictionary[posX][posY] == null)
            return null;
        return this.dictionary[posX][posY].data;
    };
    /**Gets all values stored in the PathDictionary. */
    PathDictionary.prototype.getAllValues = function () {
        var values = [];
        for (var key in Object.keys(this.filledCoordinates)) {
            values.push(this.filledCoordinates[key]);
        }
        return values;
    };
    /**
     * Sets the value corresponding to the provided coordinate key to the provided value.
     * @param coordinate The coordinate used as the key to set the value in the dictionary.
     * @param data The value to set in the dictionary.
     */
    PathDictionary.prototype.setValue = function (coordinate, data) {
        var obj = new DictionaryObject();
        obj.coordinate = coordinate;
        obj.data = data;
        this.dictionary[coordinate.positionX][coordinate.positionY] = obj;
        var key = coordinate.positionX + "," + coordinate.positionY;
        this.filledCoordinates[key] = obj;
    };
    /**
     * Removes the value corresponding to the coordinate key in the dictionary.
     * @param coordinate The coordinate used as the key in the dictionary.
     */
    PathDictionary.prototype.remove = function (coordinate) {
        var obj = this.dictionary[coordinate.positionX][coordinate.positionY];
        this.dictionary[coordinate.positionX][coordinate.positionY] = null;
        var key = coordinate.positionX + "," + coordinate.positionY;
        delete this.filledCoordinates[key];
        if (obj == null)
            return null;
        else
            return obj.data;
    };
    return PathDictionary;
}());
/**Object representing stored data in a PathDictionary. */
var DictionaryObject = /** @class */ (function () {
    function DictionaryObject() {
    }
    return DictionaryObject;
}());
//# sourceMappingURL=pathfinder.static.js.map