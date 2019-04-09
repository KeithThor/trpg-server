"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var character_stats_model_1 = require("./character-stats.model");
var CharacterTemplate = /** @class */ (function () {
    function CharacterTemplate() {
        this.allocatedStats = new character_stats_model_1.CharacterStats();
        this.name = "";
    }
    return CharacterTemplate;
}());
exports.CharacterTemplate = CharacterTemplate;
var CharacterHair = /** @class */ (function () {
    function CharacterHair() {
    }
    return CharacterHair;
}());
exports.CharacterHair = CharacterHair;
var CharacterBase = /** @class */ (function () {
    function CharacterBase() {
        this.maxStats = new character_stats_model_1.CharacterStats();
        this.bonusStats = new character_stats_model_1.CharacterStats();
    }
    return CharacterBase;
}());
exports.CharacterBase = CharacterBase;
var CharacterIconSet = /** @class */ (function () {
    function CharacterIconSet() {
    }
    CharacterIconSet.asArray = function (iconSet) {
        var arr = [];
        if (iconSet.cloakIconUri != null)
            arr.push(iconSet.cloakIconUri);
        if (iconSet.baseIconUri != null)
            arr.push(iconSet.baseIconUri);
        if (iconSet.legsIconUri != null)
            arr.push(iconSet.legsIconUri);
        if (iconSet.bootsIconUri != null)
            arr.push(iconSet.bootsIconUri);
        if (iconSet.bodyIconUri != null)
            arr.push(iconSet.bodyIconUri);
        if (iconSet.glovesIconUri != null)
            arr.push(iconSet.glovesIconUri);
        if (iconSet.hairIconUri != null)
            arr.push(iconSet.hairIconUri);
        if (iconSet.headIconUri != null)
            arr.push(iconSet.headIconUri);
        if (iconSet.leftHandIconUri != null)
            arr.push(iconSet.leftHandIconUri);
        if (iconSet.rightHandIconUri != null)
            arr.push(iconSet.rightHandIconUri);
        if (iconSet.extraIconUri != null)
            arr.push(iconSet.extraIconUri);
        return arr;
    };
    return CharacterIconSet;
}());
exports.CharacterIconSet = CharacterIconSet;
var CreateCharacterData = /** @class */ (function () {
    function CreateCharacterData() {
        this.hairs = [];
        this.bases = [];
        this.classTemplates = [];
    }
    return CreateCharacterData;
}());
exports.CreateCharacterData = CreateCharacterData;
//# sourceMappingURL=character.model.js.map