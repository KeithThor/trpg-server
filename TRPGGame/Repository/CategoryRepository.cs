using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;

namespace TRPGGame.Repository
{
    public class CategoryRepository : IRepository<Category>
    {
        private IEnumerable<Category> _categories;

        public async Task<IEnumerable<Category>> GetDataAsync()
        {
            if (_categories == null)
            {
                await LoadDataAsync();
            }
            return _categories;
        }

        private async Task LoadDataAsync()
        {
            using (var reader = new StreamReader(DataConstants.AssemblyLocation + "/Data/Categories.json"))
            {
                _categories = JsonConvert.DeserializeObject<List<Category>>(await reader.ReadToEndAsync());
            }
        }
    }
}
