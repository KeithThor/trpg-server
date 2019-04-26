﻿using System;
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

        public WorldState(IRepository<Map> mapRepo,
                          IWorldEntityFactory worldEntityFactory,
                          AiEntityManagerFactory aiEntityManagerFactory)
        {
            _mapRepo = mapRepo;
            _worldEntityFactory = worldEntityFactory;
            _aiEntityManagerFactory = aiEntityManagerFactory;
            var maps = mapRepo.GetDataAsync().Result;

            var temp = new Dictionary<int, MapManager>();
            var entityManagers = new Dictionary<int, MapEntityManager>();

            foreach (var map in maps)
            {
                var mapManager = new MapManager(map);
                temp.Add(map.Id, mapManager);
                entityManagers.Add(map.Id, new MapEntityManager(mapManager, _worldEntityFactory, _aiEntityManagerFactory));
            }
            MapManagers = temp;
            MapEntityManagers = entityManagers;
        }

        public IReadOnlyDictionary<int, MapManager> MapManagers { get; }
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
