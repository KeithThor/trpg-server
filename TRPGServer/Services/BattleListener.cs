using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRPGGame.EventArgs;
using TRPGGame.Managers;
using TRPGServer.Hubs;

namespace TRPGServer.Services
{
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
        }

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

        private async void OnEndOfTurn(object sender, EndOfTurnEventArgs args)
        {
            // Todo: Send delayed abilities too
            await _battleHubContext.Clients.Users(args.ParticipantIds).SendAsync("endOfTurn", args.AffectedEntities);
        }

        private async void OnEndOfBattle(object sender, EndOfBattleEventArgs args)
        {
            await _battleHubContext.Clients.Users(args.ParticipantIds).SendAsync("endBattle");
        }
    }
}
