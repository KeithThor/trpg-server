using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;

namespace TRPGGame.Services
{
    /// <summary>
    /// A static class responsible for calculating damage and healing for combat-related
    /// purposes.
    /// </summary>
    public static class DamageCalculator
    {
        /// <summary>
        /// Gets the amount of damage dealt by a StatusEffect modified by the stats of a CombatEntity who will be using
        /// it to deal damage.
        /// </summary>
        /// <param name="attacker">The CombatEntity whose stats will be used to calculate the damage of the StatusEffect.</param>
        /// <param name="statusEffect">The StatusEffect to calculate damage from.</param>
        /// <param name="isCrit">If true, will apply the crit damage bonus to the damage amount.</param>
        /// <returns>A DamageTypes object containing the amount of damage of each type of damage the StatusEffect will deal
        /// per turn.</returns>
        public static DamageTypes GetDamage(CombatEntity attacker, StatusEffect statusEffect, bool isCrit = false)
        {
            var result = new DamageTypes();

            result += GetDamagePerStat(statusEffect.DamagePerStatPerTurn, attacker.Stats);
            result += statusEffect.DamagePerTurn;
            result = result * (attacker.SecondaryStats.DamagePercentage + 100) / 100;
            result = ApplyBonusDamage(result, attacker.SecondaryStats.Damage);

            if (isCrit)
            {
                int totalCritDamage = attacker.SecondaryStats.CritDamage * (attacker.SecondaryStats.CritDamagePercentage + 100) / 100;
                result = result * (totalCritDamage * 100) / 100;
            }

            return result;
        }

        /// <summary>
        /// Gets the amount of damage dealt by an ability if a target enemy has no defensive stats.
        /// </summary>
        /// <param name="attacker">The CombatEntity performing the ability.</param>
        /// <param name="ability">The ability to calculate damage for.</param>
        /// <param name="isCrit">If true, will include crit damage in the calculations.</param>
        /// <returns></returns>
        public static DamageTypes GetDamage(CombatEntity attacker, Ability ability, bool isCrit = false)
        {
            var result = new DamageTypes();

            result += GetDamagePerStat(ability.DamagePerStat, attacker.Stats);
            result += ability.Damage;
            result += result * (attacker.SecondaryStats.DamagePercentage + 100) / 100;
            result += ApplyBonusDamage(result, attacker.SecondaryStats.Damage);

            if (isCrit)
            {
                int totalCritDamage = attacker.SecondaryStats.CritDamage * (attacker.SecondaryStats.CritDamagePercentage + 100) / 100;
                result = result * (totalCritDamage * 100) / 100;
            }
            return result;
        }

        /// <summary>
        /// Gets the amount of damage for each damage type for an ability that does percentage of max
        /// health damage.
        /// </summary>
        /// <param name="defender">The target who will receive damage from the ability.</param>
        /// <param name="ability">The ability doing damage.</param>
        /// <returns></returns>
        public static DamageTypes GetPercentageDamage(CombatEntity defender, Ability ability)
        {
            return ability.PercentDamage * defender.Resources.MaxHealth / 100;
        }

        /// <summary>
        /// Gets the amount of damage for each damage type for an ability that does percentage of max
        /// health damage as an integer.
        /// </summary>
        /// <param name="defender">The target who will receive damage from the ability.</param>
        /// <param name="ability">The ability doing damage.</param>
        /// <returns></returns>
        public static int GetPercentageDamageAsInt(CombatEntity defender, Ability ability)
        {
            return GetDamageTypesAsInt(GetPercentageDamage(defender, ability));
        }

        /// <summary>
        /// Gets the total amount of damage an ability will do to a target.
        /// </summary>
        /// <param name="attacker">The CombatEntity performing the ability.</param>
        /// <param name="defender">The CombatEntity who will be damaged by the ability.</param>
        /// <param name="ability">The ability to calculate damage from.</param>
        /// <param name="isCrit">If true, will include critical damage in the calculations.</param>
        /// <returns></returns>
        public static DamageTypes GetTotalDamage(CombatEntity attacker, CombatEntity defender, Ability ability, bool isCrit = false)
        {
            var damage = GetDamage(attacker, ability, isCrit);
            damage += GetPercentageDamage(defender, ability);

            return GetTotalDamage(damage, defender);
        }

