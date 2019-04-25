using System;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;

namespace TRPGGame.Services
{
    /// <summary>
    /// Interface for a factory that creates WorldEntities.
    /// </summary>
    public interface IWorldEntityFactory
    {
        /// <summary>
        /// Creates an ai-controlled WorldEntity from an EnemyFormationTemplate.
        /// </summary>
        /// <param name="template">The template to use to create the WorldEntity.</param>
        /// <returns></returns>
        WorldEntity Create(EnemyFormationTemplate template);

        /// <summary>
        /// Creates a new WorldEntity using the id of the formation that this WorldEntity represents and the id of
        /// the player who will own this Entity.
        /// </summary>
        /// <param name="playerId">The id of the player who will own the new entity.</param>
        /// <param name="formationId">The id of the formation that this entity will represent.</param>
        /// <returns></returns>
        WorldEntity Create(Guid playerId, int formationId);

        /// <summary>
        /// Creates a new WorldEntity using the id of the formation that this WorldEntity represents and the id of
        /// the player who will own this Entity. Will also use the location of the old WorldEntity.
        /// </summary>
        /// <param name="playerId">The id of the player who will own the new entity.</param>
        /// <param name="formationId">The id of the formation that this entity will represent.</param>
        /// <returns></returns>
        WorldEntity Create(Guid playerId, int formationId, WorldEntity oldEntity);
    }
}