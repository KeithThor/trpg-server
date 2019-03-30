using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Services;

namespace TRPGGame.Managers
{
    public class CombatEntityManager : ICombatEntityManager
    {
        private readonly CombatEntityFactory _combatEntityFactory;

        // Todo: switch in memory list for queryable from dbcontext
        private readonly List<CombatEntity> _combatEntities;

        public CombatEntityManager(CombatEntityFactory combatEntityFactory)
        {
            _combatEntityFactory = combatEntityFactory;
            _combatEntities = new List<CombatEntity>();
        }

        /// <summary>
        /// Returns an IEnumerable of CombatEntities that can be modified.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<CombatEntity> GetModifiableEntities()
        {
            return _combatEntities;
        }

        /// <summary>
        /// Returns an IEnumerable of IReadOnlyCombatEntity that contains all the combat entities in memory.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IReadOnlyCombatEntity> GetEntities()
        {
            return _combatEntities;
        }

        /// <summary>
        /// Creates and returns a CombatEntity from a given template.
        /// </summary>
        /// <param name="template">The template used to insert spawn the CombatEntity.</param>
        /// <returns>Returns a read-only reference to the CombatEntity if the operation was successful. Else returns null.</returns>
        public async Task<IReadOnlyCombatEntity> CreateAsync(CharacterTemplate template)
        {
            var entity = await _combatEntityFactory.CreateAsync(template);
            _combatEntities.Add(entity);

            return entity;
        }

        /// <summary>
        /// Updates a CombatEntity using a given template.
        /// </summary>
        /// <param name="template">The template used to update the CombatEntity.</param>
        /// <returns>Returns a read-only reference to the CombatEntity if the operation was successful. Else returns null.</returns>
        public async Task<IReadOnlyCombatEntity> UpdateAsync(CharacterTemplate template)
        {
            int index = _combatEntities.FindIndex(e => e.Id == template.EntityId);
            if (index == -1) return null;

            var modified = await _combatEntityFactory.UpdateAsync(_combatEntities[index], template);
            if (modified == null) return null;

            _combatEntities[index] = modified;
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
            var index = _combatEntities.FindIndex(e => e.Id == entityId);
            if (index == -1) return false;
            if (_combatEntities[index].OwnerId != playerId) return false;

            _combatEntities.RemoveAt(index);
            return true;
        }
    }
}
