using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;
using TRPGGame.Services;

namespace TRPGGame.Static
{
    /// <summary>
    /// Static class responsible for writing and returning error messages for a battle instance.
    /// </summary>
    public static class BattleErrorWriter
    {
        /// <summary>
        /// Returns a string indicating that the battle has not yet been initiated.
        /// </summary>
        public static string WriteNotInitiated()
        {
            return "The battle has not yet been initiated.";
        }

        /// <summary>
        /// Returns a string indicating that the player is not participating in the battle.
        /// </summary>
        public static string WriteNotParticipating()
        {
            return "You are not currently participating in this battle.";
        }

        /// <summary>
        /// Returns a string indicating that the acting CombatEntity does not exist in the player's
        /// formation.
        /// </summary>
        public static string WriteActorNotFound()
        {
            return "The character you've chosen to act does not exist in your formation.";
        }

        /// <summary>
        /// Returns a string indicating that it is not the player's turn.
        /// </summary>
        public static string WriteNotPlayersTurn()
        {
            return "You can only act on your turn.";
        }

        /// <summary>
        /// Returns a string indicating that the player has no more actions for this turn.
        /// </summary>
        public static string WriteNoMoreActions()
        {
            return "You have no more available actions for this turn.";
        }

        /// <summary>
        /// Returns a string indicating that the currently chosen CombatEntity cannot act this turn.
        /// </summary>
        /// <param name="entity">The CombatEntity of focus.</param>
        public static string WriteEntityCannotAct(CombatEntity entity)
        {
            return $"{entity.Name} does not have an action this turn.";
        }

        /// <summary>
        /// Returns a string indicating that the currently chosen CombatEntity cannot act this turn because
        /// it is currently stunned.
        /// </summary>
        /// <param name="entity">The CombatEntity of focus.</param>
        public static string WriteEntityIsStunned(CombatEntity entity)
        {
            return $"{entity.Name} cannot act this turn because it is stunned.";
        }

        /// <summary>
        /// Returns a string indicating that the entity does not have the activated item in its inventory.
        /// </summary>
        /// <param name="entity">The CombatEntity of focus.</param>
        public static string WriteItemDoesntExist(CombatEntity entity)
        {
            return $"Oops, that item does not exist in {entity.Name}'s inventory.";
        }

        /// <summary>
        /// Returns a string indicating that the entity does not know how to use an ability.
        /// </summary>
        /// <param name="entity">The CombatEntity of focus.</param>
        public static string WriteAbilityDoesntExist(CombatEntity entity)
        {
            return $"Oops, {entity.Name} doesn't know how to use that ability.";
        }

        /// <summary>
        /// Returns a string indicating that the entity cannot use an ability because it is silenced.
        /// </summary>
        /// <param name="entity">The CombatEntity of focus.</param>
        /// <param name="ability">The ability being used.</param>
        public static string WriteEntityIsSilenced(CombatEntity entity, Ability ability)
        {
            return $"{entity.Name} cannot use {ability.Name} because {entity.Name} is currently silenced. Being silenced " +
                $"removes your ability to cast spells.";
        }

        /// <summary>
        /// Returns a string indicating that the entity cannot use an ability because it is restricted.
        /// </summary>
        /// <param name="entity">The CombatEntity of focus.</param>
        /// <param name="ability">The ability being used.</param>
        public static string WriteEntityIsRestricted(CombatEntity entity, Ability ability)
        {
            return $"{entity.Name} cannot use {ability.Name} because {entity.Name} is currently restricted. Being restricted " +
                $"removes your ability to use physical abilities.";
        }

        /// <summary>
        /// Returns a string indicating that a player's target position is out of bounds.
        /// </summary>
        public static string WriteTargetPositionOutOfBounds()
        {
            return "You are targeting out of the bounds of the Formation.";
        }

        /// <summary>
        /// Returns a string indicating that a player's targets are all dead.
        /// </summary>
        public static string WriteAllTargetsAreDead()
        {
            return "All of your targets are already dead.";
        }

        /// <summary>
        /// Returns a string indicating that a player's ability targets no CombatEntities.
        /// </summary>
        /// <returns></returns>
        public static string WriteNoTargets()
        {
            return "Your ability targets a position with no characters.";
        }

        /// <summary>
        /// Returns a string indicating that the entity cannot use an ability because it doesn't have enough
        /// action points.
        /// </summary>
        /// <param name="entity">The CombatEntity of focus.</param>
        /// <param name="ability">The ability being used.</param>
        public static string WriteInsufficientActionPoints(CombatEntity entity, Ability ability)
        {
            int totalActionPointCost = ResourceCalculator.GetTotalActionPointCost(entity, ability);
            return $"{entity.Name} does not have enough Action Points to use {ability.Name}. {ability.Name} " +
                $"requires {totalActionPointCost} Action Points, {entity.Name} has {entity.Resources.CurrentActionPoints}.";
        }

        /// <summary>
        /// Returns a string indicating that the entity cannot use an ability because it doesn't have enough
        /// mana.
        /// </summary>
        /// <param name="entity">The CombatEntity of focus.</param>
        /// <param name="ability">The ability being used.</param>
        public static string WriteInsufficientMana(CombatEntity entity, Ability ability)
        {
            int totalManaCost = ResourceCalculator.GetTotalManaCost(entity, ability);
            return $"{entity.Name} does not have enough Mana to use {ability.Name}. {ability.Name} " +
                $"requires {totalManaCost} Mana, {entity.Name} has {entity.Resources.CurrentMana}.";
        }

        /// <summary>
        /// Returns a string indicating that the entity cannot use an ability because it doesn't have enough
        /// health.
        /// </summary>
        /// <param name="entity">The CombatEntity of focus.</param>
        /// <param name="ability">The ability being used.</param>
        public static string WriteInsufficientHealth(CombatEntity entity, Ability ability)
        {
            int totalHealthCost = ResourceCalculator.GetTotalHealthCost(entity, ability);
            return $"{entity.Name} does not have enough Health to use {ability.Name}. {ability.Name} " +
                $"requires {totalHealthCost} Health, {entity.Name} has {entity.Resources.CurrentHealth}.";
        }
    }
}
