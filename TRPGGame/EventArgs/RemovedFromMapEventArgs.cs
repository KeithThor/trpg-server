using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities.Data;

namespace TRPGGame.EventArgs
{
    public class RemovedFromMapEventArgs
    {
        public SpawnEntityData SpawnData { get; set; }
    }
}
