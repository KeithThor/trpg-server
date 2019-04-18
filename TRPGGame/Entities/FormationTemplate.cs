using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities
{
    /// <summary>
    /// A template used to create a Formation.
    /// </summary>
    public class FormationTemplate
    {
        /// <summary>
        /// The unique identifier of the Formation this template will override, if any.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// The unique identifier of the user who will own the Formation created from this template.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// A 2d array containing the ids of characters who occupy a spot in the given formation space.
        /// </summary>
        public int?[][] Positions { get; set; }

        /// <summary>
        /// Contains the id of the class the player has chosen for this CombatEntity.
        /// </summary>
        public int ClassId { get; set; }

        /// <summary>
        /// The name to give the newly created formation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Contains the unique identifier of the leader of the Formation.
        /// </summary>
        public int LeaderId { get; set; }

        /// <summary>
        /// If true, will make the Formation created from this template the active Formation.
        /// </summary>
        public bool MakeActive { get; set; }
    }
}
