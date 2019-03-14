using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRPGGame;
using TRPGGame.EventArgs;
using TRPGServer.Hubs;
using TRPGServer.Models;

namespace TRPGServer.Services
{
    /// <summary>
    /// Class that listens to changes from a single MapManager and sends messages to all clients
    /// subscribed to a map when there is an update.
    /// </summary>
    public class MapEntityListener : IDisplayEntityStore
    {
        private readonly MapManager _mapManager;
        private readonly IHubContext<WorldEntityHub> _hubContext;
        private readonly WorldEntityManager _worldEntityManager;

        /// <summary>
        /// The id of the map that this listener is listening to.
        /// </summary>
        public readonly string MapId;
        private List<DisplayEntity> _displayEntities = new List<DisplayEntity>();

        public MapEntityListener(MapManager mapManager,
                                 IHubContext<WorldEntityHub> hubContext,
                                 WorldEntityManager worldEntityManager)
        {
            _mapManager = mapManager;
            _hubContext = hubContext;
            _worldEntityManager = worldEntityManager;
            _mapManager.MapStateChanged += OnMapStateChanged;
            _mapManager.WorldEntityRemoved += OnWorldEntitiesRemoved;
            _mapManager.WorldEntityAdded += OnWorldEntitiesAdded;
            MapId = _mapManager.Map.Id.ToString();
        }

        private void OnMapStateChanged(object sender, MapStateChangedArgs e)
        {
            var entityLocations = e.Entities.Select(kvp => new
            {
                Id = kvp.Key,
                Location = kvp.Value
            });

            _hubContext.Clients.Group(MapId).SendAsync("updateEntities", e.MapSpaces, entityLocations);
        }

        private void OnWorldEntitiesAdded(object sender, WorldEntityAddedArgs e)
        {
            var entities = new List<DisplayEntity>();
            foreach (var entity in e.AddedEntities)
            {
                entities.Add(new DisplayEntity
                {
                    Id = entity.Id,
                    IconUris = entity.IconUris,
                    Name = entity.Name
                });
            }
            _displayEntities.AddRange(entities);

            var tasks = new List<Task>
            {
                _hubContext.Clients.Group(MapId).SendAsync("addEntities", entities)
            };

            foreach (var entity in e.AddedEntities)
            {
                tasks.Add(_hubContext.Clients.User(entity.OwnerGuid.ToString()).SendAsync("addEntities", _displayEntities));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private void OnWorldEntitiesRemoved(object sender, WorldEntityRemovedArgs e)
        {
            _hubContext.Clients.Group(MapId).SendAsync("removeEntities", e.RemovedEntityIds);
            _displayEntities = _displayEntities.Where(entity => !e.RemovedEntityIds.Contains(entity.Id)).ToList();
        }

        public IEnumerable<DisplayEntity> GetDisplayEntities()
        {
            return _displayEntities;
        }
    }
}
