using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities
{
    public class Map : IReadOnlyMap
    {
        public int Id { get; set; }
        public IReadOnlyList<IReadOnlyList<IReadOnlyMapTile>> MapData { get; set; }
        public IEnumerable<IReadOnlyMapTile> UniqueTiles { get; set; }
        public IEnumerable<int> MapConnections { get; set; }
    }
}