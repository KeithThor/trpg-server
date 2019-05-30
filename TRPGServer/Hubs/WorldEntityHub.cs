using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRPGGame;
using TRPGGame.Managers;
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
        private readonly BattleListenerContainer _battleListenerContainer;
        private readonly IStateManager _stateManager;

        public WorldEntityHub(WorldEntityManager worldEntityManager,
                              MapListenerContainer container,
                              BattleListenerContainer battleListenerContainer,
                              IStateManager stateManager)
        {
            _worldEntityManager = worldEntityManager;
            _listenerContainer = container;
            _battleListenerContainer = battleListenerContainer;
            _stateManager = stateManager;
        }

        /// <summary>
        /// Starts the initial connection, generating the player entity manager for the player and starts the connection
        /// to the map the player was last in.
        /// </summary>
        /// <returns></returns>
        public async Task StartConnection()
        {
            var userId = Guid.Parse(Context.UserIdentifier);
            var state = _stateManager.GetPlayerState(userId);
            if (state != PlayerStateConstants.Free)
            {
                if (state == null)
                {
                    _stateManager.SetPlayerMakeCharacter(userId);
                    state = PlayerStateConstants.MakeCharacter;
                }
                await Clients.Caller.SendAsync("invalidState", _stateManager.GetPlayerState(userId));
                return;
            }
            var manager = _worldEntityManager.GetPlayerEntityManager(userId);
            int mapId = manager.GetCurrentMap().Id;
            await Groups.AddToGroupAsync(Context.ConnectionId, mapId.ToString());
        }

        /// <summary>
        /// Puts the PlayerEntityManager into a playable state and inserts the player's entity into the game map.
        /// </summary>
        /// <returns></returns>
        public void BeginPlay()
        {
            var manager = _worldEntityManager.GetPlayerEntityManager(Guid.Parse(Context.UserIdentifier));
            manager.BeginPlay();
        }

        /// <summary>
        /// Called whenever the client requests to change maps.
        /// </summary>
        /// <returns></returns>
        public async Task ChangeMaps()
        {
            if (!await HandleState()) return;

            var manager = _worldEntityManager.GetPlayerEntityManager(Guid.Parse(Context.UserIdentifier));
            // Calling ChangeMap will restart an inactive connection with the server
            if (!manager.IsActive)
            {
                await StartConnection();
                BeginPlay();
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

        /// <summary>
        /// Called whenever the client logs out or terminates the connection to the website, signaling a removal of their
        /// world entity and player manager.
        /// </summary>
        public void EndConnection()
        {
            var removedManager = _worldEntityManager.RemovePlayerEntityManager(Guid.Parse(Context.UserIdentifier));
            if (removedManager != null && removedManager.GetCurrentMap() != null)
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
        public async Task MoveEntity(IEnumerable<Coordinate> movePath)
        {
            if (!await HandleState()) return;

            var manager = _worldEntityManager.GetPlayerEntityManager(Guid.Parse(Context.UserIdentifier));
            // Calling this method will restart an inactive connection with the server.
            if (!manager.IsActive)
            {
                await StartConnection();
                BeginPlay();
            }
            manager.SetMovePath(movePath);
        }

        /// <summary>
        /// Attempts to initiate battle with the given the entity of the given id by following the
        /// provided path.
        /// </summary>
        /// <param name="entityId">The id of the entity to attack.</param>
        /// <param name="ownerId">The id of the owner of the entity to attack.</param>
        /// <param name="action">The name of the action to perform.</param>
        /// <param name="path">The path used to get to the entity.</param>
        /// <returns></returns>
        public async Task QueueAction(int entityId, string ownerId, string action, IEnumerable<Coordinate> path)
        {
            if (! await HandleState()) return;

            var manager = _worldEntityManager.GetPlayerEntityManager(Guid.Parse(Context.UserIdentifier));
            manager.QueueAction(entityId, ownerId, action, path);
        }

        /// <summary>
        /// Called by the client to get missing entity data.
        /// </summary>
        /// <param name="entityIds">The ids of the entity's to get data of.</param>
        /// <returns></returns>
        public async Task RequestEntityData(IEnumerable<int> entityIds)
        {
            var manager = _worldEntityManager.GetPlayerEntityManager(Guid.Parse(Context.UserIdentifier));
            int mapId = manager.GetCurrentMap().Id;
            var entities = _listenerContainer.GetDisplayEntityStore(mapId)
                                             .GetDisplayEntities()
                                             .Where(entity => entityIds.Contains(entity.Id));

            await Clients.Caller.SendAsync("receiveMissingEntities", entities);
        }

        /// <summary>
        /// Called by the client to retrieve the WorldEntity that belongs to the player.
        /// </summary>
        /// <returns></returns>
        public async Task RequestPlayerEntity()
        {
            var manager = _worldEntityManager.GetPlayerEntityManager(Guid.Parse(Context.UserIdentifier));
            int mapId = manager.GetCurrentMap().Id;
            await _listenerContainer.GetDisplayEntityStore(mapId)
                                    .SendOwnedEntityAsync(Guid.Parse(Context.UserIdentifier));
        }

        /// <summary>
        /// Handles the state of a player. Allows normal access to WorldEntityHub functions if the player is
        /// in a valid state; otherwise, sends an invalid state message to the client and returns false.
        /// </summary>
        /// <returns>Returns true if the current player state is valid.</returns>
        private async Task<bool> HandleState()
        {
            var userId = Guid.Parse(Context.UserIdentifier);
            var playerState = _stateManager.GetPlayerState(userId);

            if (playerState != PlayerStateConstants.Free)
            {
                await Clients.Caller.SendAsync("invalidState", playerState);
                return false;
            }

            return true;
        }
    }
}
