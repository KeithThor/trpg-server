using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRPGGame.Entities.Data;

namespace TRPGGame.Repository
{
    public class AiFormationRepository : IRepository<AiFormationTemplate>
    {
        private readonly IRepository<AiEntityBase> _aiEntityBaseRepo;
        private List<AiFormationTemplate> _formationTemplates;

        public AiFormationRepository(IRepository<AiEntityBase> aiEntityBaseRepo)
        {
            _aiEntityBaseRepo = aiEntityBaseRepo;
        }

        public async Task<IEnumerable<AiFormationTemplate>> GetDataAsync()
        {
            if (_formationTemplates == null)
            {
                await LoadDataAsync();
            }
            return _formationTemplates;
        }

        private async Task LoadDataAsync()
        {
            using (var reader = new StreamReader(DataConstants.AssemblyLocation + "/Data/AiFormations.json"))
            {
                _formationTemplates = new List<AiFormationTemplate>();
                var entityData = await _aiEntityBaseRepo.GetDataAsync();
                JContainer templateList = JsonConvert.DeserializeObject<JContainer>(reader.ReadToEnd());
                foreach (var tObject in templateList)
                {
                    var template = tObject.ToObject<AiFormationTemplate>();

                    var entityBases = new List<AiEntityBase[]>();
                    if (tObject["entityBaseIds"] != null)
                    {
                        var entityBaseIds = tObject["entityBaseIds"].ToObject<int?[][]>();
                        foreach (var row in entityBaseIds)
                        {
                            var fRow = new AiEntityBase[row.Length];
                            for (int i = 0; i < row.Length; i++)
                            {
                                if (row[i].HasValue)
                                {
                                    var foundEntity = entityData.FirstOrDefault(e => e.Id == row[i].Value);
                                    if (foundEntity != null) fRow[i] = foundEntity;
                                }
                                else fRow[i] = null;
                            }
                            entityBases.Add(fRow);
                        }
                        template.EntityBases = entityBases.ToArray();
                    }

                    _formationTemplates.Add(template);
                }
            }
        }
    }
}
