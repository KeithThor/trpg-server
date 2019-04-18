"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var damage_types_model_1 = require("../model/damage-types.model");
var damage_per_stat_model_1 = require("../model/damage-per-stat.model");
var character_stats_model_1 = require("../model/character-stats.model");
/** Static class responsible for calculating damage for combat. */
var DamageCalculator = /** @class */ (function () {
    function DamageCalculator() {
    }
    /**
     * Gets the amount of damage an ability will do without accounting for a target character's
     * armor and resistances.
     * @param owner The CombatEntity that owns the ability to calculate damage for.
     * @param ability The Ability to calculate damage for.
     */
    DamageCalculator.getAbilityDamage = function (owner, ability) {
        if (this.doesNoDamage(ability.damage))
            return new damage_types_model_1.DamageTypes();
        var damageWithBonuses = this.addDamage(this.getBonusDamage(owner, ability), ability.damage, this.getDamageFromStats(ability.damagePerStat, owner.stats));
        return this.getPercentDamage(damageWithBonuses, owner.secondaryStats.damagePercentage);
    };
    /**
     * Returns a DamageTypes object containing the amount of bonus damage for each damage type
     * if an ability does not do 0 damage with that type.
     * @param owner The CombatEntity that the ability belongs to.
     * @param ability The ability to calculate bonus damage for.
     */
    DamageCalculator.getBonusDamage = function (owner, ability) {
        var bonusDamage = owner.secondaryStats.damage;
        var result = new damage_types_model_1.DamageTypes();
        if (ability.damage.blunt != null && ability.damage.blunt != 0)
            result.blunt = bonusDamage.blunt;
        if (ability.damage.sharp != null && ability.damage.sharp != 0)
            result.sharp = bonusDamage.sharp;
        if (ability.damage.fire != null && ability.damage.fire != 0)
            result.fire = bonusDamage.fire;
        if (ability.damage.frost != null && ability.damage.frost != 0)
            result.frost = bonusDamage.frost;
        if (ability.damage.lightning != null && ability.damage.lightning != 0)
            result.lightning = bonusDamage.lightning;
        if (ability.damage.earth != null && ability.damage.earth != 0)
            result.earth = bonusDamage.earth;
        if (ability.damage.holy != null && ability.damage.holy != 0)
            result.holy = bonusDamage.holy;
        if (ability.damage.shadow != null && ability.damage.shadow != 0)
            result.shadow = bonusDamage.shadow;
        return result;
    };
    /**
     * Gets the amount of healing an ability does after applying all bonuses.
     * @param owner The CombatEntity who owns the ability to get healing for.
     * @param ability The ability to calculate healing for.
     */
    DamageCalculator.getAbilityHeal = function (owner, ability) {
        if (!this.abilityDoesHeal(ability))
            return 0;
        var flatHealing = owner.secondaryStats.healBonus + ability.heal + this.getHealFromStats(ability.healPerStat, owner.stats);
        return Math.floor(flatHealing * (100 + owner.secondaryStats.healPercentageBonus) / 100);
    };
    /**
     * Sums up all the damage for every damage type.
     * @param damageTypes A damage type object containing the values for each damage type.
     */
    DamageCalculator.getFlatDamage = function (damageTypes) {
        var damage = 0;
        damage_types_model_1.DamageTypes.asArray(damageTypes).forEach(function (val) {
            damage += val;
        });
        return damage;
    };
    /**
     * Returns true if a damage type object does no positive or negative damage.
     * @param damageTypes The damage type object to check.
     */
    DamageCalculator.doesNoDamage = function (damageTypes) {
        if (damageTypes == null)
            return true;
        if (damageTypes.blunt !== 0)
            return false;
        if (damageTypes.sharp !== 0)
            return false;
        if (damageTypes.fire !== 0)
            return false;
        if (damageTypes.frost !== 0)
            return false;
        if (damageTypes.lightning !== 0)
            return false;
        if (damageTypes.earth !== 0)
            return false;
        if (damageTypes.holy !== 0)
            return false;
        if (damageTypes.shadow !== 0)
            return false;
        return true;
    };
    /**
     * Returns true if an ability does more than or less than 0 points of healing.
     * @param ability
     */
    DamageCalculator.abilityDoesHeal = function (ability) {
        if (ability.heal != null && ability.heal != 0)
            return true;
        if ((ability.heal == null || ability.heal === 0) && ability.healPerStat == null)
            return false;
        var healPerStat = character_stats_model_1.CharacterStats.asArray(ability.healPerStat);
        for (var i = 0; i < healPerStat.length; i++) {
            if (healPerStat[i] != null && healPerStat[i] !== 0)
                return true;
        }
        return false;
    };
    /**
     * Calculates and returns the amount of damage of each type given a DamagePerStat object and
     * a CharacterStats object.
     * @param damagePerStat The amount of each type of damage given for each point of a given stat.
     * @param stats The amount of each stat to calculate with.
     */
    DamageCalculator.getDamageFromStats = function (damagePerStat, stats) {
        if (damagePerStat == null || stats == null)
            return new damage_types_model_1.DamageTypes();
        var damage = new damage_types_model_1.DamageTypes();
        var perStatArray = damage_per_stat_model_1.DamagePerStat.asArray(damagePerStat);
        var statsArray = character_stats_model_1.CharacterStats.asArray(stats);
        var _loop_1 = function () {
            if (perStatArray[i] != null) {
                var dmgArr_1 = [];
                var statDamage = damage_types_model_1.DamageTypes.asArray(perStatArray[i]);
                statDamage.forEach(function (val) {
                    dmgArr_1.push(val * statsArray[i]);
                });
                damage = this_1.addDamage(damage, damage_types_model_1.DamageTypes.fromArray(statDamage));
            }
        };
        var this_1 = this;
        for (var i = 0; i < perStatArray.length; i++) {
            _loop_1();
        }
        return damage;
    };
    /**
     * Calculates and returns the amount of total amount of healing done an amount of healing per stat and a stat amount.
     * @param healPerStat The amount of healing given per point of stat.
     * @param stats The amount of each stat to calculate the healing with.
     */
    DamageCalculator.getHealFromStats = function (healPerStat, stats) {
        var result = 0;
        var healPerStatArr = character_stats_model_1.CharacterStats.asArray(healPerStat);
        var statsArr = character_stats_model_1.CharacterStats.asArray(stats);
        for (var i = 0; i < healPerStatArr.length; i++) {
            result += healPerStatArr[i] * statsArr[i];
        }
        return result;
    };
    /**
     * Adds two or more DamageTypes objects together and returns a new DamageType object containing
     * the sum of all.
     * @param first The first DamageType object.
     * @param second The second DamageType object.
     * @param rest One or more additional DamageType objects to add.
     */
    DamageCalculator.addDamage = function (first, second) {
        var rest = [];
        for (var _i = 2; _i < arguments.length; _i++) {
            rest[_i - 2] = arguments[_i];
        }
        if (first == null && second == null)
            return new damage_types_model_1.DamageTypes();
        if (first == null && second != null)
            return second;
        if (first != null && second == null)
            return first;
        var arrResult = [];
        var firstAsArray = damage_types_model_1.DamageTypes.asArray(first);
        var secondAsArray = damage_types_model_1.DamageTypes.asArray(second);
        var restAsArray = [];
        if (rest != null) {
            rest.forEach(function (damageType) {
                restAsArray.push(damage_types_model_1.DamageTypes.asArray(damageType));
            });
        }
        var _loop_2 = function () {
            var damage = 0;
            damage = firstAsArray[i] + secondAsArray[i];
            restAsArray.forEach(function (damageType) {
                damage += damageType[i];
            });
            arrResult.push(damage);
        };
        for (var i = 0; i < firstAsArray.length; i++) {
            _loop_2();
        }
        return damage_types_model_1.DamageTypes.fromArray(arrResult);
    };
    /**
     * Returns a new DamageTypes object that is a percentage of the initial object.
     * @param initial The DamageTypes object to return a percentage of.
     * @param percentage A DamageTypes object containing the percentage of each value to calculate. 0 is 100%, -15 is 85%, 15 is 115%.
     */
    DamageCalculator.getPercentDamage = function (initial, percentage) {
        if (initial == null)
            return new damage_types_model_1.DamageTypes();
        if (initial != null && percentage == null)
            return initial;
        var arrResult = [];
        var initialAsArray = damage_types_model_1.DamageTypes.asArray(initial);
        var percentageAsArray = damage_types_model_1.DamageTypes.asArray(percentage);
        for (var i = 0; i < initialAsArray.length; i++) {
            arrResult.push(Math.floor(initialAsArray[i] * (100 + percentageAsArray[i]) / 100));
        }
        return damage_types_model_1.DamageTypes.fromArray(arrResult);
    };
    return DamageCalculator;
}());
exports.DamageCalculator = DamageCalculator;
//# sourceMappingURL=damage-calculator.static.js.map