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
        /// Returns a 2d array containing read-only versions of the CombatEntities who exist in the formation.
        /// </summary>
        IReadOnlyCombatEntity[][] IReadOnlyFormation.Positions => Positions;
    }
}
