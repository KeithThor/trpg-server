using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities
{
    public class Category : IReadOnlyCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> IconUris { get; set; }
    }
}
