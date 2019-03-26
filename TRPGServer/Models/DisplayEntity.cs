using System.Collections.Generic;
using TRPGGame.Entities.Data;

namespace TRPGServer.Models
{
    public class DisplayEntity
    {
        public int Id { get; set; }
        public IReadOnlyCharacterIconSet IconUris { get; set; }
        public string Name { get; set; }
    }
}
