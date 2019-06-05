using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;

namespace TRPGGame.Repository
{
    public class AiEntityBaseRepository : IRepository<AiEntityBase>
    {
        private readonly IRepository<StatusEffect> _statusEffectRepo;
        private readonly IRepository<Ability> _abilityRepo;
        private List<AiEntityBase> _bases;

        public AiEntityBaseRepository(IRepository<StatusEffect> statusEffectRepo,
                                         IRepository<Ability> abilityRepo)
        {
            _statusEffectRepo = statusEffectRepo;
            _abilityRepo = abilityRepo;
        }

        public async Task<IEnumerable<AiEntityBase>> GetDataAsync()
        {
            if (_bases == null) await LoadDataAsync();
            return _bases;
        }

        private async Task LoadDataAsync()
        {
            using (var reader = new StreamReader(DataConstants.AssemblyLocation + "/Data/EnemyEntityBases.json"))
            {
                _bases = new List<AiEntityBase>();
                var statusEffects = await _statusEffectRepo.GetDataAsync();
                var abilityData = await _abilityRepo.GetDataAsync();
                JContainer basesAsList = JsonConvert.DeserializeObject<JContainer>(reader.ReadToEnd());
                foreach (var baseObject in basesAsList)
                {
                    var entityBase = baseObject.ToObject<AiEntityBase>();

                    var statuses = new List<StatusEffect>();
                    List<int> statusIds = null;
                    if (baseObject["statusEffectIds"] != null)
                    {
                        statusIds = baseObject["statusEffectIds"].ToObject<List<int>>();
                        foreach (var id in statusIds)
                        {
                            var foundStatus = statusEffects.FirstOrDefault(se => se.Id == id);
                            if (foundStatus != null) statuses.Add(foundStatus);
                        }
                        entityBase.StatusEffects = statuses;
                    }

                    var abilities = new List<Ability>();
                    List<int> abilityIds;
                    if (baseObject["abilityIds"] != null)
                    {
                        abilityIds = baseObject["abilityIds"].ToObject<List<int>>();
                        foreach (var id in abilityIds)
                        {
                            var foundAbility = abilityData.FirstOrDefault(se => se.Id == id);
                            if (foundAbility != null) abilities.Add(foundAbility);
                        }
                        entityBase.Abilities = abilities;
                    }

                    _bases.Add(entityBase);
                }
            }
        }
    }
}
