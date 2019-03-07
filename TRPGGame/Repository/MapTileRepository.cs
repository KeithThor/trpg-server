using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TRPGGame.Entities;

namespace TRPGGame.Repository
{
    public class MapTileRepository : IRepository<MapTile>
    {
        private List<MapTile> _mapTiles;

        public async Task<IEnumerable<MapTile>> GetDataAsync()
        {
            if (_mapTiles == null)
            {
                await LoadDataAsync();
            }

            return _mapTiles;
        }

        private async Task LoadDataAsync()
        {
            using (var reader = new StreamReader(DataConstants.AssemblyLocation + "/Data/MapTiles.json"))
            {
                _mapTiles = JsonConvert.DeserializeObject<List<MapTile>>(await reader.ReadToEndAsync());
            }
        }
    }
}
