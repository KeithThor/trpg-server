﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRPGGame.EventArgs;
using TRPGGame.Managers;
using TRPGServer.Hubs;

namespace TRPGServer.Services
{
    /// <summary>
    /// Listener responsible for sending events in battle to clients who are connected to the same instance of
    /// Battle as this listener is.
    /// </summary>
    public class BattleListener
    {
        private readonly IBattleManager _battleManager;
        private readonly IHubContext<BattleHub> _battleHubContext;

        public BattleListener(IBattleManager battleManager,
                              IHubContext<BattleHub> battleHubContext)
        {
            _battleManager = battleManager;
            _battleHubContext = battleHubContext;
            _battleManager.EndOfBattleEvent += OnEndOfBattle;
            _battleManager.EndOfTurnEvent += OnEndOfTurn;
            _battleManager.StartOfTurnEvent += OnStartOfTurn;
            _battleManager.SuccessfulActionEvent += OnSuccessfulAction;
        }

        /// <summary>
        /// Formats data to send to the client on the start of a new turn in battle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnStartOfTurn(object sender, StartOfTurnEventArgs args)
        {
            var activeEntities = new List<dynamic>();
            foreach (var kvp in args.ActiveEntities)
            {
                activeEntities.Add(new
                {
                    FormationId = kvp.Key,
                    EntityIds = kvp.Value
                });
            }
            int turnExpiration = (int)(args.TurnExpiration - DateTime.Now).TotalSeconds;

            // Todo: Send delayed abilities too
            await _battleHubContext.Clients.Users(args.ParticipantIds).SendAsync("startOfTurn", new
            {
                activeEntities,
                args.AffectedEntities,
                args.IsDefendersTurn,
                turnExpiration
            });
        }

        /// <summary>
        /// On a successful action, send all clients the updated data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnSuccessfulAction(object sender, SuccessfulActionEventArgs args)
        {
            await _battleHubContext.Clients.Users(args.ParticipantIds).SendAsync("actionSuccess", new
            {
                args.Ability,
                args.Action,
                args.Actor,
                args.AffectedEntities
            });
        }

        /// <summary>
        /// At the end of the turn, send all connected clients the CombatEntities affected by end of turn effects.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnEndOfTurn(object sender, EndOfTurnEventArgs args)
        {
            // Todo: Send delayed abilities too
            await _battleHubContext.Clients.Users(args.ParticipantIds).SendAsync("endOfTurn", args.AffectedEntities);
        }

        /// <summary>
        /// Signal to the client that the battle has ended along with whether the attackers or defenders won.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnEndOfBattle(object sender, EndOfBattleEventArgs args)
        {
            await _battleHubContext.Clients.Users(args.ParticipantIds).SendAsync("endBattle", args.DidAttackersWin);
        }
    }
}
