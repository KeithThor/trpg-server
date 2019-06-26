using System;
using System.Threading;
using TRPGGame.EventArgs;

namespace TRPGGame
{
    public class Game
    {
        public event EventHandler<GameStateUpdateEvent> GameStateUpdate;
        private int _secretData = 0;
        private readonly IWorldState _worldStateHandler;
        private readonly WorldEntityManager _entityManager;

        public CancellationToken CancellationToken { get; set; }

        public Game(IWorldState worldStateHandler,
                    WorldEntityManager entityManager)
        {
            _worldStateHandler = worldStateHandler;
            _entityManager = entityManager;
        }

        public void StartGame()
        {
            if (CancellationToken == null) CancellationToken = new CancellationToken();
            GameLoop();
        }

        private void GameLoop()
        {
            while(!CancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(GameplayConstants.GameTicksPerSecond);
                _secretData++;
                _worldStateHandler.OnGameTick();
                if (_secretData % (1000 / GameplayConstants.GameTicksPerSecond) == 0)
                {
                    GameStateUpdate?.Invoke(this, new GameStateUpdateEvent
                    {
                        GameState = $"Game state changed {_secretData}"
                    });
                }
                if (_secretData >= GameplayConstants.InactiveTimeoutDuration * 60000 / GameplayConstants.GameTicksPerSecond)
                {
                    _entityManager.RemoveInactiveManagers();
                    _secretData = 0;
                }
            }
            GameStateUpdate?.Invoke(this, new GameStateUpdateEvent
            {
                GameState = $"Game Ending..."
            });
        }
    }
}
