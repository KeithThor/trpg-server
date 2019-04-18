using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;
using TRPGGame.Managers;
using TRPGGame.Repository;
using TRPGShared;

namespace TRPGGame.Services
{
    /// <summary>
    /// Factory responsible for creating and populating Formations.
    /// </summary>
    public class FormationFactory : IFormationFactory
    {
        private readonly CombatEntityManager _combatEntityManager;
        private readonly ICombatEntityFactory _combatEntityFactory;
        private readonly IRepository<EnemyEntityBase> _entityBaseRepo;

        public FormationFactory(CombatEntityManager combatEntityManager,
                                ICombatEntityFactory combatEntityFactory,
                                IRepository<EnemyEntityBase> entityBaseRepo)
        {
            _combatEntityManager = combatEntityManager;
            _combatEntityFactory = combatEntityFactory;
            _entityBaseRepo = entityBaseRepo;
        }

        // Todo: Remove class id setters, saving object into database assigns the object a unique id
        private static int _id = 1;

        /// <summary>
        /// Creates a Formation from a given FormationTemplate.
        /// </summary>
        /// <param name="template">The template to use to create the Formation.</param>
        /// <returns></returns>
        public Formation Create(FormationTemplate template)
        {
            if (template.Positions == null) return null;
            if (template.Positions.Length != GameplayConstants.MaxFormationRows) return null;
            if (template.Positions.GetTotalSize() != GameplayConstants.MaxFormationSize) return null;

            var ids = new List<int>();
            foreach (var row in template.Positions)
            {
                if (row.Length != GameplayConstants.MaxFormationColumns) return null;

                foreach (var id in row)
                {
                    if (id != null)
                    {
                        ids.Add(id.Value);
                    }
                }
            }

            var entities = _combatEntityManager.GetModifiableEntities()
                                               .Where(entity => ids.Contains(entity.Id));

            var positions = new CombatEntity[GameplayConstants.MaxFormationRows][];
            for (int i = 0; i < GameplayConstants.MaxFormationRows; i++)
            {
                var rowPositions = new CombatEntity[GameplayConstants.MaxFormationColumns];
                for (int j = 0; j < GameplayConstants.MaxFormationColumns; j++)
                {
                    if (template.Positions[i][j] == null)
                    {
                        rowPositions[j] = null;
                    }
                    else
                    {
                        rowPositions[j] = entities.FirstOrDefault(e => e.Id == template.Positions[i][j]);
                    }
                }
                positions[i] = rowPositions;
            }

            return new Formation
            {
                Id = _id++,
                OwnerId = template.OwnerId,
                Positions = positions,
                LeaderId = template.LeaderId,
                Name = template.Name
            };
        }

        /// <summary>
        /// Creates a Formation from an EnemyFormationTemplate asynchronously.
        /// </summary>
        /// <param name="template">The EnemyFormationTemplate used to create the Formation.</param>
        /// <returns></returns>
        public async Task<Formation> CreateAsync(EnemyFormationTemplate template)
        {
            var basesData = await _entityBaseRepo.GetDataAsync();
            var matchingBases = basesData.Where(b => template.EntityBaseIds.AnyTwoD(id => id.HasValue && id.Value == b.Id));

            var positions = new CombatEntity[GameplayConstants.MaxFormationRows][];
            int leaderId = -1;
            for (int i = 0; i < template.EntityBaseIds.Length; i++)
            {
                positions[i] = new CombatEntity[GameplayConstants.MaxFormationColumns];
                for (int j = 0; j < template.EntityBaseIds[i].Length; j++)
                {
                    if (template.EntityBaseIds[i][j].HasValue)
                    {
                        var matchingBase = matchingBases.First(b => b.Id == template.EntityBaseIds[i][j].Value);
                        var entity = _combatEntityFactory.Create(matchingBase);
                        positions[i][j] = entity;
                        if (matchingBase.Id == template.LeaderId && leaderId == -1) leaderId = entity.Id;
                    }
                    else positions[i][j] = null;
                }
            }

            return new Formation
            {
                Id = _id,
                LeaderId = leaderId,
                Name = template.Name,
                OwnerId = GameplayConstants.AiId,
                Positions = positions
            };
        }
    }
}
