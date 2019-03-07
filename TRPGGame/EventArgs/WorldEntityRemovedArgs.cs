using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.EventArgs
{
    public class WorldEntityRemovedArgs
    {
        public IEnumerable<int> RemovedEntityIds { get; set; }
    }
}
