using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities;
using TRPGGame.Entities.Data;
using TRPGGame.Managers;

namespace TRPGGame.Services
{
    /// <summary>
    /// Factory responsible for creating instances of AiEntityManagers.
    /// </summary>
    public class AiEntityManagerFactory
    {
        /// <summary>
        /// Creates a new AiEntityManager from the provided WorldEntity and IMapManager instances.
        /// </summary>
        /// <param name="entity">The WorldEntity the created AiEntityManager will control.</param>
        /// <param name="mapManager">The MapManager for the map that the WorldEntity the AiEntityManager controls exists in.</param>
        /// <param name="mapBattleManager">The MapBattleManager for the map that the WorldEntity the AiEntityManager controls
        /// exists in.</param>
        /// <param name="spawnEntityData">An object containing spawn data for the type of WorldEntity the created manager controls.</param>
        /// <returns></returns>
        public AiEntityManager Create(WorldEntity entity,
                                      IMapManager mapManager,
                                      IMapBattleManager mapBattleManager,
                                      SpawnEntityData spawnEntityData)
        {
            return new AiEntityManager(entity, mapManager, mapBattleManager, spawnEntityData);
        }
    }
}
