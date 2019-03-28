using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TRPGGame.Entities;

namespace TRPGGame.Managers
{
    /// <summary>
    /// Interface for a manager that is responsible for dealing with CRUD operations for CombatEntitys.
    /// </summary>
    public interface ICombatEntityManager
    {
        /// <summary>
        /// Creates a CombatEntity from a given template asynchronously.
        /// </summary>
        /// <param name="template">The template to use as a basis for creating a CombatEntity.</param>
        /// <returns></returns>
        Task<IReadOnlyCombatEntity> CreateAsync(CharacterTemplate template);

        /// <summary>
        /// Deletes a CombatEntity.
        /// </summary>
        /// <param name="entityId">The id of the entity to delete.</param>
        /// <param name="playerId">THe id of the player who owns the entity.</param>
        /// <returns>Returns true if the CombatEntity was deleted.</returns>
        bool Delete(int entityId, Guid playerId);

        /// <summary>
        /// Returns all CombatEntities.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IReadOnlyCombatEntity> GetEntities();

        /// <summary>
        /// Updates a CombatEntity using a given template.
        /// </summary>
        /// <param name="template">The template to update the original CombatEntity with.</param>
        /// <returns></returns>
        Task<IReadOnlyCombatEntity> UpdateAsync(CharacterTemplate template);
    }
}