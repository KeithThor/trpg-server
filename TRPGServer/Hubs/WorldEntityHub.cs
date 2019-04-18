using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRPGGame;
using TRPGGame.Managers;
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
        public async Task MoveEntity(Coordinate delta)
        {
            var manager = _worldEntityManager.GetPlayerEntityManager(Guid.Parse(Context.UserIdentifier));
            // Calling this method will restart an inactive connection with the server.
            if (!manager.IsActive)
            {
                await StartConnection();
                BeginPlay();
            }
            bool success = manager.MoveEntity(delta.PositionX, delta.PositionY, out bool canStartBattle);

            // Potentially useless code
            if (success)
            {
                int mapId = manager.GetCurrentMap().Id;
                await Clients.Group(mapId.ToString()).SendAsync("updateMovement", manager.Entity.Position);
                if (canStartBattle) await Clients.Caller.SendAsync("canStartBattle");
            }
        }

        public async Task InitiateBattle()
        {
            var manager = _worldEntityManager.GetPlayerEntityManager(Guid.Parse(Context.UserIdentifier));
            var battleManager = manager.StartBattle();
            if (battleManager != null)
            {
                _battleListenerContainer.CreateListener(battleManager);
                var tasks = new List<Task>();
                var participantIds = battleManager.GetParticipantIds();
                foreach (var id in participantIds)
                {
                    tasks.Add(Clients.User(id).SendAsync("battleInitiated"));
                }
                await Task.WhenAll(tasks);
            }
        }

        public async Task JoinBattle(Guid joinUserId, bool asAttacker)
        {
            var userId = Guid.Parse(Context.UserIdentifier);
            var playerState = _stateManager.GetPlayerState(userId);
            if (playerState != PlayerStateConstants.Free)
            {
                await Clients.Caller.SendAsync("invalidState", playerState);
                return;
            }

            var callerManager = _worldEntityManager.GetPlayerEntityManager(userId);
            var joinManager = _worldEntityManager.GetPlayerEntityManager(joinUserId);

            var battleManager = joinManager.JoinMyBattle(callerManager, asAttacker);
            if (battleManager != null)
            {
                await Clients.Caller.SendAsync("battleInitiated");
            }
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
    }
}
