"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var character_stats_model_1 = require("./character-stats.model");
var secondary_stat_model_1 = require("./secondary-stat.model");
var ClassTemplate = /** @class */ (function () {
    function ClassTemplate() {
        this.name = "";
        this.description = "";
        this.stats = new character_stats_model_1.CharacterStats();
        this.secondaryStats = new secondary_stat_model_1.SecondaryStat();
        this.abilities = [];
        this.equippedItems = [];
    }
    return ClassTemplate;
}());
exports.ClassTemplate = ClassTemplate;
//# sourceMappingURL=class-template.model.js.map