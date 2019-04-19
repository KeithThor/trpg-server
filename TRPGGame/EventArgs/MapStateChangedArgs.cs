using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;
using TRPGShared;

namespace TRPGGame.EventArgs
{
    public class MapStateChangedArgs
    {
        public Dictionary<WorldEntity, Coordinate> Entities;
        public IReadOnlyList<Guid> ConnectedPlayers { get; set; }
    }
}
