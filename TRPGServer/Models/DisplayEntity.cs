using System.Collections.Generic;

namespace TRPGServer.Models
{
    public class DisplayEntity
    {
        public int Id { get; set; }
        public IEnumerable<string> IconUris { get; set; }
        public string Name { get; set; }
    }
}
