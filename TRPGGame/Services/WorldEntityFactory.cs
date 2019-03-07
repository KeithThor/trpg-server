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
                    IconUri = "images/player/base/human_male.png",
                    Position = new TRPGShared.Coordinate() { PositionX = 1, PositionY = 1}
                };
                await _dbContext.PlayerEntities.AddAsync(entity);
                return entity;
            }
        }
    }
}
