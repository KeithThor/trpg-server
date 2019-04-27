using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TRPGServer.Models;

namespace TRPGServer.Services
{
    /// <summary>
    /// Interface of a class that provides the ability to retrieve DisplayEntities.
    /// </summary>
    public interface IDisplayEntityStore
    {
        /// <summary>
        /// Gets all of the DisplayEntities for the map this listener is listening to.
        /// </summary>
        /// <returns></returns>
        IEnumerable<DisplayEntity> GetDisplayEntities();

        /// <summary>
        /// Sends the DisplayEntity of the player with the given id to said player.
        /// </summary>
        /// <param name="ownerId">The unique identifier that represents the player.</param>
        /// <returns></returns>
        Task SendOwnedEntityAsync(Guid ownerId);
    }
}