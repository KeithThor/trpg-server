using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TRPGGame.Entities;
using TRPGGame.Entities.Combat;
using TRPGGame.EventArgs;
using TRPGGame.Managers;
using TRPGGame.Services;
using TRPGShared;

namespace TRPGGame
{
    /// <summary>
    /// Manager responsible for replicating a player's entity's actions from the server into the game.
    /// </summary>
    public class PlayerEntityManager
    {
        public WorldEntity Entity
        {
            get { lock (_lock) return _entity; }
            set { lock (_lock) _entity = value; }
        }

        public Guid PlayerId { get; set; }
        private IBattleManager _battleManager = null;
        private IEnumerable<WorldEntity> _hostilesQueuedForBattle;
        private readonly IWorldState _worldState;
        private readonly IStateManager _stateManager;
        private readonly BattleManagerFactory _battleManagerFactory;
        private readonly object _lock = new object();
        private MapManager _currentMapManager;
        private WorldEntity _entity;

        public bool IsActive { get; private set; } = false;

        public PlayerEntityManager(IWorldState worldState,
                                   IStateManager stateManager,
                                   BattleManagerFactory battleManagerFactory)
        {
            _worldState = worldState;
            _stateManager = stateManager;
            _battleManagerFactory = battleManagerFactory;
            LastAccessed = DateTime.Now;
        }

        /// <summary>
        /// Contains the time this manager was last accessed by a user.
        /// </summary>
        public DateTime LastAccessed { get; private set; }

        /// <summary>
        /// Adds the player's entity to the last saved location and begins accepting interactions.
        /// </summary>
        public void BeginPlay()
        {
            lock (_lock)
            {
                if (!IsActive && _entity != null)
                {
                    _currentMapManager = _worldState.MapManagers[Entity.CurrentMapId];
                    _currentMapManager.TryAddPlayerEntity(Entity, Entity.Position);
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
        /// <param name="canStartBattle">Returns true if the player's entity walked into hostile entities.</param>
        /// <returns>Returns true if the move succeeded.</returns>
        public bool MoveEntity(int deltaX, int deltaY, out bool canStartBattle)
        {
            if (!IsActive || Entity == null) throw new Exception("PlayerEntityManager must be initialized before calling MoveEntity.");
            canStartBattle = false;
            IEnumerable<WorldEntity> hostiles = null;
            bool success = false;

            lock (_lock)
            {
                LastAccessed = DateTime.Now;
                var oldPosition = Entity.Position;
                var newPosition = new Coordinate()
                {
                    PositionX = oldPosition.PositionX + deltaX,
                    PositionY = oldPosition.PositionY + deltaY
                };

                if (newPosition.PositionX >= _currentMapManager.Map.MapData.Count) return false;
                if (newPosition.PositionY >= _currentMapManager.Map.MapData[0].Count) return false;
                if (_currentMapManager.Map.MapData[newPosition.PositionX][newPosition.PositionY].IsBlocking) return false;

                if (_currentMapManager.TryMovePlayerEntity(Entity, newPosition, out hostiles))
                {
                    Entity.Position = newPosition;
                    _hostilesQueuedForBattle = hostiles;
                    success = true;
                }
            }
            if (hostiles != null) canStartBattle = true;
            return success;
        }

        /// <summary>
        /// Creates a battle for this player and returns the BattleManager responsible for handling this battle.
        /// <para>If a battle already exists, returns the BattleManager for the current battle.</para>
        /// </summary>
        /// <returns></returns>
        public IBattleManager StartBattle()
        {
            lock (_lock)
            {
                if (_battleManager != null) return _battleManager;
                if (_stateManager.GetPlayerState(PlayerId) != PlayerStateConstants.Free) return null;
                if (_hostilesQueuedForBattle == null || _hostilesQueuedForBattle.Count() <= 0) return null;

                _battleManager = _battleManagerFactory.Create();
                var defenders = _hostilesQueuedForBattle.Select(h => h.ActiveFormation).ToList();
                _battleManager.EndOfBattleEvent += OnEndOfBattle;

                _battleManager.StartBattle(new List<Formation> { Entity.ActiveFormation }, defenders);
                return _battleManager;
            }
        }

        /// <summary>
        /// Called to add a new player to this PlayerEntityManager's current battle. Returns the BattleManager
        /// for this PlayerEntityManager's battle.
        /// </summary>
        /// <param name="joinerManager">The PlayerManager for the player joining this battle.</param>
        /// <param name="isAttacker">If true, will join battle as an attacker. Else as a defender.</param>
        /// <returns></returns>
        public IBattleManager JoinMyBattle(PlayerEntityManager joinerManager, bool isAttacker)
        {
            var joiningFormation = joinerManager.Entity.ActiveFormation;
            lock (_lock)
            {
                if (_battleManager == null) return null;
                _battleManager.JoinBattle(joiningFormation, isAttacker);
                _battleManager.EndOfBattleEvent += joinerManager.OnEndOfBattle;
                _stateManager.SetPlayerInCombat(joinerManager.PlayerId);
                return _battleManager;
            }
        }

        public void OnEndOfBattle(object sender, EndOfBattleEventArgs args)
        {
            lock (_lock)
            {
                _battleManager = null;
                _stateManager.SetPlayerFree(PlayerId);
            }
        }

        /// <summary>
        /// Gets the BattleManager that is managing this PlayerEntityManager's battle, if any.
        /// </summary>
        /// <returns></returns>
        public IBattleManager GetBattleManager()
        {
            lock (_lock)
            {
                return _battleManager;
            }
        }

        /// <summary>
        /// Returns true if the player's entity is eligible to change maps.
        /// </summary>
        /// <returns></returns>
        public bool CanChangeMaps()
        {
            lock (_lock)
            {
                if (!IsActive || Entity == null) throw new Exception("PlayerEntityManager must be initialized before calling ChangeMaps.");
                var tile = _currentMapManager.Map.MapData[Entity.Position.PositionX][Entity.Position.PositionY];
                if (!tile.CanTransport) return false;
                if (!_currentMapManager.Map.MapConnections.Contains(tile.TransportMapId)) return false;
            }

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
            if (!CanChangeMaps()) return false;

            lock (_lock)
            {
                LastAccessed = DateTime.Now;
                var tile = _currentMapManager.Map.MapData[Entity.Position.PositionX][Entity.Position.PositionY];
                newMapId = tile.TransportMapId;
                _currentMapManager.RemovePlayerEntity(Entity);
                _currentMapManager = _worldState.MapManagers[newMapId];
                Entity.CurrentMapId = newMapId;
                Entity.Position = tile.TransportLocation;
                IsActive = false;
            }
            return true;
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
                if (!IsActive || Entity == null) throw new Exception("PlayerEntityManager must be initialized before calling ChangeMaps.");
                LastAccessed = DateTime.Now;
                if (!_currentMapManager.Map.MapConnections.Contains(newMapId)) return false;
                var tile = _currentMapManager.Map.MapData[Entity.Position.PositionX][Entity.Position.PositionY];
                if (!tile.CanTransport) return false;
                if (tile.TransportMapId != newMapId) return false;
                _currentMapManager.RemovePlayerEntity(Entity);
                _currentMapManager = _worldState.MapManagers[newMapId];
                Entity.Position = tile.TransportLocation;
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
            if (Entity == null) return null;
            lock (_lock)
            {
                if (IsActive) return _currentMapManager.Map;
                else
                {
                    int mapId = Entity.CurrentMapId;
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
                    _currentMapManager.RemovePlayerEntity(Entity);
                    IsActive = false;
                }
            }
        }
    }
}