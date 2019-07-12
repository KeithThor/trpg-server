using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;
using TRPGGame.Entities.Combat;

namespace TRPGGame.EventArgs
{
    /// <summary>
    /// Object containing data for a StartOfTurnEvent.
    /// </summary>
    public class StartOfTurnEventArgs
    {
        /// <summary>
        /// Contains a list of delayed Abilities that were activated at the start of the turn.
        /// </summary>
        public List<IReadOnlyAbility> DelayedAbilities { get; set; }

        /// <summary>
        /// Contains a list of CombatEntities affected by StatusEffects or delayed Abilities at the start of the turn.
        /// </summary>
        public List<IReadOnlyCombatEntity> AffectedEntities { get; set; }

        /// <summary>
        /// A Dictionary whose key is the Formation id and value the ActionPointData of all CombatEntities who
        /// had their CurrentActionPoints altered.
        /// </summary>
        public Dictionary<int, IEnumerable<ActionPointData>> ActionPointData { get; set; }

        /// <summary>
        /// Contains the DateTime at which the turn will automatically be ended.
        /// </summary>
        public DateTime TurnExpiration { get; set; }

        /// <summary>
        /// Contains ids of all CombatEntities that can currently act this turn.
        /// </summary>
        public List<ActiveEntities> ActiveEntities { get; set; }

        /// <summary>
        /// A List of string containing the ids(Guid) of all players in this battle.
        /// </summary>
        public IReadOnlyList<string> ParticipantIds { get; set; }

        /// <summary>
        /// If true, it is currently the defenders turn.
        /// </summary>
        public bool IsDefendersTurn { get; set; }
    }
}
