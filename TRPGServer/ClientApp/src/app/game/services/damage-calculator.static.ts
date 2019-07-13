import { CombatEntity } from "../model/combat-entity.model";
import { Ability } from "../model/ability.model";
import { DamageTypes } from "../model/damage-types.model";
import { DamagePerStat } from "../model/damage-per-stat.model";
import { CharacterStats } from "../model/character-stats.model";

/** Static class responsible for calculating damage for combat. */
export class DamageCalculator {
  /**
   * Gets the amount of damage an ability will do without accounting for a target character's
   * armor and resistances.
   * @param owner The CombatEntity that owns the ability to calculate damage for.
   * @param ability The Ability to calculate damage for.
   */
  public static getAbilityDamage(owner: CombatEntity, ability: Ability): DamageTypes {
    let damage = this.addDamage(ability.damage,
      this.getDamageFromStats(ability.damagePerStat, owner.stats));

    let bonusDamage = this.getBonusDamage(owner, damage);
    damage = this.addDamage(damage, bonusDamage);

    return this.getPercentDamage(damage, owner.secondaryStats.damagePercentage);
  }

  /**
   * Returns a DamageTypes object containing the amount of bonus damage for each damage type
   * if an ability does not do 0 damage with that type.
   * @param owner The CombatEntity that the ability belongs to.
   * @param ability The ability to calculate bonus damage for.
   */
  private static getBonusDamage(owner: CombatEntity, damage: DamageTypes): DamageTypes {
    let bonusDamage = owner.secondaryStats.damage;
    let result = new DamageTypes();
    if (damage.blunt != null && damage.blunt != 0) result.blunt = bonusDamage.blunt;
    if (damage.sharp != null && damage.sharp != 0) result.sharp = bonusDamage.sharp;
    if (damage.fire != null && damage.fire != 0) result.fire = bonusDamage.fire;
    if (damage.frost != null && damage.frost != 0) result.frost = bonusDamage.frost;
    if (damage.lightning != null && damage.lightning != 0) result.lightning = bonusDamage.lightning;
    if (damage.earth != null && damage.earth != 0) result.earth = bonusDamage.earth;
    if (damage.holy != null && damage.holy != 0) result.holy = bonusDamage.holy;
    if (damage.shadow != null && damage.shadow != 0) result.shadow = bonusDamage.shadow;
    return result;
  }

  /**
   * Gets the amount of healing an ability does after applying all bonuses.
   * @param owner The CombatEntity who owns the ability to get healing for.
   * @param ability The ability to calculate healing for.
   */
  public static getAbilityHeal(owner: CombatEntity, ability: Ability): number {
    if (!this.abilityDoesHeal(ability)) return 0;
    let flatHealing = owner.secondaryStats.healBonus + ability.heal + this.getHealFromStats(ability.healPerStat, owner.stats);
    return Math.floor(flatHealing * (100 + owner.secondaryStats.healPercentageBonus) / 100);
  }

  /**
   * Sums up all the damage for every damage type.
   * @param damageTypes A damage type object containing the values for each damage type.
   */
  public static getFlatDamage(damageTypes: DamageTypes): number {
    let damage = 0;

    DamageTypes.asArray(damageTypes).forEach(val => {
      damage += val;
    });

    return damage;
  }

  /**
   * Returns true if a damage type object does no positive or negative damage.
   * @param damageTypes The damage type object to check.
   */
  private static doesNoDamage(damageTypes: DamageTypes): boolean {
    if (damageTypes == null) return true;
    if (damageTypes.blunt !== 0) return false;
    if (damageTypes.sharp !== 0) return false;
    if (damageTypes.fire !== 0) return false;
    if (damageTypes.frost !== 0) return false;
    if (damageTypes.lightning !== 0) return false;
    if (damageTypes.earth !== 0) return false;
    if (damageTypes.holy !== 0) return false;
    if (damageTypes.shadow !== 0) return false;
    return true;
  }