        /// <summary>
        /// Gets the total amount of damage an ability will do to a target as an integer.
        /// </summary>
        /// <param name="attacker">The CombatEntity performing the ability.</param>
        /// <param name="defender">The CombatEntity who will be damaged by the ability.</param>
        /// <param name="ability">The ability to calculate damage from.</param>
        /// <param name="isCrit">If true, will include critical damage in the calculations.</param>
        /// <returns></returns>
        public static int GetTotalDamageAsInt(CombatEntity attacker, CombatEntity defender, Ability ability, bool isCrit = false)
        {
            return GetDamageTypesAsInt(GetTotalDamage(attacker, defender, ability, isCrit));
        }

        /// <summary>
        /// Gets the total amount of damage done to a target.
        /// </summary>
        /// <param name="damage">The previously calculated amount of damage to deal to a target.</param>
        /// <param name="defender">The defending CombatEntity who will receive damage.</param>
        /// <returns></returns>
        public static DamageTypes GetTotalDamage(DamageTypes damage, CombatEntity defender)
        {
            var result = damage.Copy();
            var totalArmor = defender.SecondaryStats.Armor * (defender.SecondaryStats.ArmorPercentage + 100) / 100;
            result -= totalArmor;
            result = result * 100 / (defender.SecondaryStats.Resistances + 100);

            return result;
        }

        /// <summary>
        /// Gets the total amount of damage in a DamageTypes object.
        /// </summary>
        /// <param name="damage">The DamageTypes object to total up.</param>
        /// <returns></returns>
        public static int GetDamageTypesAsInt(DamageTypes damage)
        {
            return damage.Blunt +
                   damage.Sharp +
                   damage.Fire +
                   damage.Frost +
                   damage.Lightning +
                   damage.Earth +
                   damage.Holy +
                   damage.Shadow;
        }

        /// <summary>
        /// Gets the total amount of damage per stats in a DamagePerStats object.
        /// </summary>
        /// <param name="damagePerStat">The object to get the total damage from.</param>
        /// <returns></returns>
        public static int GetDamagePerStatAsInt(DamagePerStat damagePerStat)
        {
            return GetDamageTypesAsInt(damagePerStat.Strength)
                   + GetDamageTypesAsInt(damagePerStat.Dexterity)
                   + GetDamageTypesAsInt(damagePerStat.Agility)
                   + GetDamageTypesAsInt(damagePerStat.Intelligence)
                   + GetDamageTypesAsInt(damagePerStat.Constitution);
        }

        /// <summary>
        /// Applies bonus damage to a DamageTypes object if the initial DamageType object does greater than
        /// or less than 0 damage.
        /// </summary>
        /// <param name="damage">The initial amount of damage.</param>
        /// <param name="bonus">The bonus damage to apply.</param>
        /// <returns>A damageTypes object containing modified damage if a given damage type is not 0.</returns>
        private static DamageTypes ApplyBonusDamage(DamageTypes damage, DamageTypes bonus)
        {
            var result = new DamageTypes();

            if (damage.Blunt != 0) result.Blunt = damage.Blunt + bonus.Blunt;
            if (damage.Sharp != 0) result.Sharp = damage.Sharp + bonus.Sharp;
            if (damage.Fire != 0) result.Fire = damage.Fire + bonus.Fire;
            if (damage.Frost != 0) result.Frost = damage.Frost + bonus.Frost;
            if (damage.Lightning != 0) result.Lightning = damage.Lightning + bonus.Lightning;
            if (damage.Earth != 0) result.Earth = damage.Earth + bonus.Earth;
            if (damage.Holy != 0) result.Holy = damage.Holy + bonus.Holy;
            if (damage.Shadow != 0) result.Shadow = damage.Shadow + bonus.Shadow;

            return result;
        }

