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
    public class EnemyFormationRepository : IRepository<EnemyFormationTemplate>
    {
        private readonly IRepository<EnemyEntityBase> _enemyEntityBaseRepo;
        private List<EnemyFormationTemplate> _formationTemplates;

        public EnemyFormationRepository(IRepository<EnemyEntityBase> enemyEntityBaseRepo)
        {
            _enemyEntityBaseRepo = enemyEntityBaseRepo;
        }

        public async Task<IEnumerable<EnemyFormationTemplate>> GetDataAsync()
        {
            if (_formationTemplates == null)
            {
                await LoadDataAsync();
            }
            return _formationTemplates;
        }

        private async Task LoadDataAsync()
        {
            using (var reader = new StreamReader(DataConstants.AssemblyLocation + "/Data/EnemyFormations.json"))
            {
                _formationTemplates = new List<EnemyFormationTemplate>();
                var entityData = await _enemyEntityBaseRepo.GetDataAsync();
                JContainer templateList = JsonConvert.DeserializeObject<JContainer>(reader.ReadToEnd());
                foreach (var tObject in templateList)
                {
                    var template = tObject.ToObject<EnemyFormationTemplate>();

                    var entityBases = new List<EnemyEntityBase[]>();
                    if (tObject["entityBaseIds"] != null)
                    {
                        var entityBaseIds = tObject["entityBaseIds"].ToObject<int?[][]>();
                        foreach (var row in entityBaseIds)
                        {
                            var fRow = new EnemyEntityBase[row.Length];
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
