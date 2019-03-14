using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRPGGame;
using TRPGServer.Models;
using TRPGServer.Services;
using TRPGShared;

namespace TRPGServer.Hubs
{
    /// <summary>
    /// Hub responsible for relaying world entity data between the server and client.
    /// </summary>
    [Authorize]
    public class WorldEntityHub : Hub
    {
        private readonly WorldEntityManager _worldEntityManager;
        private readonly MapListenerContainer _listenerContainer;
        private readonly IWorldState _worldState;

        public WorldEntityHub(WorldEntityManager worldEntityManager,
                              MapListenerContainer container,
                              IWorldState worldState)
        {
            _worldEntityManager = worldEntityManager;
            _listenerContainer = container;
            _worldState = worldState;
        }

        /// <summary>
        /// Starts the initial connection, generating the player entity manager for the player and starts the connection
        /// to the map the player was last in.
        /// </summary>
        /// <returns></returns>
        public async Task StartConnection()
        {
            var manager = await _worldEntityManager.GetPlayerEntityManagerAsync(Guid.Parse(Context.UserIdentifier));
            int mapId = manager.GetCurrentMap().Id;
            await Groups.AddToGroupAsync(Context.ConnectionId, mapId.ToString());
        }

        /// <summary>
        /// Puts the PlayerEntityManager into a playable state and inserts the player's entity into the game map.
        /// </summary>
        /// <returns></returns>
        public async Task BeginPlay()
        {
            var manager = await _worldEntityManager.GetPlayerEntityManagerAsync(Guid.Parse(Context.UserIdentifier));
            manager.BeginPlay();
        }

        /// <summary>
        /// Called whenever the client requests to change maps.
        /// </summary>
        /// <returns></returns>
        public async Task ChangeMaps()
        {
            var manager = await _worldEntityManager.GetPlayerEntityManagerAsync(Guid.Parse(Context.UserIdentifier));
            // Calling ChangeMap will restart an inactive connection with the server
            if (!manager.IsActive)
            {
                await StartConnection();
                await BeginPlay();
            }
            int previousMapId = manager.GetCurrentMap().Id;
            int newMapId = -1;
            var success = manager.ChangeMaps(ref newMapId);

            if (success && newMapId != -1)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, previousMapId.ToString());
                await Clients.Caller.SendAsync("changeMapsSuccess", newMapId);
                await Groups.AddToGroupAsync(Context.ConnectionId, newMapId.ToString());
            }
        }

        ///// <summary>
        ///// Called whenever the client requests to change maps.
        ///// </summary>
        ///// <param name="newMapId">The id of the map to change to.</param>
        ///// <returns></returns>
        //public async Task ChangeMap(int newMapId)
        //{
        //    var manager = await _worldEntityManager.GetPlayerEntityManagerAsync(Guid.Parse(Context.UserIdentifier));
        //    int previousMapId = manager.GetCurrentMap().Id;
        //    var success = manager.ChangeMaps(newMapId);
        //    if (success)
        //    {
        //        await Groups.RemoveFromGroupAsync(Context.ConnectionId, previousMapId.ToString());
        //        await Groups.AddToGroupAsync(Context.ConnectionId, newMapId.ToString());
        //    }
        //}

        /// <summary>
        /// Called whenever the client logs out or terminates the connection to the website, signaling a removal of their
        /// world entity and player manager.
        /// </summary>
        public void EndConnection()
        {
            var removedManager = _worldEntityManager.RemovePlayerEntityManager(Guid.Parse(Context.UserIdentifier));
            if (removedManager != null)
            {
                int mapId = removedManager.GetCurrentMap().Id;
                Groups.RemoveFromGroupAsync(Context.ConnectionId, mapId.ToString());
                removedManager.EndConnection();
            }
        }

        /// <summary>
        /// Called by the client to move their entity a specified amount from their current location.
        /// </summary>
        /// <param name="delta">The amount of change to move from the current location.</param>
        /// <returns></returns>
        public async Task MoveEntity(Coordinate delta)
        {
            var manager = await _worldEntityManager.GetPlayerEntityManagerAsync(Guid.Parse(Context.UserIdentifier));
            // Calling this method will restart an inactive connection with the server.
            if (!manager.IsActive)
            {
                await StartConnection();
                await BeginPlay();
            }
            bool success = manager.MoveEntity(delta.PositionX, delta.PositionY);

            if (success)
            {
                int mapId = manager.GetCurrentMap().Id;
                await Clients.Group(mapId.ToString()).SendAsync("updateMovement", manager.GetEntity().Position);
            }
        }

        /// <summary>
        /// Called by the client to get missing entity data.
        /// </summary>
        /// <param name="entityIds">The ids of the entity's to get data of.</param>
        /// <returns></returns>
        public async Task RequestEntityData(IEnumerable<int> entityIds)
        {
            var manager = await _worldEntityManager.GetPlayerEntityManagerAsync(Guid.Parse(Context.UserIdentifier));
            int mapId = manager.GetCurrentMap().Id;
            var entities = _listenerContainer.GetDisplayEntityStore(mapId)
                                             .GetDisplayEntities()
                                             .Where(entity => entityIds.Contains(entity.Id));

            await Clients.Caller.SendAsync("receiveMissingEntities", entities);
        }
    }
}
