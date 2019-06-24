using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.EventArgs;
using TRPGGame.Managers;
using TRPGGame.Repository;
using TRPGGame.Services;

namespace TRPGGame
{
    /// <summary>
    /// Class that contains all the MapManagers and MapEntityManagers that exists in the game.
    /// </summary>
    public class WorldState : IWorldState
    {
        private readonly IRepository<Map> _mapRepo;
        private readonly IWorldEntityFactory _worldEntityFactory;
        private readonly AiEntityManagerFactory _aiEntityManagerFactory;
        private readonly BattleManagerFactory _battleManagerFactory;

        public WorldState(IRepository<Map> mapRepo,
                          IWorldEntityFactory worldEntityFactory,
                          AiEntityManagerFactory aiEntityManagerFactory,
                          BattleManagerFactory battleManagerFactory)
        {
            _mapRepo = mapRepo;
            _worldEntityFactory = worldEntityFactory;
            _aiEntityManagerFactory = aiEntityManagerFactory;
            _battleManagerFactory = battleManagerFactory;
            var maps = mapRepo.GetDataAsync().Result;

            var temp = new Dictionary<int, MapManager>();
            var mapBattleManagers = new Dictionary<int, IMapBattleManager>();
            var entityManagers = new Dictionary<int, MapEntityManager>();

            foreach (var map in maps)
            {
                var mapManager = new MapManager(map);
                temp.Add(map.Id, mapManager);
                var mapBattleManager = new MapBattleManager(_battleManagerFactory);
                entityManagers.Add(map.Id, new MapEntityManager(mapManager, 
                                                                mapBattleManager, 
                                                                _worldEntityFactory, 
                                                                _aiEntityManagerFactory));

                mapBattleManagers.Add(map.Id, mapBattleManager);
            }
            MapManagers = temp;
            MapBattleManagers = mapBattleManagers;
            MapEntityManagers = entityManagers;
        }

        /// <summary>
        /// Gets a dictionary containing all of the map managers this WorldState is handling.
        /// <para>Key is the id of the map the MapManager is managing.</para>
        /// </summary>
        public IReadOnlyDictionary<int, MapManager> MapManagers { get; }

        /// <summary>
        /// Gets a dictionary containing all of the MapBattleManagers this WorldState is handling.
        /// <para>Key is the id of the map the MapBattleManagers is managing.</para>
        /// </summary>
        public IReadOnlyDictionary<int, IMapBattleManager> MapBattleManagers { get; }

        /// <summary>
        /// Gets a dictionary containing all of the MapEntityManagers this WorldState is handling.
        /// <para>Key is the id of the map the MapEntityManager is managing.</para>
        /// </summary>
        public IReadOnlyDictionary<int, MapEntityManager> MapEntityManagers { get; }

        /// <summary>
        /// Goes through each map and checks their states, invoking any necessary game events as needed.
        /// </summary>
        public void CheckMapStates()
        {
            var tasks = new List<Task>();
            Parallel.ForEach(MapManagers.Values, (maps) =>
            {
                tasks.Add(Task.Run(() => maps.CheckChangesAsync()));
            });

            Task.WaitAll(tasks.ToArray());
        }
    }
}
