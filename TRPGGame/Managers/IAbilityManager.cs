using System.Collections.Generic;
using TRPGGame.Entities;
using TRPGGame.Entities.Combat;

namespace TRPGGame.Managers
{
    /// <summary>
    /// Object representing the return value of a PerformAction result.
    /// </summary>
    public class AbilityResult
    {
        /// <summary>
        /// Contains an IEnumerable of all CombatEntities affected by the Ability.
        /// </summary>
        public IEnumerable<CombatEntity> AffectedEntities { get; set; }

        /// <summary>
        /// If this property is set, the Ability failed. Contains a string that explains to the player
        /// why the Ability failed.
        /// </summary>
        public string FailureReason { get; set; }
    }

    /// <summary>
    /// Object representing the return value of CreateDelayedAbility.
    /// </summary>
    public class DelayedAbilityResult
    {
        /// <summary>
        /// Contains the successfully created DelayedAbility, if any.
        /// </summary>
        public DelayedAbility DelayedAbility { get; set; }

        /// <summary>
        /// If this property is set, creating the DelayedAbility failed. Contains a string that explains to the player
        /// why the DelayedAbility failed.
        /// </summary>
        public string FailureReason { get; set; }
    }

    /// <summary>
    /// Manager responsible for applying the effects of Abilities onto the correct targets in combat.
    /// </summary>
    public interface IAbilityManager
    {
        /// <summary>
        /// Performs an ability on a target location, applying damage, healing, and status effects to any targets.
        /// </summary>
        /// <param name="attacker">The attacker performing the ability.</param>
        /// <param name="ability">The ability used to attack the enemy.</param>
        /// <param name="action">Contains info about the targets for the attack.</param>
        /// <param name="targetFormation">The Formation that the CombatEntity is targeting with its action.</param>
        /// <returns></returns>
        AbilityResult PerformAbility(CombatEntity attacker, Ability ability, BattleAction action, Formation targetFormation);

        /// <summary>
        /// Activates the effects of a DelayedAbility on its target formation, returning an IEnumerable of
        /// CombatEntities affected by the DelayedAbility.
        /// </summary>
        /// <param name="ability">The DelayedAbility to activate the effects of.</param>
        /// <returns></returns>
        IEnumerable<CombatEntity> PerformDelayedAbility(DelayedAbility ability);

        /// <summary>
        /// Creates an instance of a DelayedAbility.
        /// </summary>
        /// <param name="attacker">The CombatEntity starting the effect of a DelayedAbility.</param>
        /// <param name="ability">The Ability to convert into a DelayedAbility.</param>
        /// <param name="action">The object containing details about the action being performed.</param>
        /// <param name="targetFormation">The Formation that the CombatEntity is targeting with its action.</param>
        /// <returns></returns>
        DelayedAbilityResult CreateDelayedAbility(CombatEntity attacker, Ability ability, BattleAction action, Formation targetFormation);
    }
}