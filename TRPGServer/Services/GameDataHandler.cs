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
    public class GameDataHandler
    {
        private readonly IHubContext<GameDataHub> _gameDataHub;
        private readonly Game _game;

        public GameDataHandler(IHubContext<GameDataHub> gameDataHub,
                               Game game)
        {
            _gameDataHub = gameDataHub;
            _game = game;
            _game.GameStateUpdate += OnGameStateUpdate;
        }

        private void OnGameStateUpdate(object sender, GameStateUpdateEvent e)
        {
            _gameDataHub.Clients.All.SendAsync("updateGameState", e.GameState);
        }
    }
}
