using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities.Data
{
    /// <summary>
    /// A template used to create WorldEntities that belong to the Ai.
    /// </summary>
    public class EnemyFormationTemplate
    {
        /// <summary>
        /// The id of this template.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name given to the WorldEntity who is created from this template.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The iconUris that make up the visuals for the WorldEntity created from this template.
        /// </summary>
        public IEnumerable<string> IconUris { get; set; }

        /// <summary>
        /// Contains the EntityBases that belong in each position of the Formation created from this template.
        /// </summary>
        public EnemyEntityBase[][] EntityBases { get; set; }

        /// <summary>
        /// Contains the id of the leader of this formation.
        /// </summary>
        public int LeaderId { get; set; }
    }
}
