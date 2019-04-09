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
    public class ClassTemplateRepository : IRepository<ClassTemplate>, IRepository<IReadOnlyClassTemplate>
    {
        private readonly IRepository<Item> _itemRepository;
        private readonly IRepository<Ability> _abilityRepository;
        private List<ClassTemplate> _classTemplates;

        public ClassTemplateRepository(IRepository<Item> itemRepository,
                                       IRepository<Ability> abilityRepository)
        {
            _itemRepository = itemRepository;
            _abilityRepository = abilityRepository;
        }

        public async Task<IEnumerable<ClassTemplate>> GetDataAsync()
        {
            if (_classTemplates == null) await LoadDataAsync();
            return _classTemplates;
        }

        async Task<IEnumerable<IReadOnlyClassTemplate>> IRepository<IReadOnlyClassTemplate>.GetDataAsync()
        {
            if (_classTemplates == null) await LoadDataAsync();
            return _classTemplates;
        }

        private async Task LoadDataAsync()
        {
            using (var reader = new StreamReader(DataConstants.AssemblyLocation + "/Data/ClassTemplates.json"))
            {
                _classTemplates = new List<ClassTemplate>();
                var abilityData = await _abilityRepository.GetDataAsync();
                var items = await _itemRepository.GetDataAsync();
                JContainer templatesAsList = JsonConvert.DeserializeObject<JContainer>(reader.ReadToEnd());
                foreach (var templateObject in templatesAsList)
                {
                    var template = templateObject.ToObject<ClassTemplate>();
                    if (templateObject["equippedItemIds"] != null)
                    {
                        var itemIds = templateObject["equippedItemIds"].ToObject<IEnumerable<int>>();
                        var foundItems = items.Where(item => itemIds.Contains(item.Id)).ToList();
                        template.EquippedItems = foundItems;
                    }
                    
                    if (templateObject["abilityIds"] != null)
                    {
                        var equippedAbilityIds = templateObject["abilityIds"].ToObject<List<int>>();
                        template.Abilities = abilityData.Where(ability => equippedAbilityIds.Contains(ability.Id)).ToList();
                    }

                    _classTemplates.Add(template);
                }
            }
        }
    }
}
