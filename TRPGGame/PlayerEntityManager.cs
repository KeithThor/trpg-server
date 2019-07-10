using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private IEnumerable<WorldEntity> _contactsQueuedForBattle;
        internal List<WorldEntity> AlliedEntities { get; set; }
        private readonly IWorldState _worldState;
        private readonly IStateManager _stateManager;
        private readonly IPlayerEntityManagerStore _playerEntityManagerStore;
        private readonly object _lock = new object();
        private MapManager _currentMapManager;
        private IMapBattleManager _mapBattleManager;
        private WorldEntity _entity;
        private Queue<Coordinate> _movementQueue;

        private int _targetEntityId;
        private string _targetOwnerId;
        private string _actionName;
        private const int _MovementsPerSecond = 5;
        private int _ticks = 0;

        public bool IsActive { get; private set; } = false;

        public PlayerEntityManager(IWorldState worldState,
                                   IStateManager stateManager,
                                   IPlayerEntityManagerStore playerEntityManagerStore)
        {
            _worldState = worldState;
            _stateManager = stateManager;
            _playerEntityManagerStore = playerEntityManagerStore;
            LastAccessed = DateTime.Now;
            _movementQueue = new Queue<Coordinate>();
            AlliedEntities = new List<WorldEntity>();
        }

        /// <summary>
        /// Event invoked when this PlayerEntityManager should be destroyed.
        /// </summary>
        public event EventHandler<System.EventArgs> OnDestroy;

        /// <summary>
        /// Event invoked when the WorldEntity being managed has stopped moving.
        /// </summary>
        public event EventHandler<System.EventArgs> OnMovementStopped;

        /// <summary>
        /// Event invoked whenever the PlayerEntityManager initiates battle or has battle initiated on the
        /// WorldEntity being managed.
        /// </summary>
        public event EventHandler<CreatedBattleEventArgs> OnBattleInitiated;

        /// <summary>
        /// Event invoked whenever the PlayerEntityManager successfully joins a battle.
        /// </summary>
        public event EventHandler<JoinBattleSuccessEventArgs> OnJoinBattleSuccess;

        private void OnGameTick(object sender, GameTickEventArgs args)
        {
            _ticks++;
            if (_ticks < 1000 / _MovementsPerSecond / GameplayConstants.GameTicksPerSecond) return;

            _ticks = 0;
            bool success = false;
            bool isMovementStopped = false;

            // Don't move if wrong state
            if (_stateManager.GetPlayerState(PlayerId) != PlayerStateConstants.Free) return;

            lock (_lock)
            {
                while (!success && _movementQueue.Count > 0)
                {
                    success = TryMoveEntity(_movementQueue.Dequeue());
                }

                isMovementStopped = _movementQueue.Count <= 0;
                if (isMovementStopped) PerformAction();
            }

            if (isMovementStopped) Task.Run(() => OnMovementStopped?.Invoke(this, new System.EventArgs()));
        }

        /// <summary>
        /// Handler invoked whenever any battle is created on the map the managed WorldEntity is on.
        /// <para>If the WorldEntity is in the newly created battle, initiates battle for this player.</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnCreatedBattle(object sender, CreatedBattleEventArgs args)
        {
            // Call initiated battle
            if (args.PlayersInBattle.TryGetValue(PlayerId, out Guid actual))
            {
                lock(_lock)
                {
                    if (_battleManager != null)
                        throw new Exception($"A new battle was created for {PlayerId} while player was in another battle!");

                    _battleManager = args.BattleManager;
                    _battleManager.EndOfBattleEvent += OnEndOfBattle;
                    _stateManager.SetPlayerInCombat(PlayerId);
                }

                Task.Run(() => OnBattleInitiated?.Invoke(this, args));
            }
        }

        /// <summary>
        /// Handler invoked at the end of a battle; Will remove state restrictions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnEndOfBattle(object sender, EndOfBattleEventArgs args)
        {
            lock (_lock)
            {
                _battleManager.EndOfBattleEvent -= OnEndOfBattle;
                _battleManager = null;
                _stateManager.SetPlayerFree(PlayerId);
            }
        }

        /// <summary>
        /// Contains the time this manager was last accessed by a user.
        /// </summary>
        public DateTime LastAccessed { get; private set; }

        /// <summary>
        /// Enters the player's WorldEntity into it's last saved location in the last saved map.
        /// <para>Subscribes to all relevant events in that map.</para>
        /// </summary>
        public void BeginPlay()
        {
            lock (_lock)
            {
                if (!IsActive && _entity != null)
                {
                    _currentMapManager = _worldState.MapManagers[Entity.CurrentMapId];
                    _currentMapManager.GameTick += OnGameTick;

                    _mapBattleManager = _worldState.MapBattleManagers[Entity.CurrentMapId];
                    _mapBattleManager.OnCreatedBattle += OnCreatedBattle;

                    _currentMapManager.TryAddEntity(Entity, Entity.Position);
                    IsActive = true;
                    LastAccessed = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Tries to move the player's WorldEntity to the given location on the current map.
        /// </summary>
        /// <param name="newLocation">The location to move the player's WorldEntity to.</param>
        /// <returns>Returns true if the move was successful.</returns>
        private bool TryMoveEntity(Coordinate newLocation)
        {
            if (!IsActive || Entity == null) throw new Exception("PlayerEntityManager must be initialized before calling MoveEntity.");
            int deltaX = 0;
            int deltaY = 0;

            lock (_lock)
            {
                deltaX = Entity.Position.PositionX - newLocation.PositionX;
                deltaY = Entity.Position.PositionY - newLocation.PositionY;

                if (deltaX > GameplayConstants.MaxTileMoveDistance
                || deltaX < -GameplayConstants.MaxTileMoveDistance) return false;

                if (deltaY > GameplayConstants.MaxTileMoveDistance
                    || deltaY < -GameplayConstants.MaxTileMoveDistance) return false;

                // Disallow diagonal movement
                if (deltaX != 0 && deltaY != 0) return false;
            }

            if (deltaX == 0 && deltaY == 0) return false;

            return MoveEntity(newLocation);
        }

        /// <summary>
        /// Moves the player's entity to a new coordinate position.
        /// </summary>
        /// <param name="newPosition">The new position to move the WorldEntity to.</param>
        /// <returns>Returns true if the move succeeded.</returns>
        private bool MoveEntity(Coordinate newPosition)
        {
            if (!IsActive || Entity == null) throw new Exception("PlayerEntityManager must be initialized before calling MoveEntity.");
            bool success = false;

            lock (_lock)
            {
                LastAccessed = DateTime.Now;

                if (!_currentMapManager.IsValidLocation(newPosition)) return false;

                if (_currentMapManager.TryMoveEntity(Entity, newPosition, out _contactsQueuedForBattle))
                {
                    Entity.Position = newPosition;
                    success = true;
                }
            }
            return success;
        }
        
        /// <summary>
        /// Sets the current movement queue to the provided movement path.
        /// </summary>
        /// <param name="movePath">An IEnumerable of Coordinates that represent the movement path for the player's entity.</param>
        public void SetMovePath(IEnumerable<Coordinate> movePath)
        {
            _movementQueue = new Queue<Coordinate>(movePath);
        }

        /// <summary>
        /// Queues an action targeted at a WorldEntity by moving along the given move path.
        /// <para>Will attempt to perform the action at the end of the move path.</para>
        /// </summary>
        /// <param name="entityId">The id of the WorldEntity to perform the action on.</param>
        /// <param name="ownerId">The id of the owner of the WorldEntity to perform the action on.</param>
        /// <param name="action">The name of the action to perform. Import PlayerActionConstants for the complete
        /// list of actions.</param>
        /// <param name="movePath">The path of Coordinates to follow to the target destination.</param>
        public void QueueAction(int entityId, string ownerId, string action, IEnumerable<Coordinate> movePath)
        {
            SetMovePath(movePath);

            // Action is targeting self, is invalid
            if (entityId == Entity.Id || PlayerId.ToString() == ownerId) return;

            _targetEntityId = entityId;
            _targetOwnerId = ownerId;
            _actionName = action;
        }

        /// <summary>
        /// Performs the currently queued action, if any.
        /// </summary>
        private void PerformAction()
        {
            if (_actionName == null || _targetOwnerId == null) return;

            switch (_actionName)
            {
                case PlayerActionConstants.Attack:
                    StartBattle();
                    break;
                case PlayerActionConstants.Join:
                    JoinBattle();
                    break;
                default:
                    break;
            }

            _actionName = null;
            _targetOwnerId = null;
        }

        /// <summary>
        /// Creates a battle for this player and returns the BattleManager responsible for handling this battle.
        /// <para>If a battle already exists, returns the BattleManager for the current battle.</para>
        /// </summary>
        /// <returns></returns>
        private void StartBattle()
        {
            lock (_lock)
            {
                // Player hasn't created a formation or is in combat, prevent from starting a battle
                if (_stateManager.GetPlayerState(PlayerId) != PlayerStateConstants.Free) return;

                if (_contactsQueuedForBattle == null || _contactsQueuedForBattle.Count() <= 0) return;

                // Find original target
                var target = _contactsQueuedForBattle.FirstOrDefault(contact =>
                {
                    return contact.Id == _targetEntityId && contact.OwnerGuid.ToString() == _targetOwnerId;
                });

                // If target is not in the list of contacts or is already in a battle, cancels trying to start a battle
                if (target == null || _mapBattleManager.TryGetBattle(target, out IBattleManager throwaway)) return;

                IEnumerable<WorldEntity> defenders = null;

                // Get all ai WorldEntity contacts and set them as the target defenders
                if (target.OwnerGuid == GameplayConstants.AiId)
                {
                    defenders = _contactsQueuedForBattle.Where(entity =>
                    {
                        if (entity == target) return false;
                        return entity.OwnerGuid == GameplayConstants.AiId &&
                                                   !_mapBattleManager.TryGetBattle(entity, out IBattleManager manager);
                    }).Take(GameplayConstants.MaxFormationsPerSide - 1)
                      .Append(target)
                      .ToList();
                }
                // Target is a player, get all of the target player's allies and set them as the defenders
                else
                {
                    var enemyPlayerManager = _playerEntityManagerStore.GetPlayerEntityManager(target.OwnerGuid);
                    if (enemyPlayerManager == null) return;

                    defenders = enemyPlayerManager.AlliedEntities.Append(target).ToList();
                    if (defenders.Count() > GameplayConstants.MaxFormationsPerSide)
                        throw new Exception($"Too many people in the party of player with id {target.OwnerGuid}!");
                }

                var attackers = AlliedEntities.Append(Entity).ToList();

                var success = _mapBattleManager.CreateBattle(attackers, defenders);
            }
        }

        /// <summary>
        /// Attempts to join the battle of the stored target WorldEntity.
        /// </summary>
        private void JoinBattle()
        {
            lock (_lock)
            {
                // Player hasn't created a formation or is in combat, prevent from joining a battle
                if (_stateManager.GetPlayerState(PlayerId) != PlayerStateConstants.Free) return;

                if (_contactsQueuedForBattle == null || _contactsQueuedForBattle.Count() <= 0) return;
                if (_battleManager != null) throw new Exception($"Player {PlayerId} tried to join a battle while another is in progress!");

                var hostEntity = _contactsQueuedForBattle.FirstOrDefault(entity =>
                {
                    return entity.Id == _targetEntityId && entity.OwnerGuid.ToString() == _targetOwnerId;
                });

                if (_mapBattleManager.TryGetBattle(hostEntity, out IBattleManager battleManager))
                {
                    if (battleManager.JoinBattle(hostEntity, Entity) != null) _battleManager = battleManager;
                }
            }

            if (_battleManager != null)
            {
                _battleManager.EndOfBattleEvent += OnEndOfBattle;
                _stateManager.SetPlayerInCombat(PlayerId);
                Task.Run(() => OnJoinBattleSuccess?.Invoke(this, new JoinBattleSuccessEventArgs
                {
                    BattleManager = _battleManager
                }));
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

                _currentMapManager.RemoveEntity(Entity);
                _currentMapManager.GameTick -= OnGameTick;
                _mapBattleManager.OnCreatedBattle -= OnCreatedBattle;

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

                _currentMapManager.RemoveEntity(Entity);
                _currentMapManager.GameTick -= OnGameTick;
                _mapBattleManager.OnCreatedBattle -= OnCreatedBattle;

                _currentMapManager = _worldState.MapManagers[newMapId];

                Entity.Position = tile.TransportLocation;
                IsActive = false;

                return true;
            }
        }

        /// <summary>
        /// Sets the date last accessed to the current time, delaying the point at which this PlayerEntityManager gets
        /// garbage collected automatically.
        /// </summary>
        public void SetDateAccessed()
        {
            lock (_lock)
            {
                LastAccessed = DateTime.Now;
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
                    _currentMapManager.RemoveEntity(Entity);
                    _currentMapManager.GameTick -= OnGameTick;
                    _mapBattleManager.OnCreatedBattle -= OnCreatedBattle;
                    IsActive = false;
                }
            }

            OnDestroy?.Invoke(this, new System.EventArgs());
        }
    }

    public static class PlayerActionConstants
    {
        public const string Attack = "attack";
        public const string Join = "join";
        public const string Move = "move";
    }
}