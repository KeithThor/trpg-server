using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRPGGame;

namespace TRPGServer.Hubs
{
    public class GameDataHub: Hub
    {
        private readonly Game _game;

        public GameDataHub(Game game)
        {
            _game = game;
        }

        public Task SendMessage(string message)
        {
            return Clients.All.SendAsync("receiveMessage", message);
        }
    }
}
