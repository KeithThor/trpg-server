using System;
using System.Collections.Generic;

namespace TRPGGame.Entities.Combat
{
    /// <summary>
    /// Represents an read-only reference to an instance of battle occuring in-game.
    /// </summary>
    public interface IReadOnlyBattle
    {
        /// <summary>
        /// A Dictionary containing the Formations whose turn is currently active and the CombatEntities who've yet
        /// to perform in battle.
        /// </summary>
        IReadOnlyDictionary<IReadOnlyFormation, IReadOnlyList<IReadOnlyCombatEntity>> ActionsLeftPerFormation { get; }

        /// <summary>
        /// A List containing all of the DelayedAbilities created by the Attackers in battle.
        /// </summary>
        IReadOnlyList<IReadOnlyDelayedAbility> AttackerDelayedAbilities { get; }

        /// <summary>
        /// The Formations who initiated the battle.
        /// </summary>
        IEnumerable<IReadOnlyFormation> Attackers { get; }

        /// <summary>
        /// A List containing all of the DelayedAbilities caused by the Defenders in battle.
        /// </summary>
        IEnumerable<IReadOnlyDelayedAbility> DefenderDelayedAbilities { get; }

        /// <summary>
        /// The Formations who have been initiated on in battle.
        /// </summary>
        IEnumerable<IReadOnlyFormation> Defenders { get; }

        /// <summary>
        /// The id of this battle instance.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// If true, the defending Formations are currently active. If false, the attacking Formations are currently active.
        /// </summary>
        bool IsDefenderTurn { get; }

        /// <summary>
        /// The current round number for the battle. The round number is incremented after both attackers and defenders have had
        /// their turns.
        /// </summary>
        int Round { get; }

        /// <summary>
        /// The time at which a turn will expire automatically even if there are actions left.
        /// </summary>
        DateTime TurnExpiration { get; }
    }
}