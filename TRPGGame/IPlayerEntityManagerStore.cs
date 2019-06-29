using System;
using TRPGGame.EventArgs;

namespace TRPGGame
{
    /// <summary>
    /// Interface that provides a method to allow for retrieval of a PlayerEntityManager using a player's
    /// owner id.
    /// </summary>
    public interface IPlayerEntityManagerStore
    {
        /// <summary>
        /// Event invoked whenever a PlayerEntityManager is being created.
        /// </summary>
        event EventHandler<PlayerEntityManagerCreatedEventArgs> OnPlayerEntityManagerCreated;

        /// <summary>
        /// Gets the PlayerEntityManager responsible for managing the current player's entity.
        /// </summary>
        /// <param name="ownerId">The Guid of the player to get the PlayerEntityManager for.</param>
        /// <returns></returns>
        PlayerEntityManager GetPlayerEntityManager(Guid ownerId);
    }
}