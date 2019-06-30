"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var formation_model_1 = require("../model/formation.model");
/**Static class used for Ability targetting purposes. */
var FormationTargeter = /** @class */ (function () {
    function FormationTargeter() {
    }
    /**
     * Returns an array of CombatEntities that exist in the targetFormation that is the target of the provided
     * ability and target position.
     * @param ability The ability used to target entities in the formation.
     * @param targetPosition The center position used to position the center of the ability in a formation.
     * @param targetFormation The formation targeted by the ability.
     */
    FormationTargeter.getTargets = function (ability, targetPosition, targetFormation) {
        var targets = [];
        if (ability.isPositionStatic) {
            targetFormation.positions.forEach(function (row, xIndex) {
                row.forEach(function (entity, yIndex) {
                    if (ability.targets.some(function (val) { return val === yIndex * formation_model_1.FormationConstants.maxColumns + xIndex + 1; })) {
                        targets.push(entity);
                    }
                });
            });
        }
        else if (this.isTargetBlocked(ability, targetPosition, targetFormation))
            return targets;
        var translatedTargets = this.translate(ability.centerOfTargets, ability.targets, targetPosition);
        for (var i = 0; i < targetFormation.positions.length; i++) {
            for (var j = 0; j < targetFormation.positions[i].length; j++) {
                if (translatedTargets.some(function (tt) { return tt === i * formation_model_1.FormationConstants.maxRows + j + 1; })) {
                    if (targetFormation.positions[i][j] != null)
                        targets.push(targetFormation.positions[i][j]);
                }
            }
        }
        return targets;
    };
    /**
     * Returns true if a given target position is blocked by a CombatEntity for an Ability in a given target Formation.
     *
     * Always returns false if the Ability cannot be blocked by a CombatEntity.
     * @param ability The Ability to check if blocked.
     * @param targetPosition The position to check in the Formation to see if it is blocked.
     * @param targetFormation The formation used to see if any other CombatEntities are blocking the target position.
     */
    FormationTargeter.isTargetBlocked = function (ability, targetPosition, targetFormation) {
        if (!ability.canTargetBeBlocked)
            return false;
        var column = this.getColumn(targetPosition);
        if (column !== 1) {
            var row = this.getRow(targetPosition);
            var entityRow = targetFormation.positions[row - 1];
            for (var i = 0; i < column - 1; i++) {
                if (entityRow[i] != null)
                    return true;
            }
        }
        return false;
    };
    /**
     * Translates a given targets array from one point to another, removing any positions
     * that logically don't belong there.
     * @param initialCenter The initial center of position for the targets to translate.
     * @param targets The target positions to translate to another position.
     * @param newCenter The new center of position to translate the targets to.
     */
    FormationTargeter.translate = function (initialCenter, targets, newCenter) {
        var _this = this;
        var newTargets = targets.slice();
        var change = initialCenter - newCenter;
        var rowDifference = this.getRowDifference(initialCenter, newCenter);
        var columnDifference = this.getColumnDifference(initialCenter, newCenter);
        // Remove positions depending on their change in rows
        var removePositions = [];
        newTargets.forEach(function (val, index) {
            var valRowDiff = _this.getRowDifference(val, val - change);
            var rowPosition = _this.getRow(val) - valRowDiff;
            // Remove any positions that go out of bounds of row size
            if (rowPosition < 1 || rowPosition > formation_model_1.FormationConstants.maxRows)
                removePositions.push(index);
            // If there is a misalignment in the change in rows of this position and the center, remove this position
            else if (rowDifference != valRowDiff)
                removePositions.push(index);
        });
        removePositions.reverse().forEach(function (i) { return newTargets.splice(i, 1); });
        // Remove positions depending on their change in columns
        removePositions = [];
        newTargets.forEach(function (val, index) {
            var valColDiff = _this.getColumnDifference(val, val - change);
            var columnPosition = _this.getColumn(val) - valColDiff;
            // Remove any positions that go out of bounds of column size
            if (columnPosition < 1 || columnPosition > formation_model_1.FormationConstants.maxColumns)
                removePositions.push(index);
            // If there is a misalignment in the chance in columns of this position and the center, remove this position
            else if (columnDifference != valColDiff)
                removePositions.push(index);
        });
        removePositions.reverse().forEach(function (i) { return newTargets.splice(i, 1); });
        newTargets.forEach(function (val, index) {
            newTargets[index] = val - change;
        });
        return newTargets;
    };
    /**
     * Given the starting center position and the new center position, gets the change
     * in rows that would occur if the center position was translated to the new
     * position.
     * @param initialCenter The number representing the position of the initial center of targets
     * position for an ability.
     * @param newCenter The number representing the position to translate the center of targets position to.
     */
    FormationTargeter.getRowDifference = function (initialCenter, newCenter) {
        var initialRow = this.getRow(initialCenter);
        var newRow = this.getRow(newCenter);
        return initialRow - newRow;
    };
    /**
     * Gets the row number that a given position exists in.
     * @param position The position to get the row number for.
     */
    FormationTargeter.getRow = function (position) {
        // - 1 to keep every third number from being a new row
        return Math.floor((position - 1) / formation_model_1.FormationConstants.maxColumns + 1);
    };
    /**
     * Given the starting center position and the new center position, gets the change
     * in columns that would occur if the center position was translated to the new
     * position.
     * @param initialCenter The number representing the position of the initial center of targets
     * position for an ability.
     * @param newCenter The number representing the position to translate the center of targets position to.
     */
    FormationTargeter.getColumnDifference = function (initialCenter, newCenter) {
        var initalColumn = this.getColumn(initialCenter);
        var newColumn = this.getColumn(newCenter);
        return initalColumn - newColumn;
    };
    /**
     * Gets the column number that a given position exists in.
     * @param position The position to get the row number for.
     */
    FormationTargeter.getColumn = function (position) {
        return (position - 1) % formation_model_1.FormationConstants.maxRows + 1;
    };
    return FormationTargeter;
}());
exports.FormationTargeter = FormationTargeter;
//# sourceMappingURL=formation-targeter.static.js.map