using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities.Combat
{
    /// <summary>
    /// Represents an instance of battle occuring in-game.
    /// </summary>
    public class Battle : IReadOnlyBattle
    {
        public Battle()
        {
            Id = _id++;
            Round = 1;
        }

        /// <summary>
        /// The id of this battle instance.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The Formations who initiated the battle.
        /// </summary>
        public List<Formation> Attackers { get; set; }

        /// <summary>
        /// The Formations who have been initiated on in battle.
        /// </summary>
        public List<Formation> Defenders { get; set; }

        /// <summary>
        /// A Dictionary containing the Formations whose turn is currently active and the CombatEntities who've yet
        /// to perform in battle.
        /// </summary>
        public Dictionary<Formation, List<CombatEntity>> ActionsLeftPerFormation { get; set; }

        /// <summary>
        /// A List containing all of the DelayedAbilities caused by the Attackers in battle.
        /// </summary>
        public List<DelayedAbility> AttackerDelayedAbilities { get; set; } = new List<DelayedAbility>();

        /// <summary>
        /// A List containing all of the DelayedAbilities created by the Defenders in battle.
        /// </summary>
        public List<DelayedAbility> DefenderDelayedAbilities { get; set; } = new List<DelayedAbility>();

        /// <summary>
        /// If true, the defending Formations are currently active. If false, the attacking Formations are currently active.
        /// </summary>
        public bool IsDefenderTurn { get; set; }

        /// <summary>
        /// The current round number for the battle. The round number is incremented after both attackers and defenders have had
        /// their turns.
        /// </summary>
        public int Round { get; set; }

        /// <summary>
        /// The time at which a turn will expire automatically even if there are actions left.
        /// </summary>
        public DateTime TurnExpiration { get; set; }

        IReadOnlyDictionary<IReadOnlyFormation, IReadOnlyList<IReadOnlyCombatEntity>> IReadOnlyBattle.ActionsLeftPerFormation
        {
            get
            {
                return (IReadOnlyDictionary<IReadOnlyFormation, IReadOnlyList<IReadOnlyCombatEntity>>)ActionsLeftPerFormation;
            }
        }
        IReadOnlyList<IReadOnlyDelayedAbility> IReadOnlyBattle.AttackerDelayedAbilities => AttackerDelayedAbilities;
        IEnumerable<IReadOnlyFormation> IReadOnlyBattle.Attackers => Attackers;
        IEnumerable<IReadOnlyDelayedAbility> IReadOnlyBattle.DefenderDelayedAbilities => DefenderDelayedAbilities;
        IEnumerable<IReadOnlyFormation> IReadOnlyBattle.Defenders => Defenders;

        private static int _id = 0;
    }
}
