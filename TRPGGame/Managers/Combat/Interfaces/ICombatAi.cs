using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;
using TRPGShared;

namespace TRPGGame.Managers.Combat.Interfaces
{
    public interface ICombatAi
    {
        CombatAiDecision MakeDecision(Formation myFormation,
                                      CombatEntity myActiveEntity,
                                      IEnumerable<Formation> myAllies,
                                      IEnumerable<Formation> myEnemies);
    }

    /// <summary>
    /// Data object representing a CombatAi's choice in targets and ability for one CombatEntity it controls.
    /// </summary>
    public class CombatAiDecision
    {
        /// <summary>
        /// The target Formation of the CombatAi's decision.
        /// </summary>
        public Formation TargetFormation { get; set; }

        /// <summary>
        /// The target Coordinate of the Formation of the CombatAi's decision.
        /// </summary>
        public Coordinate TargetCoordinate { get; set; }

        /// <summary>
        /// The Ability the CombatAi has chosen to perform.
        /// </summary>
        public Ability Ability { get; set; }

        /// <summary>
        /// The CombatEntity who will be performing the Ability at the target.
        /// </summary>
        public CombatEntity Actor { get; set; }

        /// <summary>
        /// If true, will perform a Defend action instead of another Ability action.
        /// </summary>
        public bool IsDefending { get; set; }
    }
}
