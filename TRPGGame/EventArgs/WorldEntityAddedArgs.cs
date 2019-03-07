using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;

namespace TRPGGame.EventArgs
{
    public class WorldEntityAddedArgs
    {
        public IEnumerable<IReadOnlyWorldEntity> AddedEntities { get; set; }
    }
}
