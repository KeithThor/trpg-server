using System;
using System.Collections.Generic;
using TRPGGame.Entities.Data;
using TRPGGame.EventArgs;
using TRPGGame.Services;

namespace TRPGGame.Managers
{
    /// <summary>
    /// Manager responsible for creating and handling AiEntityManagers for a given map.
    /// </summary>
    public class MapEntityManager
    {
        /// <summary>
        /// A wrapper around a SpawnEntityData object that keeps track of when an entity was removed.
        /// </summary>
        private class RespawnData
        {
            /// <summary>
            /// The spawn data this RespawnData wraps around.
            /// </summary>
            public SpawnEntityData SpawnData { get; set; }

            /// <summary>
            /// The time at which the entity that corresponds with the SpawnData was removed from a map.
            /// </summary>
            public DateTime DeathTime { get; set; }
        }

        private readonly IMapManager _mapManager;
        private readonly IMapBattleManager _mapBattleManager;
        private readonly IWorldEntityFactory _worldEntityFactory;
        private readonly AiEntityManagerFactory _aiEntityManagerFactory;
        private readonly IReadOnlyList<SpawnEntityData> _spawnData;

        private int Ticks = 0;
        private readonly int _ticksPerSecond = 1000 / GameplayConstants.GameTickInMilliseconds;
        private readonly Queue<RespawnData> _respawnQueue = new Queue<RespawnData>();
        private readonly List<AiEntityManager> _managers = new List<AiEntityManager>();

        public MapEntityManager(IMapManager mapManager,
                                IMapBattleManager mapBattleManager,
                                IWorldEntityFactory worldEntityFactory,
                                AiEntityManagerFactory aiEntityManagerFactory)
        {
            _mapManager = mapManager;
            _mapBattleManager = mapBattleManager;
            _mapManager.GameTick += OnGameTick;
            _worldEntityFactory = worldEntityFactory;
            _aiEntityManagerFactory = aiEntityManagerFactory;
            _spawnData = _mapManager.Map.SpawnData;

            Initialize();
        }

        /// <summary>
        /// Called on startup to spawn the maximum number of ai-controlled WorldEntities as is
        /// specified for the current map.
        /// </summary>
        private void Initialize()
        {
            foreach (var spawnData in _spawnData)
            {
                for (int i = 0; i < spawnData.MaxEntities; i++)
                {
                    SpawnEntity(spawnData);
                }
            }
        }

        /// <summary>
        /// Called on every tick of the game. Allows AiEntityManagers to perform actions and respawns
        /// removed Entities.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnGameTick(object sender, GameTickEventArgs args)
        {
            Ticks++;
            if (Ticks >= _ticksPerSecond)
            {
                Ticks = 0;
                foreach (var manager in _managers)
                {
                    manager.OnGameTick();
                }

                RespawnEntity();
            }
        }

        /// <summary>
        /// Creates a new AiEntityManager and WorldEntity and inserts the WorldEntity into the map.
        /// </summary>
        private void RespawnEntity()
        {
            if (_respawnQueue.Count == 0) return;

            var respawnData = _respawnQueue.Peek();
            if (DateTime.Now >= respawnData.DeathTime.AddSeconds(respawnData.SpawnData.RespawnTime))
            {
                _respawnQueue.Dequeue();
                SpawnEntity(respawnData.SpawnData);
            }
        }

        /// <summary>
        /// Spawns a new WorldEntity and creates a new AiEntityManager to control it. Returns the AiEntityManager.
        /// </summary>
        /// <param name="spawnData">Contains data used to create the spawn WorldEntity.</param>
        /// <returns></returns>
        private AiEntityManager SpawnEntity(SpawnEntityData spawnData)
        {
            var spawnedEntity = _worldEntityFactory.Create(spawnData.FormationTemplate);
            var manager = _aiEntityManagerFactory.Create(spawnedEntity, _mapManager, _mapBattleManager, spawnData);
            manager.RemovedFromMap += OnEntityRemovedFromMap;
            _managers.Add(manager);
            return manager;
        }

        /// <summary>
        /// Called whenever a WorldEntity is removed from the map. Retrieves the SpawnData from the AiEntityManager
        /// who managed the removed entity and enqueues that data into the respawn queue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnEntityRemovedFromMap(object sender, RemovedFromMapEventArgs args)
        {
            var manager = sender as AiEntityManager;

            manager.RemovedFromMap -= OnEntityRemovedFromMap;
            _managers.Remove(manager);

            _respawnQueue.Enqueue(new RespawnData
            {
                SpawnData = args.SpawnData,
                DeathTime = DateTime.Now
            });
        }
    }
}
