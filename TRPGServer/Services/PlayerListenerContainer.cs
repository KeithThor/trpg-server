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
    /// Container responsible for creating and removing PlayerManagerListeners.
    /// </summary>
    public class PlayerListenerContainer
    {
        private readonly IPlayerEntityManagerStore _managerStore;
        private readonly BattleListenerContainer _battleListenerContainer;
        private readonly IHubContext<WorldEntityHub> _hubContext;
        private readonly Dictionary<PlayerEntityManager, PlayerManagerListener> _listeners;

        public PlayerListenerContainer(IPlayerEntityManagerStore playerEntityManagerStore,
                                       BattleListenerContainer battleListenerContainer,
                                       IHubContext<WorldEntityHub> hubContext)
        {
            _managerStore = playerEntityManagerStore;
            _managerStore.OnPlayerEntityManagerCreated += OnCreated;

            _battleListenerContainer = battleListenerContainer;
            _hubContext = hubContext;
            _listeners = new Dictionary<PlayerEntityManager, PlayerManagerListener>();
        }

        /// <summary>
        /// Called whenever a new PlayerEntityManager is being created. Creates a PlayerManagerListener
        /// for the newly created manager.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnCreated(object sender, PlayerEntityManagerCreatedEventArgs args)
        {
            if (args.CreatedManager == null) throw new Exception("IPlayerEntityManagerStore created null PlayerEntityManager!");

            var listener = new PlayerManagerListener(args.CreatedManager,
                                                     _battleListenerContainer,
                                                     _hubContext);

            _listeners.Add(args.CreatedManager, listener);
            listener.OnDestroy += OnDestroyingListener;
        }

        /// <summary>
        /// Called when a PlayerManagerListener is being destroyed. Removes it from memory and removes this
        /// function from the destroyed manager's event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnDestroyingListener(object sender, EventArgs args)
        {
            PlayerManagerListener listener = sender as PlayerManagerListener;
            if (listener == null) throw new Exception("Destroyed listener is not a PlayerManagerListener!");

            _listeners.Remove(listener.Manager);
            listener.OnDestroy -= OnDestroyingListener;
        }
    }
}
