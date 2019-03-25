"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var CharacterStats = /** @class */ (function () {
    function CharacterStats() {
    }
    CharacterStats.prototype.getStat = function (statName) {
        switch (statName) {
            case StatNames.strength:
                return this.strength;
            case StatNames.dexterity:
                return this.dexterity;
            case StatNames.agility:
                return this.agility;
            case StatNames.intelligence:
                return this.intelligence;
            case StatNames.constitution:
                return this.constitution;
            default:
                return null;
        }
    };
    return CharacterStats;
}());
exports.CharacterStats = CharacterStats;
var StatNames = /** @class */ (function () {
    function StatNames() {
    }
    StatNames.strength = "strength";
    StatNames.dexterity = "dexterity";
    StatNames.agility = "agility";
    StatNames.intelligence = "intelligence";
    StatNames.constitution = "constitution";
    return StatNames;
}());
exports.StatNames = StatNames;
//# sourceMappingURL=character-stats.model.js.map