using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRPGGame.Data;
using TRPGGame.Entities;

namespace TRPGGame.Services
{
    /// <summary>
    /// Factory responsible for creating instances of WorldEntities.
    /// </summary>
    public class WorldEntityFactory
    {
        public WorldEntityFactory()
        {
            _id = 1;
        }

        private readonly WorldEntityDbContext _dbContext;

        public WorldEntityFactory(WorldEntityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private static int _id;

        [Obsolete]
        public async Task<WorldEntity> CreateAsync(Guid ownerId)
        {
            var foundEntity = await _dbContext.PlayerEntities.Where(entity => entity.OwnerGuid == ownerId)
                                                             .FirstOrDefaultAsync();
            if (foundEntity != null)
            {
                return foundEntity;
            }
            else
            {
                var entity = new WorldEntity
                {
                    Id = _id++,
                    OwnerGuid = ownerId,
                    Name = "User",
                    CurrentMapId = 1,
                    IconUris = new Entities.Data.CharacterIconSet
                    {
                        BodyIconUri = "images/player/base/human_male.png",
                        HairIconUri = "images/player/hair/brown_1.png"
                    },
                    Position = new TRPGShared.Coordinate() { PositionX = 1, PositionY = 1}
                };
                await _dbContext.PlayerEntities.AddAsync(entity);
                return entity;
            }
        }

        /// <summary>
        /// Creates a WorldEntity using the data given from a CombatEntity.
        /// </summary>
        /// <param name="entity">The CombatEntity to use to construct the WorldEntity.</param>
        /// <returns></returns>
        public async Task<WorldEntity> CreateAsync(CombatEntity entity)
        {
            var foundEntity = await _dbContext.PlayerEntities.Where(e => e.OwnerGuid == entity.OwnerId)
                                                             .FirstOrDefaultAsync();
            if (foundEntity != null)
            {
                return foundEntity;
            }
            else
            {
                var wEntity = new WorldEntity
                {
                    Id = _id++,
                    OwnerGuid = entity.OwnerId,
                    Name = entity.OwnerName,
                    CurrentMapId = 1,
                    IconUris = entity.IconUris,
                    Position = new TRPGShared.Coordinate() { PositionX = 1, PositionY = 1 }
                };
                await _dbContext.PlayerEntities.AddAsync(wEntity);
                return wEntity;
            }
        }
    }
}
