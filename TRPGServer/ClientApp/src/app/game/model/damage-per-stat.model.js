"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var DamagePerStat = /** @class */ (function () {
    function DamagePerStat() {
    }
    DamagePerStat.asArray = function (damagePerStat) {
        return [
            damagePerStat.strength,
            damagePerStat.dexterity,
            damagePerStat.agility,
            damagePerStat.intelligence,
            damagePerStat.constitution
        ];
    };
    return DamagePerStat;
}());
exports.DamagePerStat = DamagePerStat;
//# sourceMappingURL=damage-per-stat.model.js.map