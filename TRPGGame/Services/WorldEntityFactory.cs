using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRPGGame.Data;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;
using TRPGGame.Managers;
using TRPGShared;

namespace TRPGGame.Services
{
    /// <summary>
    /// Factory responsible for creating instances of WorldEntities.
    /// </summary>
    public class WorldEntityFactory : IWorldEntityFactory
    {
        public WorldEntityFactory(IFormationFactory formationFactory,
                                  IFormationManager formationManager)
        {
            _id = 1;
            _formationFactory = formationFactory;
            _formationManager = formationManager;
        }
        private readonly IFormationFactory _formationFactory;
        private readonly IFormationManager _formationManager;

        private static int _id;

        /// <summary>
        /// Creates a new WorldEntity using the id of the formation that this WorldEntity represents and the id of
        /// the player who will own this Entity.
        /// </summary>
        /// <param name="playerId">The id of the player who will own the new entity.</param>
        /// <param name="formationId">The id of the formation that this entity will represent.</param>
        /// <returns></returns>
        public WorldEntity Create(Guid playerId, int formationId)
        {
            var formation = _formationManager.GetFormation(playerId, formationId);
            if (formation == null) return null;
            var leader = formation.Positions.FirstOrDefaultTwoD(entity => entity != null && entity.Id == formation.LeaderId);

            return new WorldEntity
            {
                Id = _id++,
                OwnerGuid = playerId,
                ActiveFormation = (Formation)formation,
                CurrentMapId = GameplayConstants.StartingMapId,
                Position = GameplayConstants.StartingPosition,
                IconUris = (CharacterIconSet)leader.IconUris,
                Name = leader.OwnerName
            };
        }

        /// <summary>
        /// Creates a new WorldEntity using the id of the formation that this WorldEntity represents and the id of
        /// the player who will own this Entity. Will also use the location of the old WorldEntity.
        /// </summary>
        /// <param name="playerId">The id of the player who will own the new entity.</param>
        /// <param name="formationId">The id of the formation that this entity will represent.</param>
        /// <returns></returns>
        public WorldEntity Create(Guid playerId, int formationId, WorldEntity oldEntity)
        {
            var formation = _formationManager.GetFormation(playerId, formationId);
            if (formation == null) return null;
            var leader = formation.Positions.FirstOrDefaultTwoD(entity => entity != null && entity.Id == formation.LeaderId);

            return new WorldEntity
            {
                Id = oldEntity.Id,
                OwnerGuid = playerId,
                ActiveFormation = (Formation)formation,
                CurrentMapId = oldEntity.CurrentMapId,
                Position = oldEntity.Position,
                IconUris = (CharacterIconSet)leader.IconUris,
                Name = leader.OwnerName
            };
        }

        /// <summary>
        /// Creates an ai-controlled WorldEntity from an EnemyFormationTemplate.
        /// </summary>
        /// <param name="template">The template to use to create the WorldEntity.</param>
        /// <returns></returns>
        public WorldEntity Create(AiFormationTemplate template)
        {
            var formation = _formationFactory.Create(template);

            return new WorldEntity
            {
                Id = _id++,
                IconUris = new CharacterIconSet(template.IconUris),
                OwnerGuid = GameplayConstants.AiId,
                Name = template.Name,
                ActiveFormation = formation
            };
        }
    }
}
