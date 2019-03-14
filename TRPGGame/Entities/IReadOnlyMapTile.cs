using System.Collections.Generic;
using TRPGShared;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a read-only version of an in-game map tile.
    /// </summary>
    public interface IReadOnlyMapTile
    {
        /// <summary>
        /// The id unique to this tile.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The uris containing the layers of icons that represents this tile on the world map.
        /// </summary>
        IEnumerable<string> IconUris { get; }

        /// <summary>
        /// Gets or sets whether or not this tile blocks player movement.
        /// </summary>
        bool IsBlocking { get; }

        /// <summary>
        /// Gets or sets whether or not the player can move to another map if on this tile.
        /// </summary>
        bool CanTransport { get; }

        /// <summary>
        /// Gets or sets the id of the map the player will move to if the player changes map while on this tile.
        /// </summary>
        int TransportMapId { get; }

        /// <summary>
        /// Gets or sets the coordinates the player will arrive at if it moves to another map while on this tile.
        /// </summary>
        Coordinate TransportLocation { get; }
    }
}