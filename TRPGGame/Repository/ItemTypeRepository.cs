using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TRPGGame.Entities;

namespace TRPGGame.Repository
{
    public class ItemTypeRepository: IRepository<ItemType>
    {
        private IEnumerable<ItemType> _itemTypes;

        public async Task<IEnumerable<ItemType>> GetDataAsync()
        {
            if (_itemTypes == null)
            {
                await LoadDataAsync();
            }
            return _itemTypes;
        }

        private async Task LoadDataAsync()
        {
            using (var reader = new StreamReader(DataConstants.AssemblyLocation + "/Data/ItemTypes.json"))
            {
                _itemTypes = JsonConvert.DeserializeObject<List<ItemType>>(await reader.ReadToEndAsync());
            }
        }
    }
}
