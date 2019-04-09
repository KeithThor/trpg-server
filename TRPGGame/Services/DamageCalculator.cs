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
        /// <returns>A DamageTypes object containing the amount of damage of each type of damage the StatusEffect will deal
        /// per turn.</returns>
        public static DamageTypes GetDamage(CombatEntity attacker, StatusEffect statusEffect)
        {
            var result = new DamageTypes();

            result += GetDamagePerStat(statusEffect.DamagePerStatPerTurn, attacker.Stats);
            result += statusEffect.DamagePerTurn;
            result = result * (attacker.SecondaryStats.DamagePercentage + 100) / 100;
            result = ApplyBonusDamage(result, attacker.SecondaryStats.Damage);

            return result;
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
        /// <returns>An integer amount representing how much health is healed per turn from this StatusEffect.</returns>
        public static int GetHeal(CombatEntity healer, StatusEffect statusEffect)
        {
            var perStatHeal = statusEffect.HealPerStatPerTurn * healer.Stats;
            int healAmount = perStatHeal.Strength + 
                perStatHeal.Dexterity + 
                perStatHeal.Agility + 
                perStatHeal.Intelligence + 
                perStatHeal.Constitution;

            healAmount = healAmount * (healer.SecondaryStats.HealPercentageBonus + 100) / 100;
            healAmount += healer.SecondaryStats.HealBonus;

            return healAmount;
        }
    }
}
