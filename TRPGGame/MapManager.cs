using System;
using System.Collections.Generic;
using System.Threading;
using TRPGGame.Entities;
using TRPGGame.EventArgs;
using TRPGShared;

namespace TRPGGame
{
    /// <summary>
    /// Manager responsible for managing events and interactions between entities in a given map.
    /// </summary>
    public class MapManager
    {
        /// <summary>
        /// The map that this MapManager is responsible for managing.
        /// </summary>
        public IReadOnlyMap Map { get; }

        /// <summary>
        /// Event invoked every game update if the map state has changed.
        /// </summary>
        public event EventHandler<MapStateChangedArgs> MapStateChanged;

        /// <summary>
        /// Event invoked whenever one or more WorldEntities are added to the map.
        /// </summary>
        public event EventHandler<WorldEntityAddedArgs> WorldEntityAdded;

        /// <summary>
        /// Event invoked whenever one or more WorldEntities are removed from the map.
        /// </summary>
        public event EventHandler<WorldEntityRemovedArgs> WorldEntityRemoved;

        /// <summary>
        /// A 2d list of entity ids that matches the size of the managed map that tracks which spaces on the map
        /// are occupied by entities.
        /// </summary>
        private List<List<int?>> _occupiedSpaces;

        /// <summary>
        /// A dictionary containing all the entities that exist on the managed map along with the coordinate
        /// on the map they currently occupy.
        /// </summary>
        private Dictionary<int, Coordinate> _entities = new Dictionary<int, Coordinate>();
        private readonly object _lock = new object();
        private bool _isStateChanged = false;
        private bool _worldEntityAdded = false;
        private bool _worldEntityRemoved = false;
        private List<IReadOnlyWorldEntity> _newEntities = new List<IReadOnlyWorldEntity>();
        private List<int> _removedEntityIds = new List<int>();

        public MapManager(IReadOnlyMap map)
        {
            Map = map;
            _occupiedSpaces = new List<List<int?>>();
            for (int i = 0; i < Map.MapData.Count; i++)
            {
                _occupiedSpaces.Add(new List<int?>());
                for (int j = 0; j < Map.MapData[i].Count; j++)
                {
                    _occupiedSpaces[i].Add(null);
                }
            }
        }

        /// <summary>
        /// Adds a entity to this map.
        /// </summary>
        /// <param name="entity">The entity to add to the map.</param>
        /// <param name="location">The location to spawn the entity at.</param>
        public void AddEntity(IReadOnlyWorldEntity entity, Coordinate location)
        {
            lock(_lock)
            {
                if (_occupiedSpaces[location.PositionX][location.PositionY] == null)
                {
                    _occupiedSpaces[location.PositionX][location.PositionY] = entity.Id;
                    _entities.Remove(entity.Id);
                    _entities.Add(entity.Id, location);
                    _newEntities.Add(entity);
                    _isStateChanged = true;
                    _worldEntityAdded = true;
                }
            }
        }

        /// <summary>
        /// Removes the entity with the given id from this map.
        /// </summary>
        /// <param name="entityId">The id of the entity to remove.</param>
        public void RemoveEntity(int entityId)
        {
            lock(_lock)
            {
                if (_entities.ContainsKey(entityId))
                {
                    var location = _entities[entityId];
                    if (_occupiedSpaces[location.PositionX][location.PositionY] == entityId)
                    {
                        _occupiedSpaces[location.PositionX][location.PositionY] = null;
                        _entities.Remove(entityId);
                        _removedEntityIds.Add(entityId);
                        _isStateChanged = true;
                        _worldEntityRemoved = true;
                    }
                }
            }
        }

        /// <summary>
        /// Tries to move an entity to a given location; returns true if the move is successful.
        /// </summary>
        /// <param name="entityId">The id of the entity.</param>
        /// <param name="newLocation">The new location to try to move to.</param>
        /// <returns>Returns whether the move was successful.</returns>
        public bool TryMoveEntity(int entityId, Coordinate newLocation)
        {
            lock(_lock)
            {
                if (_occupiedSpaces[newLocation.PositionX][newLocation.PositionY] != null) return false;

                var oldLocation = _entities[entityId];
                _occupiedSpaces[oldLocation.PositionX][oldLocation.PositionY] = null;
                _occupiedSpaces[newLocation.PositionX][newLocation.PositionY] = entityId;
                _entities[entityId] = newLocation;
                _isStateChanged = true;
                return true;
            }
        }

        /// <summary>
        /// Checks to see if the map state has changed, if so invokes the events.
        /// </summary>
        public void CheckChanges()
        {
            WorldEntityAddedArgs entityAddedArgs = null;
            WorldEntityRemovedArgs entityRemovedArgs = null;
            MapStateChangedArgs mapStateChangedArgs = null;

            lock (_lock)
            {
                if (_worldEntityAdded)
                {
                    var newEntities = _newEntities;
                    entityAddedArgs = new WorldEntityAddedArgs
                    {
                        AddedEntities = newEntities
                    };
                    _newEntities = new List<IReadOnlyWorldEntity>();
                    _worldEntityAdded = false;
                }
                if (_worldEntityRemoved)
                {
                    var removedEntities = _removedEntityIds;
                    entityRemovedArgs = new WorldEntityRemovedArgs
                    {
                        RemovedEntityIds = removedEntities
                    };
                    _removedEntityIds = new List<int>();
                    _worldEntityRemoved = false;
                }
                if (_isStateChanged)
                {
                    mapStateChangedArgs = new MapStateChangedArgs
                    {
                        MapSpaces = new List<List<int?>>(_occupiedSpaces),
                        Entities = _entities
                    };
                    _isStateChanged = false;
                }
            }

            if (entityAddedArgs != null)
            {
                WorldEntityAdded?.Invoke(this, entityAddedArgs);
            }
            if (entityRemovedArgs != null)
            {
                WorldEntityRemoved?.Invoke(this, entityRemovedArgs);
            }
            if (mapStateChangedArgs != null)
            {
                MapStateChanged?.Invoke(this, mapStateChangedArgs);
            }
        }
    }
}
