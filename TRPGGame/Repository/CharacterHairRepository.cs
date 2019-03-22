using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TRPGGame.Entities.Data;

namespace TRPGGame.Repository
{
    public class CharacterHairRepository : IRepository<CharacterHair>
    {
        private IEnumerable<CharacterHair> _characterHair;

        public async Task<IEnumerable<CharacterHair>> GetDataAsync()
        {
            if (_characterHair == null)
            {
                await LoadDataAsync();
            }
            return _characterHair;
        }

        private async Task LoadDataAsync()
        {
            using (var reader = new StreamReader(DataConstants.AssemblyLocation + "/Data/Characters/Hair.json"))
            {
                _characterHair = JsonConvert.DeserializeObject<List<CharacterHair>>(await reader.ReadToEndAsync());
            }
        }
    }
}
