using System;
using System.Collections.Generic;
using System.Text;
using TRPGShared;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a tile on the world map.
    /// </summary>
    public class MapTile : IReadOnlyMapTile
    {
        public MapTile() { }

        public MapTile(MapTile tile)
        {
            Id = tile.Id;
            IconUris = tile.IconUris;
            IsBlocking = tile.IsBlocking;
            CanTransport = tile.CanTransport;
            TransportMapId = tile.TransportMapId;
            TransportLocation = tile.TransportLocation;
        }

        /// <summary>
        /// The id unique to this tile.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The uri containing the icon that represents this tile on the world map.
        /// </summary>
        public IEnumerable<string> IconUris { get; set; }

        /// <summary>
        /// Gets or sets whether or not this tile blocks player movement.
        /// </summary>
        public bool IsBlocking { get; set; }

        /// <summary>
        /// Gets or sets whether or not the player can move to another map if on this tile.
        /// </summary>
        public bool CanTransport { get; set; }

        /// <summary>
        /// Gets or sets the id of the map the player will move to if the player changes map while on this tile.
        /// </summary>
        public int TransportMapId { get; set; }

        /// <summary>
        /// Gets or sets the coordinates the player will arrive at if it moves to another map while on this tile.
        /// </summary>
        public Coordinate TransportLocation { get; set; }
    }
}
