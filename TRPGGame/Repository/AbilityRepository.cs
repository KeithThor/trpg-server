using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRPGGame.Entities;

namespace TRPGGame.Repository
{
    public class AbilityRepository: IRepository<Ability>
    {
        private readonly IRepository<StatusEffect> _statusEffectRepo;
        private readonly IRepository<Category> _categoryRepo;
        private List<Ability> _abilities;

        public AbilityRepository(IRepository<StatusEffect> statusEffectRepo,
                                 IRepository<Category> categoryRepo)
        {
            _statusEffectRepo = statusEffectRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<IEnumerable<Ability>> GetDataAsync()
        {
            if (_abilities != null) return _abilities;
            else
            {
                await LoadDataAsync();
                return _abilities;
            }
        }

        private async Task LoadDataAsync()
        {
            using (var reader = new StreamReader(DataConstants.AssemblyLocation + "/Data/Abilities.json"))
            {
                _abilities = new List<Ability>();
                var statusEffects = await _statusEffectRepo.GetDataAsync();
                var categories = await _categoryRepo.GetDataAsync();
                JContainer abilitiesAsList = JsonConvert.DeserializeObject<JContainer>(reader.ReadToEnd());
                foreach (var abilityObject in abilitiesAsList)
                {
                    var ability = abilityObject.ToObject<Ability>();
                    if (abilityObject["categoryId"] != null)
                    {
                        var categoryId = abilityObject["categoryId"].ToObject<int>();
                        var foundCategory = categories.FirstOrDefault(c => c.Id == categoryId);
                        if (foundCategory != null) ability.Category = foundCategory;
                    }

                    var appliedStatusEffects = new List<StatusEffect>();
                    List<int> appliedStatusIds = null;
                    if (abilityObject["appliedStatusEffectIds"] != null)
                    {
                        appliedStatusIds = abilityObject["appliedStatusEffectIds"].ToObject<List<int>>();
                        foreach (var id in appliedStatusIds)
                        {
                            var foundStatus = statusEffects.FirstOrDefault(se => se.Id == id);
                            if (foundStatus != null) appliedStatusEffects.Add(foundStatus);
                        }
                        ability.AppliedStatusEffects = appliedStatusEffects;
                    }

                    var selfAppliedStatusEffects = new List<StatusEffect>();
                    List<int> selfAppliedStatusIds = null;
                    if (abilityObject["selfAppliedStatusEffectIds"] != null)
                    {
                        selfAppliedStatusIds = abilityObject["selfAppliedStatusEffectIds"].ToObject<List<int>>();
                        foreach (var id in selfAppliedStatusIds)
                        {
                            var foundStatus = statusEffects.FirstOrDefault(se => se.Id == id);
                            if (foundStatus != null) selfAppliedStatusEffects.Add(foundStatus);
                        }
                        ability.SelfAppliedStatusEffects = selfAppliedStatusEffects;
                    }
                    _abilities.Add(ability);
                }
            }
        }
    }
}
