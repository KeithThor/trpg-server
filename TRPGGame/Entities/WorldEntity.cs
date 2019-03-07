using System;
using System.Collections.Generic;
using System.Text;
using TRPGShared;

namespace TRPGGame.Entities
{
    public class WorldEntity : IReadOnlyWorldEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid OwnerGuid { get; set; }
        public string IconUri { get; set; }
        public Coordinate Position { get; set; }
        public int CurrentMapId { get; set; }
    }
}
