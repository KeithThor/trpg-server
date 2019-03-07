using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;

namespace TRPGGame.EventArgs
{
    public class WorldStateChangedArgs
    {
        public IDictionary<int, IEnumerable<WorldEntity>> ChangedEntities { get; set; }
    }
}
