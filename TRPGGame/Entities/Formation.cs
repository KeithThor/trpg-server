using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Object representing an in-game combat formation.
    /// </summary>
    public class Formation : IReadOnlyFormation
    {
        /// <summary>
        /// The unique identifier for this instance.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name the user created for this formation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unique identifier for the owner of this formation.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// A 2d array containing the CombatEntities who exist in the formation.
        /// </summary>
        public CombatEntity[][] Positions { get; set; }

        /// <summary>
        /// Contains the unique identifier of the leader of this Formation.
        /// </summary>
        public int LeaderId { get; set; }

        /// <summary>
        /// The ai uses this value to influence how random its decisions are in battle using this formation.
        /// <para>An AiRandomness value of 0 will make the Ai always choose the best targets.</para>
        /// <para>An AiRandomness value of 10 will make the Ai always randomize its targets.</para>
        /// <para>A null value defaults to 0.</para>
        /// </summary>
        public int AiRandomness { get; set; }

        /// <summary>
        /// Returns a 2d array containing read-only versions of the CombatEntities who exist in the formation.
        /// </summary>
        IReadOnlyCombatEntity[][] IReadOnlyFormation.Positions => Positions;
    }
}
