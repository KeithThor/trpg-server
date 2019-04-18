using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities.Data;
using TRPGShared;

namespace TRPGGame.Entities
{
    public class WorldEntity : IReadOnlyWorldEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid OwnerGuid { get; set; }
        public CharacterIconSet IconUris { get; set; }
        public Coordinate Position { get; set; }
        public int CurrentMapId { get; set; }
        public Formation ActiveFormation { get; set; }

        IReadOnlyCharacterIconSet IReadOnlyWorldEntity.IconUris => IconUris;
    }
}
