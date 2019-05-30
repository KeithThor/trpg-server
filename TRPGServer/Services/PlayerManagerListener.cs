using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRPGGame;
using TRPGGame.EventArgs;
using TRPGServer.Hubs;

namespace TRPGServer.Services
{
    /// <summary>
    /// Listener that propagates events from a PlayerEntityManager to clients.
    /// </summary>
    public class PlayerManagerListener
    {
        private readonly PlayerEntityManager _manager;
        private readonly BattleListenerContainer _battleListenerContainer;
        private readonly IHubContext<WorldEntityHub> _hubContext;

        public PlayerManagerListener(PlayerEntityManager manager,
                                     BattleListenerContainer battleListenerContainer,
                                     IHubContext<WorldEntityHub> hubContext)
        {
            _manager = manager;
            _battleListenerContainer = battleListenerContainer;
            _hubContext = hubContext;
            _manager.OnDestroy += ManagerOnDestroy;
            _manager.OnMovementStopped += OnMovementStopped;
            _manager.OnJoinBattleSuccess += OnJoinBattleSuccess;
        }

        /// <summary>
        /// Event invoked whenever this listener is going to be destroyed.
        /// </summary>
        public event EventHandler<EventArgs> OnDestroy;

        /// <summary>
        /// Called whenever the PlayerEntityManager this listener listens to is queued for destruction.
        /// <para>Queues this listener to be destroyed as well.</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ManagerOnDestroy(object sender, EventArgs args)
        {
            OnDestroy?.Invoke(this, args);
        }

        /// <summary>
        /// Sends a message to the client that the player's WorldEntity has stopped moving.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnMovementStopped(object sender, EventArgs args)
        {
            await _hubContext.Clients.User(_manager.PlayerId.ToString()).SendAsync("movementStopped");
        }

        /// <summary>
        /// Sends a message to the client that the player has initiated battle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnBattleInitiated(object sender, CreatedBattleEventArgs args)
        {
            _battleListenerContainer.CreateListener(args.BattleManager);

            await _hubContext.Clients.User(_manager.PlayerId.ToString()).SendAsync("battleInitiated");
        }

        /// <summary>
        /// Sends a message to the client that the player has successfully joined a battle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnJoinBattleSuccess(object sender, JoinBattleSuccessEventArgs args)
        {
            _battleListenerContainer.CreateListener(args.BattleManager);

            await _hubContext.Clients.User(_manager.PlayerId.ToString()).SendAsync("battleInitiated");
        }
    }
}
