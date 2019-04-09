using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;

namespace TRPGGame.Services
{
    /// <summary>
    /// A static class that performs calculations to translate changes from primary stats to SecondaryStats.
    /// </summary>
    public static class StatsCalculator
    {
        /// <summary>
        /// Returns a SecondaryStats object that contains CharacterStats converted into SecondaryStat points.
        /// </summary>
        /// <param name="stats">The primary stats to convert into SecondaryStats.</param>
        /// <returns></returns>
        public static SecondaryStat GetSecondaryStats(CharacterStats stats)
        {
            return new SecondaryStat()
            {
                ActionPointCostReduction = 0,
                ActionPointCostReductionPercentage = 0,
                Armor = new DamageTypes
                {
                    Blunt = stats.Constitution / 4,
                    Sharp = stats.Constitution / 4,
                    Fire = stats.Constitution / 4,
                    Frost = stats.Constitution / 4,
                    Lightning = stats.Constitution / 4,
                    Earth = stats.Constitution / 4,
                    Holy = stats.Constitution / 4,
                    Shadow = stats.Constitution / 4
                },
                ArmorPercentage = new DamageTypes(),
                BonusActionPoints = stats.Agility / 6,
                BonusActionPointsPercentage = 0,
                CritChance = stats.Agility / 6,
                CritChancePercentage = 0,
                CritDamage = stats.Dexterity / 3,
                CritDamagePercentage = 0,
                Damage = new DamageTypes
                {
                    Blunt = stats.Strength / 2,
                    Sharp = stats.Dexterity / 2,
                    Fire = stats.Intelligence / 3,
                    Frost = stats.Intelligence / 3,
                    Lightning = stats.Intelligence / 3,
                    Earth = stats.Intelligence / 3,
                    Holy = stats.Intelligence / 3,
                    Shadow = stats.Intelligence / 3
                },
                DamagePercentage = new DamageTypes(),
                HealBonus = stats.Intelligence / 3,
                HealPercentageBonus = 0,
                ManaCostReduction = 0,
                ManaCostReductionPercentage = 0,
                Resistances = new DamageTypes()
            };
        }

        /// <summary>
        /// Returns a new amount of health given the stats to calculate for.
        /// </summary>
        /// <param name="stats"></param>
        /// <returns></returns>
        public static int GetHealth(CharacterStats stats) => stats.Constitution * 4;

        /// <summary>
        /// Returns a new amount of mana given the stats to calculate for.
        /// </summary>
        /// <param name="stats"></param>
        /// <returns></returns>
        public static int GetMana(CharacterStats stats) => stats.Intelligence * 3;
    }
}
