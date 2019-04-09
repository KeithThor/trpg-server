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
    public class ItemRepository: IRepository<Item>
    {
        private readonly IRepository<StatusEffect> _statusEffectRepo;
        private readonly IRepository<Ability> _abilityRepo;
        private readonly IRepository<ItemType> _itemTypeRepo;
        private List<Item> _items;

        public ItemRepository(IRepository<StatusEffect> statusEffectRepo,
                              IRepository<Ability> abilityRepo,
                              IRepository<ItemType> itemTypeRepo)
        {
            _statusEffectRepo = statusEffectRepo;
            _abilityRepo = abilityRepo;
            _itemTypeRepo = itemTypeRepo;
        }

        public async Task<IEnumerable<Item>> GetDataAsync()
        {
            if (_items != null) return _items;
            else
            {
                await LoadDataAsync();
                return _items;
            }
        }

        private async Task LoadDataAsync()
        {
            using (var reader = new StreamReader(DataConstants.AssemblyLocation + "/Data/Items.json"))
            {
                _items = new List<Item>();
                var statusEffects = await _statusEffectRepo.GetDataAsync();
                var abilityData = await _abilityRepo.GetDataAsync();
                var itemTypes = await _itemTypeRepo.GetDataAsync();
                JContainer itemsAsList = JsonConvert.DeserializeObject<JContainer>(reader.ReadToEnd());
                foreach (var itemObject in itemsAsList)
                {
                    var item = itemObject.ToObject<Item>();
                    if (itemObject["itemTypeId"] != null)
                    {
                        var itemTypeId = itemObject["itemTypeId"].ToObject<int>();
                        var foundItemType = itemTypes.FirstOrDefault(iT => iT.Id == itemTypeId);
                        if (foundItemType != null) item.Type = foundItemType;
                    }

                    var appliedStatusEffects = new List<StatusEffect>();
                    List<int> appliedStatusIds = null;
                    if (itemObject["appliedStatusEffectIds"] != null)
                    {
                        appliedStatusIds = itemObject["appliedStatusEffectIds"].ToObject<List<int>>();
                        foreach (var id in appliedStatusIds)
                        {
                            var foundStatus = statusEffects.FirstOrDefault(se => se.Id == id);
                            if (foundStatus != null) appliedStatusEffects.Add(foundStatus);
                        }
                        item.AppliedStatusEffects = appliedStatusEffects;
                    }

                    var selfAppliedStatusEffects = new List<StatusEffect>();
                    List<int> selfAppliedStatusIds = null;
                    if (itemObject["selfAppliedStatusEffectIds"] != null)
                    {
                        selfAppliedStatusIds = itemObject["selfAppliedStatusEffectIds"].ToObject<List<int>>();
                        foreach (var id in selfAppliedStatusIds)
                        {
                            var foundStatus = statusEffects.FirstOrDefault(se => se.Id == id);
                            if (foundStatus != null) selfAppliedStatusEffects.Add(foundStatus);
                        }
                        item.SelfAppliedStatusEffects = selfAppliedStatusEffects;
                    }

                    var abilities = new List<Ability>();
                    List<int> equippedAbilityIds;
                    if (itemObject["equippedAbilityIds"] != null)
                    {
                        equippedAbilityIds = itemObject["equippedAbilityIds"].ToObject<List<int>>();
                        foreach (var id in equippedAbilityIds)
                        {
                            var foundAbility = abilityData.FirstOrDefault(se => se.Id == id);
                            if (foundAbility != null) abilities.Add(foundAbility);
                        }
                        item.EquippedAbilities = abilities;
                    }

                    _items.Add(item);
                }
            }
        }
    }
}
