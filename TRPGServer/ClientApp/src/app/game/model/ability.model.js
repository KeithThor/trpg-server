"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var damage_types_model_1 = require("./damage-types.model");
var damage_per_stat_model_1 = require("./damage-per-stat.model");
var character_stats_model_1 = require("./character-stats.model");
var Ability = /** @class */ (function () {
    function Ability() {
        this.name = "";
        this.description = "";
        this.targets = [];
        this.damage = new damage_types_model_1.DamageTypes();
        this.damagePerStat = new damage_per_stat_model_1.DamagePerStat();
        this.healPerStat = new character_stats_model_1.CharacterStats();
        this.percentDamage = new damage_types_model_1.DamageTypes();
        this.appliedStatusEffects = [];
        this.selfAppliedStatusEffects = [];
        this.actionPointCost = 0;
        this.healthCost = 0;
        this.healthPercentCost = 0;
        this.manaCost = 0;
        this.manaPercentCost = 0;
        this.heal = 0;
        this.percentHeal = 0;
    }
    return Ability;
}());
exports.Ability = Ability;
//# sourceMappingURL=ability.model.js.map