using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRPGGame.Managers;
using TRPGServer.Hubs;

namespace TRPGServer.Services
{
    public class BattleListenerContainer
    {
        private readonly IHubContext<BattleHub> _battleHubContext;

        public BattleListenerContainer(IHubContext<BattleHub> battleHubContext)
        {
            _battleHubContext = battleHubContext;
        }

        public List<BattleListener> BattleListeners { get; set; }

        public void CreateListener(IBattleManager battleManager)
        {
            BattleListeners.Add(new BattleListener(battleManager, _battleHubContext));
        }
    }
}
