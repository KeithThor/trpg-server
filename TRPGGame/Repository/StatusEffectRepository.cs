using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;

namespace TRPGGame.Repository
{
    public class StatusEffectRepository : IRepository<StatusEffect>
    {
        private IEnumerable<StatusEffect> _statusEffects;

        public async Task<IEnumerable<StatusEffect>> GetDataAsync()
        {
            if (_statusEffects == null)
            {
                await LoadDataAsync();
            }
            return _statusEffects;
        }

        private async Task LoadDataAsync()
        {
            using (var reader = new StreamReader(DataConstants.AssemblyLocation + "/Data/StatusEffects.json"))
            {
                _statusEffects = JsonConvert.DeserializeObject<List<StatusEffect>>(await reader.ReadToEndAsync());
            }
        }
    }
}
