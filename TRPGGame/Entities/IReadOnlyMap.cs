using System.Collections.Generic;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a map entity with readonly properties.
    /// </summary>
    public interface IReadOnlyMap
    {
        /// <summary>
        /// The unique identifier for this map.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The 2d representation of map tiles that make up this map.
        /// </summary>
        IReadOnlyList<IReadOnlyList<IReadOnlyMapTile>> MapData { get; }

        /// <summary>
        /// Contains all the different types of map tiles that exist in this map that can be used in loading data.
        /// </summary>
        IEnumerable<IReadOnlyMapTile> UniqueTiles { get; }

        /// <summary>
        /// Contains the ids of the maps that this map's tiles can transport to.
        /// </summary>
        IEnumerable<int> MapConnections { get; }
    }
}