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
    [Authorize]
    public class BattleHub : Hub
    {
        private readonly IStateManager _stateManager;
        private readonly WorldEntityManager _worldEntityManager;

        public BattleHub(IStateManager stateManager,
                         WorldEntityManager worldEntityManager)
        {
            _stateManager = stateManager;
            _worldEntityManager = worldEntityManager;
        }

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
                var manager = _worldEntityManager.GetPlayerEntityManager(userId);
                var battle = new CulledBattle(manager.GetBattleManager().GetBattle());
                await Clients.Caller.SendAsync("initialized", battle);
            }
        }

        public async Task PerformAction(BattleAction action)
        {
            var userId = Guid.Parse(Context.UserIdentifier);
            action.OwnerId = userId;
            var manager = _worldEntityManager.GetPlayerEntityManager(userId).GetBattleManager();
            if (manager != null)
            {
                var success = await manager.PerformActionAsync(action);
                if (!success) await Clients.Caller.SendAsync("invalidAction", action);
            }
        }
    }
}
