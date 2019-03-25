using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Services;

namespace TRPGGame.Managers
{
    public class CombatEntityManager
    {
        private readonly PlayerEntityFactory _playerEntityFactory;

        // Todo: switch in memory list for queryable from dbcontext
        private readonly List<CombatEntity> _combatEntities;

        public CombatEntityManager(PlayerEntityFactory playerEntityFactory)
        {
            _playerEntityFactory = playerEntityFactory;
            _combatEntities = new List<CombatEntity>();
        }

        public async Task<IReadOnlyCombatEntity> CreateAsync(CharacterTemplate template)
        {
            var entity = await _playerEntityFactory.CreateAsync(template);
            _combatEntities.Add(entity);

            return entity;
        }

        public async Task<IReadOnlyCombatEntity> UpdateAsync(CharacterTemplate template)
        {
            int index = _combatEntities.FindIndex(e => e.Id == template.EntityId);
            if (index == -1) return null;

            var modified = await _playerEntityFactory.UpdateAsync(_combatEntities[index], template);
            if (modified == null) return null;

            _combatEntities[index] = modified;
            return modified;
        }

        public bool Delete(int entityId, Guid playerId)
        {
            var index = _combatEntities.FindIndex(e => e.Id == entityId);
            if (index == -1) return false;
            if (_combatEntities[index].OwnerId != playerId) return false;

            _combatEntities.RemoveAt(index);
            return true;
        }
    }
}
