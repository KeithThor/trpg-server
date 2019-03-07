using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.EventArgs
{
    public class GameStateUpdateEvent : System.EventArgs
    {
        public string GameState { get; set; }
    }
}
