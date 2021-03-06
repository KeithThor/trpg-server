﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRPGGame;
using TRPGGame.Managers;
using TRPGServer.Hubs;

namespace TRPGServer.Services
{
    /// <summary>
    /// Container that holds BattleListener instances.
    /// </summary>
    public class BattleListenerContainer
    {
        private readonly IHubContext<BattleHub> _battleHubContext;
        private readonly IWorldState _worldState;

        public BattleListenerContainer(IHubContext<BattleHub> battleHubContext,
                                       IWorldState worldState)
        {
            _battleHubContext = battleHubContext;
            _worldState = worldState;
            //foreach (var manager in _worldState.MapBattleManagers.Values)
            //{
            //    manager.OnCreatedBattle += (sender, args) =>
            //    {
            //        CreateListener(args.BattleManager);
            //    };
            //}
        }

        public List<BattleListener> BattleListeners { get; set; } = new List<BattleListener>();

        /// <summary>
        /// Creates a BattleListener that listens to events emitted from the given IBattleManager instance.
        /// </summary>
        /// <param name="battleManager">The instance to listen to events from.</param>
        public void CreateListener(IBattleManager battleManager)
        {
            var listener = new BattleListener(battleManager, _battleHubContext);
            listener.OnDestroy += OnListenerDestroy;
            BattleListeners.Add(listener);
        }

        /// <summary>
        /// Function invoked when a listener is queued to be destroyed; removes the listener from
        /// the listeners list and removes any event bindings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnListenerDestroy(object sender, System.EventArgs args)
        {
            var listener = sender as BattleListener;
            BattleListeners.Remove(listener);
            listener.OnDestroy -= OnListenerDestroy;
        }
    }
}
