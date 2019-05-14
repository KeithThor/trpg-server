using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities.Combat
{
    /// <summary>
    /// Data object that holds the ids of active entities in a given formation.
    /// </summary>
    public class ActiveEntities
    {
        /// <summary>
        /// The ids of all active entities in a given formation.
        /// </summary>
        public IEnumerable<int> EntityIds { get; set; }

        /// <summary>
        /// The Guid that represents the owner of the formation.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// The id of the formation that the active entities exist in.
        /// </summary>
        public int FormationId { get; set; }
    }
}
