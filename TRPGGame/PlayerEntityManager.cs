using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TRPGGame.Entities;
using TRPGShared;

namespace TRPGGame
{
    /// <summary>
    /// Manager responsible for replicating a player's entity's actions on the server.
    /// </summary>
    public class PlayerEntityManager
    {
        private readonly WorldEntity _worldEntity;
        private readonly IWorldState _worldState;
        private readonly object _lock = new object();
        private MapManager _currentMapManager;
        public bool IsActive { get; private set; } = false;

        public PlayerEntityManager(WorldEntity worldEntity,
                                   IWorldState worldState)
        {
            _worldEntity = worldEntity;
            _worldState = worldState;
            LastAccessed = DateTime.Now;
        }

        /// <summary>
        /// Contains the time this manager was last accessed by a user.
        /// </summary>
        internal DateTime LastAccessed { get; private set; }

        /// <summary>
        /// Gets a read-only reference to the player's WorldEntity.
        /// </summary>
        /// <returns>A read-only reference of the player's WorldEntity.</returns>
        public IReadOnlyWorldEntity GetEntity()
        {
            return _worldEntity;
        }

        /// <summary>
        /// Adds the player's entity to the last saved location and begins accepting interactions.
        /// </summary>
        public void BeginPlay()
        {
            lock (_lock)
            {
                if (!IsActive)
                {
                    _currentMapManager = _worldState.MapManagers[_worldEntity.CurrentMapId];
                    _currentMapManager.AddEntity(_worldEntity, _worldEntity.Position);
                    IsActive = true;
                    LastAccessed = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Moves the player's entity a specified amount from the entity's current position.
        /// </summary>
        /// <param name="deltaX">The amount of horizontal displacement from the entity's current position.</param>
        /// <param name="deltaY">The amount of vertical displacement from the entity's current position.</param>
        /// <returns>Returns true if the move succeeded.</returns>
        public bool MoveEntity(int deltaX, int deltaY)
        {
            if (!IsActive) throw new Exception("PlayerEntityManager must be initialized before calling MoveEntity.");

            LastAccessed = DateTime.Now;
            var oldPosition = _worldEntity.Position;
            var newPosition = new Coordinate()
            {
                PositionX = oldPosition.PositionX + deltaX,
                PositionY = oldPosition.PositionY + deltaY
            };

            if (_currentMapManager.Map.MapData[newPosition.PositionX][newPosition.PositionY].IsBlocking) return false;

            lock (_lock)
            {
                if (_currentMapManager.TryMoveEntity(_worldEntity.Id, newPosition))
                {
                    _worldEntity.Position = newPosition;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the player's entity is eligible to change maps.
        /// </summary>
        /// <returns></returns>
        public bool CanChangeMaps()
        {
            if (!IsActive) throw new Exception("PlayerEntityManager must be initialized before calling ChangeMaps.");

            var tile = _currentMapManager.Map.MapData[_worldEntity.Position.PositionX][_worldEntity.Position.PositionY];
            if (!tile.CanTransport) return false;
            if (!_currentMapManager.Map.MapConnections.Contains(tile.TransportMapId)) return false;

            return true;
        }

        /// <summary>
        /// Changes the current map and location of the player entity to a new map and location based on the tile the
        /// player's entity currently occupies. Returns true if changing maps was successful.
        /// </summary>
        /// <param name="newMapId">Will contain the id of the new map if the change was successful.</param>
        /// <returns>Returns true if changing maps was successful.</returns>
        public bool ChangeMaps(ref int newMapId)
        {
            lock (_lock)
            {
                LastAccessed = DateTime.Now;
                if (!CanChangeMaps()) return false;

                var tile = _currentMapManager.Map.MapData[_worldEntity.Position.PositionX][_worldEntity.Position.PositionY];
                newMapId = tile.TransportMapId;

                _currentMapManager.RemoveEntity(_worldEntity.Id);
                _currentMapManager = _worldState.MapManagers[newMapId];
                _worldEntity.CurrentMapId = newMapId;
                _worldEntity.Position = tile.TransportLocation;
                IsActive = false;

                return true;
            }
        }

        /// <summary>
        /// Changes the current map and location of the player entity to a new map and location.
        /// <para>If successful, makes the manager inactive until BeginPlay is called again.</para>
        /// </summary>
        /// <param name="newMapId">The id of the new map to change to.</param>
        /// <returns>True if changing maps was successful.</returns>
        public bool ChangeMaps(int newMapId)
        {
            lock (_lock)
            {
                if (!IsActive) throw new Exception("PlayerEntityManager must be initialized before calling ChangeMaps.");
                LastAccessed = DateTime.Now;

                if (!_currentMapManager.Map.MapConnections.Contains(newMapId)) return false;
                var tile = _currentMapManager.Map.MapData[_worldEntity.Position.PositionX][_worldEntity.Position.PositionY];
                if (!tile.CanTransport) return false;
                if (tile.TransportMapId != newMapId) return false;

                _currentMapManager.RemoveEntity(_worldEntity.Id);
                _currentMapManager = _worldState.MapManagers[newMapId];
                _worldEntity.Position = tile.TransportLocation;
                IsActive = false;

                return true;
            }
        }

        /// <summary>
        /// Gets the map that the player entity current resides in.
        /// </summary>
        /// <returns>The read-only instance of the map.</returns>
        public IReadOnlyMap GetCurrentMap()
        {
            lock (_lock)
            {
                if (IsActive) return _currentMapManager.Map;
                else
                {
                    int mapId = _worldEntity.CurrentMapId;
                    return _worldState.MapManagers[mapId].Map;
                }
            }
        }

        /// <summary>
        /// Removes the player's entity from the map in preparation for a log out.
        /// </summary>
        public void EndConnection()
        {
            lock (_lock)
            {
                if (IsActive)
                {
                    LastAccessed = DateTime.Now;
                    _currentMapManager.RemoveEntity(_worldEntity.Id);
                    IsActive = false;
                }
            }
        }
    }
}