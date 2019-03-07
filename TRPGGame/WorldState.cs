using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.EventArgs;
using TRPGGame.Repository;

namespace TRPGGame
{
    public class WorldState : IWorldState
    {
        private readonly IRepository<Map> _mapRepo;

        public WorldState(IRepository<Map> mapRepo)
        {
            _mapRepo = mapRepo;
            var maps = mapRepo.GetDataAsync().Result;
            var temp = new Dictionary<int, MapManager>();
            foreach (var map in maps)
            {
                temp.Add(map.Id, new MapManager(map));
            }
            MapManagers = temp;
        }

        public IReadOnlyDictionary<int, MapManager> MapManagers { get; }

        public void CheckMapStates()
        {
            Parallel.ForEach(MapManagers.Values, (maps) =>
            {
                maps.CheckChanges();
            });
        }
    }
}
