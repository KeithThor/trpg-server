using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities.Combat
{
    /// <summary>
    /// Model object containing the current action points for a given CombatEntity.
    /// </summary>
    public class ActionPointData
    {
        /// <summary>
        /// The id of the CombatEntity whose ActionPointData is stored in this object.
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// The amount of ActionPoints the CombatEntity currently has.
        /// </summary>
        public int CurrentActionPoints { get; set; }
    }
}
