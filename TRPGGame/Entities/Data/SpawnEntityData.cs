using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities.Data
{
    /// <summary>
    /// A data object that is used to create enemy world entities for a map. This object wraps around
    /// one unique EnemyFormationTemplate.
    /// </summary>
    public class SpawnEntityData
    {
        /// <summary>
        /// The template that represents this SpawnEntityData.
        /// </summary>
        public EnemyFormationTemplate FormationTemplate { get; set; }

        /// <summary>
        /// The maximum number of entities in a given map that can have this EnemyFormationTemplate.
        /// </summary>
        public int MaxEntities { get; set; }

        /// <summary>
        /// The amount of time in seconds after being removed from the map that a new WorldEntity is
        /// replaced.
        /// </summary>
        public int RespawnTime { get; set; }
    }
}
