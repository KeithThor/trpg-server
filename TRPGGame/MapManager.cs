﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        /// A dictionary containing all the entities that exist on the managed map along with the coordinate
        /// on the map they currently occupy.
        /// </summary>
        private Dictionary<WorldEntity, Coordinate> _playerEntities = new Dictionary<WorldEntity, Coordinate>();
        private Dictionary<WorldEntity, Coordinate> _hostileEntities = new Dictionary<WorldEntity, Coordinate>();
        private Dictionary<WorldEntity, Coordinate> _allEntities = new Dictionary<WorldEntity, Coordinate>();
        private Dictionary<Coordinate, List<WorldEntity>> _playerPositions = new Dictionary<Coordinate, List<WorldEntity>>();
        private Dictionary<Coordinate, List<WorldEntity>> _hostilesPositions = new Dictionary<Coordinate, List<WorldEntity>>();
        private List<Guid> _connectedPlayers = new List<Guid>();
        private readonly object _lock = new object();
        private bool _isStateChanged = false;
        private bool _worldEntityAdded = false;
        private bool _worldEntityRemoved = false;
        private List<WorldEntity> _newEntities = new List<WorldEntity>();
        private List<int> _removedEntityIds = new List<int>();

        public MapManager(IReadOnlyMap map)
        {
            Map = map;
        }

        /// <summary>
        /// Tries to add an entity to the current map to the requested position. Will return true if adding succeeded.
        /// </summary>
        /// <param name="entity">The entity to add to the map.</param>
        /// <param name="location">The location to spawn the entity at.</param>
        public bool TryAddPlayerEntity(WorldEntity entity, Coordinate location)
        {
            if (!IsValidLocation(location)) return false;
            var swap = new List<WorldEntity> { entity };
            lock(_lock)
            {
                if (!_playerPositions.ContainsKey(location)) _playerPositions[location] = swap;
                else _playerPositions[location].Add(entity);

                _playerEntities[entity] = location;
                _connectedPlayers.Add(entity.OwnerGuid);
                _allEntities[entity] = location;
                _newEntities.Add(entity);

                _isStateChanged = true;
                _worldEntityAdded = true;
            }

            return true;
        }

        /// <summary>
        /// Tries to add an AI-controlled WorldEntity to the map at a given location. Returns false if adding failed.
        /// </summary>
        /// <param name="entity">The entity to add to the map.</param>
        /// <param name="location">The Coordinate location to spawn the entity at.</param>
        /// <returns></returns>
        public bool TryAddEnemyEntity(WorldEntity entity, Coordinate location)
        {
            if (!IsValidLocation(location)) return false;
            var swap = new List<WorldEntity> { entity };
            lock (_lock)
            {
                if (!_hostilesPositions.ContainsKey(location)) _hostilesPositions[location] = swap;
                else _hostilesPositions[location].Add(entity);

                _hostileEntities[entity] = location;
                _allEntities[entity] = location;
                _newEntities.Add(entity);

                _isStateChanged = true;
                _worldEntityAdded = true;
            }

            return true;
        }

        /// <summary>
        /// Removes the entity with the given id from this map.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void RemovePlayerEntity(WorldEntity entity)
        {
            if (!_playerEntities.ContainsKey(entity)) return;
            lock(_lock)
            {
                var location = _playerEntities[entity];
                if (_playerPositions[location].Count <= 1)
                {
                    _playerPositions.Remove(location);
                }
                else
                {
                    _playerPositions[location].Remove(entity);
                }
                _playerEntities.Remove(entity);
                _connectedPlayers.RemoveAll(guid => entity.OwnerGuid == guid);
                _allEntities.Remove(entity);

                _isStateChanged = true;
                _worldEntityRemoved = true;
            }
        }

        /// <summary>
        /// Removes the enemy entity from this map.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void RemoveEnemyEntity(WorldEntity entity)
        {
            if (!_hostileEntities.ContainsKey(entity)) return;
            lock (_lock)
            {
                var location = _hostileEntities[entity];
                if (_hostilesPositions[location].Count <= 1)
                {
                    _hostilesPositions.Remove(location);
                }
                else
                {
                    _hostilesPositions[location].Remove(entity);
                }
                _hostileEntities.Remove(entity);
                _allEntities.Remove(entity);

                _isStateChanged = true;
                _worldEntityRemoved = true;
            }
        }

        /// <summary>
        /// Tries to move an entity to a given location; returns true if the move is successful. Also returns an IEnumerable of
        /// IReadOnlyWorldEntities if any hostile targets were touched.
        /// </summary>
        /// <param name="entity">The entity to try to move.</param>
        /// <param name="newLocation">The new location to try to move to.</param>
        /// <param name="hostileContacts">Will be populated with a List of IReadOnlyWorldEntities if hostile targets were met.</param>
        /// <returns>Returns whether the move was successful.</returns>
        public bool TryMovePlayerEntity(WorldEntity entity, Coordinate newLocation, out IEnumerable<WorldEntity> hostileContacts)
        {
            hostileContacts = null;
            if (!IsValidLocation(newLocation)) return false;
            var swap = new List<WorldEntity> { entity };
            lock(_lock)
            {
                var oldLocation = _playerEntities[entity];
                if (_playerPositions[oldLocation].Count <= 1) _playerPositions.Remove(oldLocation);
                else _playerPositions[oldLocation].Remove(entity);

                _playerEntities[entity] = newLocation;
                _allEntities[entity] = newLocation;
                if (!_playerPositions.ContainsKey(newLocation)) _playerPositions[newLocation] = swap;
                else _playerPositions[newLocation].Add(entity);

                if (_hostilesPositions.ContainsKey(newLocation))
                {
                    hostileContacts = _hostilesPositions[newLocation].Take(GameplayConstants.MaxFormationsPerSide);
                }

                _isStateChanged = true;
                return true;
            }
        }

        /// <summary>
        /// Tries to move an entity to a given location; returns true if the move is successful. Also returns an IEnumerable of
        /// IReadOnlyWorldEntities if any hostile targets were touched.
        /// </summary>
        /// <param name="entity">The entity to try to move.</param>
        /// <param name="newLocation">The new location to try to move to.</param>
        /// <param name="hostileContacts">Will be populated with a List of IReadOnlyWorldEntities if hostile targets were met.</param>
        /// <returns>Returns whether the move was successful.</returns>
        public bool TryMoveEnemyEntity(WorldEntity entity, Coordinate newLocation, out IEnumerable<WorldEntity> hostileContacts)
        {
            hostileContacts = null;
            if (!IsValidLocation(newLocation)) return false;
            var swap = new List<WorldEntity> { entity };
            lock (_lock)
            {
                var oldLocation = _hostileEntities[entity];
                if (_hostilesPositions[oldLocation].Count <= 1) _hostilesPositions.Remove(oldLocation);
                else _hostilesPositions[oldLocation].Remove(entity);

                _hostileEntities[entity] = newLocation;
                _allEntities[entity] = newLocation;
                if (!_hostilesPositions.ContainsKey(newLocation)) _hostilesPositions[newLocation] = swap;
                else _hostilesPositions[newLocation].Add(entity);

                if (_playerPositions.ContainsKey(newLocation))
                {
                    hostileContacts = _playerPositions[newLocation].Take(GameplayConstants.MaxFormationsPerSide);
                }

                _isStateChanged = true;
            }
            return true;
        }

        /// <summary>
        /// Checks to see if the map state has changed, if so invokes the events.
        /// </summary>
        public Task CheckChangesAsync()
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
                        AddedEntities = newEntities,
                        ConnectedPlayers = _connectedPlayers
                    };
                    _newEntities = new List<WorldEntity>();
                    _worldEntityAdded = false;
                }
                if (_worldEntityRemoved)
                {
                    var removedEntities = _removedEntityIds;
                    entityRemovedArgs = new WorldEntityRemovedArgs
                    {
                        RemovedEntityIds = removedEntities,
                        ConnectedPlayers = _connectedPlayers
                    };
                    _removedEntityIds = new List<int>();
                    _worldEntityRemoved = false;
                }
                if (_isStateChanged)
                {
                    mapStateChangedArgs = new MapStateChangedArgs
                    {
                        Entities = _allEntities,
                        ConnectedPlayers = _connectedPlayers
                    };
                    _isStateChanged = false;
                }
            }

            var tasks = new List<Task>();
            if (entityAddedArgs != null)
            {
                tasks.Add(Task.Run(() => WorldEntityAdded?.Invoke(this, entityAddedArgs)));
            }
            if (entityRemovedArgs != null)
            {
                tasks.Add(Task.Run(() => WorldEntityRemoved?.Invoke(this, entityRemovedArgs)));
            }
            if (mapStateChangedArgs != null)
            {
                tasks.Add(Task.Run(() => MapStateChanged?.Invoke(this, mapStateChangedArgs)));
            }

            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// Returns true if a given position is valid for the current map.
        /// </summary>
        /// <param name="position">The coordinate to check.</param>
        /// <returns></returns>
        private bool IsValidLocation(Coordinate position)
        {
            if (position.PositionX < 0 || position.PositionY < 0) return false;
            if (position.PositionX > Map.MapData.Count - 1) return false;
            if (position.PositionY > Map.MapData[0].Count - 1) return false;
            if (Map.MapData[position.PositionX][position.PositionY].IsBlocking) return false;
            else return true;
        }
    }
}