        /// <summary>
        /// Returns a DamageTypes object that contains the amount of each type of damage is deal given a DamagePerStat amount
        /// and a character's stats.
        /// </summary>
        /// <param name="damagePerStat">The amount of damage dealt per point of each type of stat.</param>
        /// <param name="stats">The amount of each stat type to calculate with.</param>
        /// <returns>A DamageTypes object containing the amount of damage of each type.</returns>
        public static DamageTypes GetDamagePerStat(DamagePerStat damagePerStat, CharacterStats stats)
        {
            var result = new DamageTypes();

            result += damagePerStat.Strength * stats.Strength;
            result += damagePerStat.Dexterity * stats.Dexterity;
            result += damagePerStat.Agility * stats.Agility;
            result += damagePerStat.Intelligence * stats.Intelligence;
            result += damagePerStat.Constitution * stats.Constitution;

            return result;
        }

        /// <summary>
        /// Gets the heal per turn amount for a StatusEffect.
        /// </summary>
        /// <param name="healer">The CombatEntity applying the healing StatusEffect.</param>
        /// <param name="statusEffect">The StatusEffect to calculate healing with.</param>
        /// <param name="isCrit">If true, will include critical damage in the calculations.</param>
        /// <returns>An integer amount representing how much health is healed per turn from this StatusEffect.</returns>
        public static int GetHeal(CombatEntity healer, StatusEffect statusEffect, bool isCrit = false)
        {
            var perStatHeal = statusEffect.HealPerStatPerTurn * healer.Stats;
            int healAmount = perStatHeal.Strength + 
                perStatHeal.Dexterity + 
                perStatHeal.Agility + 
                perStatHeal.Intelligence + 
                perStatHeal.Constitution;

            healAmount = healAmount * (healer.SecondaryStats.HealPercentageBonus + 100) / 100;
            healAmount += healer.SecondaryStats.HealBonus;

            if (isCrit)
            {
                int totalCritDamage = healer.SecondaryStats.CritDamage * (healer.SecondaryStats.CritDamagePercentage + 100) / 100;
                healAmount = healAmount * (totalCritDamage * 100) / 100;
            }
            
            return healAmount;
        }

        /// <summary>
        /// Gets the heal amount for an Ability.
        /// </summary>
        /// <param name="healer">The CombatEntity using the healing Ability.</param>
        /// <param name="ability">The Ability to calculate healing with.</param>
        /// <param name="isCrit">If true, will include critical damage in the calculations.</param>
        /// <returns>An integer amount representing how much health is healed from this Ability.</returns>
        public static int GetHeal(CombatEntity healer, Ability ability, bool isCrit = false)
        {
            var perStatHeal = ability.HealPerStat * healer.Stats;
            int healAmount = perStatHeal.Strength +
                perStatHeal.Dexterity +
                perStatHeal.Agility +
                perStatHeal.Intelligence +
                perStatHeal.Constitution;

            healAmount = healAmount * (healer.SecondaryStats.HealPercentageBonus + 100) / 100;
            healAmount += healer.SecondaryStats.HealBonus;

            if (isCrit)
            {
                int totalCritDamage = healer.SecondaryStats.CritDamage * (healer.SecondaryStats.CritDamagePercentage + 100) / 100;
                healAmount = healAmount * (totalCritDamage * 100) / 100;
            }

            return healAmount;
        }

        /// <summary>
        /// Returns the amount of health healed for an ability or status effect that heals a percent of max health.
        /// </summary>
        /// <param name="maxHealth">The amount to get a percentage of.</param>
        /// <param name="healPercent">The percentage of max health to calculate.</param>
        /// <returns></returns>
        public static int GetPercentageHeal(int maxHealth, int healPercent)
        {
            return maxHealth * healPercent / 100;
        }
    }
}
