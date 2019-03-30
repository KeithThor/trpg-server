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
        /// The name to give the newly created formation.
        /// </summary>
        public string Name { get; set; }
    }
}
