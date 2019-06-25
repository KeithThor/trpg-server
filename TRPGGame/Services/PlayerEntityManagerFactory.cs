using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Managers;

namespace TRPGGame.Services
{
    /// <summary>
    /// Factory responsible for creating instances of PlayerEntityManagers.
    /// </summary>
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

        /// <summary>
        /// Creates a new PlayerEntityManager for the user of the given id.
        /// </summary>
        /// <param name="playerId">The id of the player to create a manager for.</param>
        /// <param name="playerEntityManagerStore">The </param>
        /// <returns></returns>
        public PlayerEntityManager Create(Guid playerId, IPlayerEntityManagerStore playerEntityManagerStore)
        {
            var manager = new PlayerEntityManager(_worldState, _stateManager, playerEntityManagerStore)
            {
                PlayerId = playerId
            };

            return manager;
        }
    }
}
