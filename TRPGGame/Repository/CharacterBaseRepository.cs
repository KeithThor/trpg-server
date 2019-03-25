using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                var bases = new List<CharacterBase>();
                JContainer basesAsList = JsonConvert.DeserializeObject<JContainer>(reader.ReadToEnd());
                foreach (var baseObject in basesAsList)
                {
                    var chrBase = baseObject.ToObject<CharacterBase>();
                    if (baseObject["maxStats"] != null)
                    {
                        chrBase.MaxStats = baseObject["maxStats"].ToObject<CharacterStats>();
                    }
                    else
                    {
                        chrBase.MaxStats = new CharacterStats();
                    }
                    if (baseObject["bonusStats"] != null)
                    {
                        chrBase.BonusStats = baseObject["bonusStats"].ToObject<CharacterStats>();
                    }
                    else
                    {
                        chrBase.MaxStats = new CharacterStats();
                    }
                    bases.Add(chrBase);
                }
                _characterBases = bases;
            }
        }
    }
}
