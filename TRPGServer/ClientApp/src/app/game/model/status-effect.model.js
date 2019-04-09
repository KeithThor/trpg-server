"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var damage_types_model_1 = require("./damage-types.model");
var damage_per_stat_model_1 = require("./damage-per-stat.model");
var character_stats_model_1 = require("./character-stats.model");
var secondary_stat_model_1 = require("./secondary-stat.model");
var StatusEffect = /** @class */ (function () {
    function StatusEffect() {
        this.name = "";
        this.description = "";
        this.iconUris = [];
        this.duration = 1;
        this.stackSize = 1;
        this.damagePerTurn = new damage_types_model_1.DamageTypes();
        this.damagePerStatPerTurn = new damage_per_stat_model_1.DamagePerStat();
        this.healPerTurn = 0;
        this.percentHealPerTurn = 0;
        this.healPerStatPerTurn = new character_stats_model_1.CharacterStats();
        this.modifiedStats = new character_stats_model_1.CharacterStats();
        this.modifiedStatPercentages = new character_stats_model_1.CharacterStats();
        this.modifiedSecondaryStats = new secondary_stat_model_1.SecondaryStat();
    }
    return StatusEffect;
}());
exports.StatusEffect = StatusEffect;
//# sourceMappingURL=status-effect.model.js.map