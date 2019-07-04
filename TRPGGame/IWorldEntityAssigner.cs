using System;
using TRPGGame.Entities;

namespace TRPGGame
{
    /// <summary>
    /// Represents a class that allows assigning a new WorldEntity to a player.
    /// </summary>
    public interface IWorldEntityAssigner
    {
        /// <summary>
        /// Creates a new WorldEntity and assigns it to the PlayerEntityManager.
        /// </summary>
        /// <param name="ownerId">The id of the player who will own this new entity.</param>
        /// <param name="activeFormation">The Formation this WorldEntity represents.</param>
        /// <returns></returns>
        IReadOnlyWorldEntity AssignWorldEntity(Guid ownerId, Formation activeFormation);
    }
}