"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var character_stats_model_1 = require("./character-stats.model");
var character_model_1 = require("./character.model");
var secondary_stat_model_1 = require("./secondary-stat.model");
var CombatEntity = /** @class */ (function () {
    function CombatEntity() {
        this.name = "";
        this.iconUris = new character_model_1.CharacterIconSet();
        this.stats = new character_stats_model_1.CharacterStats();
        this.unmodifiedStats = new character_stats_model_1.CharacterStats();
        this.growthPoints = new character_stats_model_1.CharacterStats();
        this.secondaryStats = new secondary_stat_model_1.SecondaryStat();
        this.abilities = [];
        this.statusEffects = [];
        this.equippedItems = [];
    }
    return CombatEntity;
}());
exports.CombatEntity = CombatEntity;
//# sourceMappingURL=combat-entity.model.js.map