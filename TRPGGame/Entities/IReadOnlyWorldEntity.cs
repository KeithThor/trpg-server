using System;
using TRPGShared;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a WorldEntity that has readonly properties.
    /// </summary>
    public interface IReadOnlyWorldEntity
    {
        /// <summary>
        /// The id of this WorldEntity.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The id of the player who owns this WorldEntity.
        /// </summary>
        Guid OwnerGuid { get; }

        /// <summary>
        /// The display name of the world entity.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The Uri containing the icon that represents this WorldEntity on the map.
        /// </summary>
        string IconUri { get; }

        /// <summary>
        /// The current X-Y position this WorldEntity occupies on the map the entity resides in.
        /// </summary>
        Coordinate Position { get; }

        /// <summary>
        /// The id of the map this WorldEntity resides in.
        /// </summary>
        int CurrentMapId { get; }
    }
}