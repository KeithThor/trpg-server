using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TRPGGame.Entities.Data;

namespace TRPGGame.Repository
{
    public class CharacterBaseRepository : IRepository<CharacterBase>
    {
        private IEnumerable<CharacterBase> _characterBases;

        public async Task<IEnumerable<CharacterBase>> GetDataAsync()
        {
            if (_characterBases == null)
            {
                await LoadDataAsync();
            }
            return _characterBases;
        }

        private async Task LoadDataAsync()
        {
            using (var reader = new StreamReader(DataConstants.AssemblyLocation + "/Data/Characters/Base.json"))
            {
                _characterBases = JsonConvert.DeserializeObject<List<CharacterBase>>(await reader.ReadToEndAsync());
            }
        }
    }
}
