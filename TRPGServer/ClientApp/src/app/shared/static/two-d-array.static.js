"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/** Generic static class containing methods to handle manipulating two dimensional arrays. */
var TwoDArray = /** @class */ (function () {
    function TwoDArray() {
    }
    /**
     * Returns the first object that satisfies the provided predicate.
     * @param array The two dimensional array to find the object in.
     * @param predicate The function used to find the desired object.
     */
    TwoDArray.find = function (array, predicate) {
        if (array == null)
            return null;
        if (predicate == null)
            return null;
        var foundObj = null;
        var found = false;
        var i = 0;
        while (!found && i < array.length) {
            var j = 0;
            if (array[i] != null) {
                while (!found && j < array[i].length) {
                    if (array[i][j] != null && predicate(array[i][j])) {
                        foundObj = array[i][j];
                        found = true;
                    }
                    j++;
                }
            }
            i++;
        }
        return foundObj;
    };
    /**
     * Calls a callback function for each element of a two dimensional array.
     * @param array The two dimensional array to loop through.
     * @param callbackfunc The callback function to call for each element of the array.
     * @param thisArg Optional parameter to substitute for the this parameter of the callback function.
     */
    TwoDArray.forEach = function (array, callbackfunc, thisArg) {
        if (array == null)
            return;
        if (callbackfunc == null)
            return;
        array.forEach(function (row, xIndex) {
            if (row == null)
                return;
            row.forEach(function (obj, yIndex) {
                if (thisArg == null)
                    callbackfunc(obj, xIndex, yIndex, array);
                else
                    callbackfunc.apply(thisArg, [obj, xIndex, yIndex, array]);
            });
        });
    };
    /**
     * Returns an array containing unique objects stored in the two dimensional array.
     * @param array The array to find unique objects from.
     */
    TwoDArray.getUnique = function (array) {
        var unique = [];
        if (array == null)
            return null;
        array.forEach(function (row) {
            if (row == null)
                return;
            row.forEach(function (obj) {
                if (obj == null)
                    return;
                if (!unique.some(function (val) { return val === obj; })) {
                    unique.push(obj);
                }
            });
        });
        return unique;
    };
    /**
     * Gets the indeces of an object that satisfies a given predicate.
     *
     * The first index in the array is the row index.
     * The second index in the array is the column index.
     *
     * array[firstIndex][secondIndex]
     * @param array
     * @param predicate
     */
    TwoDArray.findIndex = function (array, predicate) {
        if (array == null)
            return null;
        if (predicate == null)
            return null;
        var indeces = [];
        var found = false;
        var i = 0;
        while (!found && i < array.length) {
            if (array[i] != null) {
                var j = 0;
                while (!found && j < array[i].length) {
                    if (array[i][j] != null && predicate(array[i][j])) {
                        indeces.push(i);
                        indeces.push(j);
                        found = true;
                    }
                    j++;
                }
            }
            i++;
        }
        return indeces;
    };
    return TwoDArray;
}());
exports.TwoDArray = TwoDArray;
//# sourceMappingURL=two-d-array.static.js.map