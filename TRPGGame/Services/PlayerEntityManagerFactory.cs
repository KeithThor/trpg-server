using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Managers;

namespace TRPGGame.Services
{
    public class PlayerEntityManagerFactory
    {
        private readonly IWorldState _worldState;
        private readonly IStateManager _stateManager;
        private readonly BattleManagerFactory _battleManagerFactory;

        public PlayerEntityManagerFactory(IWorldState worldState,
                                          IStateManager stateManager,
                                          BattleManagerFactory battleManagerFactory)
        {
            _worldState = worldState;
            _stateManager = stateManager;
            _battleManagerFactory = battleManagerFactory;
        }

        public PlayerEntityManager Create(Guid playerId)
        {
            var manager = new PlayerEntityManager(_worldState, _stateManager)
            {
                PlayerId = playerId
            };

            return manager;
        }
    }
}
