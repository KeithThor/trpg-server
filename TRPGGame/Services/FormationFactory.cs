using System;
using System.Collections.Generic;
using System.Linq;
using TRPGGame.Entities;
using TRPGGame.Managers;
using TRPGShared;

namespace TRPGGame.Services
{
    public class FormationFactory
    {
        private readonly CombatEntityManager _combatEntityManager;

        public FormationFactory(CombatEntityManager combatEntityManager)
        {
            _combatEntityManager = combatEntityManager;
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
    }
}
