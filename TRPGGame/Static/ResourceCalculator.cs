using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;

namespace TRPGGame.Services
{
    /// <summary>
    /// Static class responsible for calculating the total cost of resources for Abilities used by a CombatEntity.
    /// </summary>
    public static class ResourceCalculator
    {
        /// <summary>
        /// Gets the total health cost of an ability modified by an entity's current stats.
        /// </summary>
        /// <param name="entity">The CombatEntity using the ability.</param>
        /// <param name="ability">The Ability being used.</param>
        /// <returns></returns>
        public static int GetTotalHealthCost(CombatEntity entity, Ability ability)
        {
            int healthCost = entity.Resources.MaxHealth * ability.HealthPercentCost / 100;
            healthCost += ability.HealthCost;

            return healthCost;
        }

        /// <summary>
        /// Gets the total mana cost of an ability modified by an entity's current stats.
        /// </summary>
        /// <param name="entity">The CombatEntity using the ability.</param>
        /// <param name="ability">The Ability being used.</param>
        /// <returns></returns>
        public static int GetTotalManaCost(CombatEntity entity, Ability ability)
        {
            int manaCost = entity.Resources.MaxMana * ability.ManaPercentCost / 100;
            manaCost += ability.ManaCost;

            manaCost -= entity.SecondaryStats.ManaCostReduction;
            manaCost = manaCost * (100 - entity.SecondaryStats.ManaCostReductionPercentage) / 100;

            return manaCost;
        }

        /// <summary>
        /// Gets the total action point cost of an ability modified by an entity's current stats.
        /// </summary>
        /// <param name="entity">The CombatEntity using the ability.</param>
        /// <param name="ability">The Ability being used.</param>
        /// <returns></returns>
        public static int GetTotalActionPointCost(CombatEntity entity, Ability ability)
        {
            int actionPointCost = ability.ActionPointCost;

            actionPointCost -= entity.SecondaryStats.ActionPointCostReduction;
            actionPointCost = actionPointCost * (100 - entity.SecondaryStats.ActionPointCostReductionPercentage) / 100;

            return actionPointCost;
        }
    }
}