  /**
   * Returns true if an ability does more than or less than 0 points of healing.
   * @param ability
   */
  private static abilityDoesHeal(ability: Ability): boolean {
    if (ability.heal != null && ability.heal != 0) return true;
    if ((ability.heal == null || ability.heal === 0) && ability.healPerStat == null) return false;

    let healPerStat = CharacterStats.asArray(ability.healPerStat);
    for (var i = 0; i < healPerStat.length; i++) {
      if (healPerStat[i] != null && healPerStat[i] !== 0) return true;
    }
    return false;
  }

  /**
   * Calculates and returns the amount of damage of each type given a DamagePerStat object and
   * a CharacterStats object.
   * @param damagePerStat The amount of each type of damage given for each point of a given stat.
   * @param stats The amount of each stat to calculate with.
   */
  public static getDamageFromStats(damagePerStat: DamagePerStat, stats: CharacterStats): DamageTypes {
    if (damagePerStat == null || stats == null) return new DamageTypes();

    let damage = new DamageTypes();
    let perStatArray = DamagePerStat.asArray(damagePerStat);
    let statsArray = CharacterStats.asArray(stats);
    for (let i = 0; i < perStatArray.length; i++) {
      if (perStatArray[i] != null) {
        let dmgArr: number[] = [];
        let statDamage = DamageTypes.asArray(perStatArray[i]);
        statDamage.forEach(val => {
          dmgArr.push(val * statsArray[i]);
        });
        damage = this.addDamage(damage, DamageTypes.fromArray(dmgArr));
      }
    }
    return damage;
  }

  /**
   * Calculates and returns the amount of total amount of healing done an amount of healing per stat and a stat amount.
   * @param healPerStat The amount of healing given per point of stat.
   * @param stats The amount of each stat to calculate the healing with.
   */
  public static getHealFromStats(healPerStat: CharacterStats, stats: CharacterStats): number {
    let result = 0;
    let healPerStatArr = CharacterStats.asArray(healPerStat);
    let statsArr = CharacterStats.asArray(stats);

    for (var i = 0; i < healPerStatArr.length; i++) {
      result += healPerStatArr[i] * statsArr[i];
    }

    return result;
  }

  /**
   * Adds two or more DamageTypes objects together and returns a new DamageType object containing
   * the sum of all.
   * @param first The first DamageType object.
   * @param second The second DamageType object.
   * @param rest One or more additional DamageType objects to add.
   */
  public static addDamage(first: DamageTypes, second: DamageTypes, ...rest: DamageTypes[]): DamageTypes {
    if (first == null && second == null) return new DamageTypes();
    if (first == null && second != null) return second;
    if (first != null && second == null) return first;

    let arrResult: number[] = [];
    let firstAsArray = DamageTypes.asArray(first);
    let secondAsArray = DamageTypes.asArray(second);
    let restAsArray: number[][] = [];

    if (rest != null) {
      rest.forEach(damageType => {
        restAsArray.push(DamageTypes.asArray(damageType));
      });
    }

    for (var i = 0; i < firstAsArray.length; i++) {
      let damage = 0;

      damage = firstAsArray[i] + secondAsArray[i];
      restAsArray.forEach(damageType => {
        damage += damageType[i];
      });

      arrResult.push(damage);
    }

    return DamageTypes.fromArray(arrResult);
  }

  /**
   * Returns a new DamageTypes object that is a percentage of the initial object.
   * @param initial The DamageTypes object to return a percentage of.
   * @param percentage A DamageTypes object containing the percentage of each value to calculate. 0 is 100%, -15 is 85%, 15 is 115%.
   */
  public static getPercentDamage(initial: DamageTypes, percentage: DamageTypes): DamageTypes {
    if (initial == null) return new DamageTypes();
    if (initial != null && percentage == null) return initial;

    let arrResult: number[] = [];
    let initialAsArray = DamageTypes.asArray(initial);
    let percentageAsArray = DamageTypes.asArray(percentage);

    for (var i = 0; i < initialAsArray.length; i++) {
      arrResult.push(Math.floor(initialAsArray[i] * (100 + percentageAsArray[i]) / 100));
    }

    return DamageTypes.fromArray(arrResult);
  }
}
