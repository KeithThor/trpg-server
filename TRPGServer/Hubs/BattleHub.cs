using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRPGGame;
using TRPGGame.Entities.Combat;
using TRPGGame.Managers;
using TRPGServer.Models;

namespace TRPGServer.Hubs
{
    /// <summary>
    /// Hub responsible for establishing a connection between the user and the user's battle instance.
    /// </summary>
    [Authorize]
    public class BattleHub : Hub
    {
        private readonly IStateManager _stateManager;
        private readonly IPlayerEntityManagerStore _playerEntityManagerStore;

        public BattleHub(IStateManager stateManager,
                         IPlayerEntityManagerStore playerEntityManagerStore)
        {
            _stateManager = stateManager;
            _playerEntityManagerStore = playerEntityManagerStore;
        }

        /// <summary>
        /// Checks if the user is in battle; if so, returns a copy of the current battle to the user. If not,
        /// sends an invalid state message.
        /// </summary>
        /// <returns></returns>
        public async Task StartConnection()
        {
            var userId = Guid.Parse(Context.UserIdentifier);
            var state = _stateManager.GetPlayerState(userId);
            if (state != PlayerStateConstants.InCombat)
            {
                await Clients.Caller.SendAsync("notInCombat", state);
            }
            else
            {
                var manager = _playerEntityManagerStore.GetPlayerEntityManager(userId);
                var battle = new CulledBattle(manager.GetBattleManager().GetBattle());
                await Clients.Caller.SendAsync("initialized", battle);
            }
        }

        /// <summary>
        /// Called by the client to attempt to perform an action using the provided BattleAction template.
        /// </summary>
        /// <param name="action">A basic data object containing the parameters to perform an action in battle.</param>
        /// <returns></returns>
        public async Task PerformAction(BattleAction action)
        {
            var userId = Guid.Parse(Context.UserIdentifier);
            action.OwnerId = userId;
            var manager = _playerEntityManagerStore.GetPlayerEntityManager(userId).GetBattleManager();
            if (manager != null)
            {
                var success = await manager.PerformActionAsync(action);
                if (!success) await Clients.Caller.SendAsync("invalidAction", action);
            }
        }
    }
}
