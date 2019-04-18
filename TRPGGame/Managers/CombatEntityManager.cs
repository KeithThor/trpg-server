using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Services;

namespace TRPGGame.Managers
{
    /// <summary>
    /// Manager responsible for CRUD operations concerning CombatEntities.
    /// </summary>
    public class CombatEntityManager : ICombatEntityManager
    {
        private readonly ICombatEntityFactory _combatEntityFactory;

        // Todo: switch in memory list for queryable from dbcontext
        private readonly SortedList<int, CombatEntity> _combatEntities;

        private object _lock = new object();
        public CombatEntityManager(ICombatEntityFactory combatEntityFactory)
        {
            _combatEntityFactory = combatEntityFactory;
            _combatEntities = new SortedList<int, CombatEntity>();
        }

        /// <summary>
        /// Returns an IEnumerable of CombatEntities that can be modified.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<CombatEntity> GetModifiableEntities()
        {
            lock(_lock)
            {
                return _combatEntities.Values;
            }
        }

        /// <summary>
        /// Returns an IEnumerable of IReadOnlyCombatEntity that contains all the combat entities in memory.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IReadOnlyCombatEntity> GetEntities()
        {
            lock(_lock)
            {
                return _combatEntities.Values;
            }
        }

        /// <summary>
        /// Creates and returns a CombatEntity from a given template.
        /// </summary>
        /// <param name="template">The template used to insert spawn the CombatEntity.</param>
        /// <returns>Returns a read-only reference to the CombatEntity if the operation was successful. Else returns null.</returns>
        public async Task<IReadOnlyCombatEntity> CreateAsync(CharacterTemplate template)
        {
            var entity = await _combatEntityFactory.CreateAsync(template);

            lock(_lock)
            {
                _combatEntities.Add(entity.Id, entity);
            }

            return entity;
        }

        /// <summary>
        /// Updates a CombatEntity using a given template.
        /// </summary>
        /// <param name="template">The template used to update the CombatEntity.</param>
        /// <returns>Returns a read-only reference to the CombatEntity if the operation was successful. Else returns null.</returns>
        public async Task<IReadOnlyCombatEntity> UpdateAsync(CharacterTemplate template)
        {
            if (!template.EntityId.HasValue) return null;
            CombatEntity entity;
            lock (_lock)
            {
                entity = _combatEntities.ContainsKey(template.EntityId.Value) ? _combatEntities[template.EntityId.Value] : null;
            }

            if (entity == null) return null;

            var modified = await _combatEntityFactory.UpdateAsync(entity, template);
            if (modified == null) return null;

            lock(_lock)
            {
                _combatEntities[modified.Id] = modified;
            }
            return modified;
        }

        /// <summary>
        /// Deletes a CombatEntity given the id of the entity and the id of the player the entity belongs to.
        /// </summary>
        /// <param name="entityId">The id of the CombatEntity to delete.</param>
        /// <param name="playerId">The id of the player the entity belongs to.</param>
        /// <returns>Returns true if deletion was successful.</returns>
        public bool Delete(int entityId, Guid playerId)
        {
            //var index = _combatEntities.FindIndex(e => e.Id == entityId);
            //if (index == -1) return false;
            //if (_combatEntities[index].OwnerId != playerId) return false;

            //_combatEntities.RemoveAt(index);
            //return true;
            
            // Todo: implement delete after moving data to databases
            return false;
        }
    }
}
