using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities.Data;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Class that represents a primitive in-game map.
    /// </summary>
    public class Map : IReadOnlyMap
    {
        /// <summary>
        /// The unique identifier for this map.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// A two dimensional list containing the tile layout for the map.
        /// </summary>
        public IReadOnlyList<IReadOnlyList<IReadOnlyMapTile>> MapData { get; set; }

        /// <summary>
        /// Contains all the unique tiles in this map.
        /// </summary>
        public IEnumerable<IReadOnlyMapTile> UniqueTiles { get; set; }

        /// <summary>
        /// Contains the ids of other maps that are connected to this one.
        /// </summary>
        public IEnumerable<int> MapConnections { get; set; }

        /// <summary>
        /// Contains the data used to populate this map with ai-controlled entities.
        /// </summary>
        public List<SpawnEntityData> SpawnData { get; set; }

        IReadOnlyList<SpawnEntityData> IReadOnlyMap.SpawnData => SpawnData;
    }
}