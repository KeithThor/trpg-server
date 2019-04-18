using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;

namespace TRPGGame.Services
{
    /// <summary>
    /// Factory responsible for creating new CombatEntities.
    /// </summary>
    public interface ICombatEntityFactory
    {
        /// <summary>
        /// Creates a CombatEntity using an EnemyEntityBase. The result is an ai-controlled CombatEntity.
        /// </summary>
        /// <param name="entityBase">The base to use for the resultant CombatEntity.</param>
        /// <returns></returns>
        CombatEntity Create(EnemyEntityBase entityBase);

        /// <summary>
        /// Creates a player combat entity from a given character template asynchronously.
        /// </summary>
        /// <param name="template">The template containing specifications on how to create the entity.</param>
        /// <returns>Returns the combat entity if the operation was successful, else returns null.</returns>
        Task<CombatEntity> CreateAsync(CharacterTemplate template);

        /// <summary>
        /// Updates an existing combat entity with the template given asynchronously.
        /// <para>Will return null if the operation failed.</para>
        /// </summary>
        /// <param name="template">The template to use to update the entity with.</param>
        /// <returns>Returns the modified combat entity or null if no entity was modified.</returns>
        Task<CombatEntity> UpdateAsync(CombatEntity entity, CharacterTemplate template);
    }
}