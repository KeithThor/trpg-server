using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.EventArgs;
using TRPGGame.Services;

namespace TRPGGame
{
    /// <summary>
    /// Responsible for managing all the entities across all maps.
    /// </summary>
    public class WorldEntityManager : IPlayerEntityManagerStore, IWorldEntityAssigner
    {
        private readonly IWorldEntityFactory _worldEntityFactory;
        private readonly PlayerEntityManagerFactory _playerEntityManagerFactory;
        private readonly IWorldState _worldState;
        private ConcurrentDictionary<Guid, WorldEntity> _worldEntities;
        private ConcurrentDictionary<Guid, PlayerEntityManager> _playerEntityManagers;
        private readonly List<CombatEntity> _combatEntities;

        public WorldEntityManager(IWorldEntityFactory worldEntityFactory,
                                  PlayerEntityManagerFactory playerEntityManagerFactory,
                                  IWorldState worldState)
        {
            _worldEntityFactory = worldEntityFactory;
            _playerEntityManagerFactory = playerEntityManagerFactory;
            _worldState = worldState;
            _worldEntities = new ConcurrentDictionary<Guid, WorldEntity>();
            _playerEntityManagers = new ConcurrentDictionary<Guid, PlayerEntityManager>();
            _combatEntities = new List<CombatEntity>();
        }

        /// <summary>
        /// Event invoked whenever a PlayerEntityManager is being created.
        /// </summary>
        public event EventHandler<PlayerEntityManagerCreatedEventArgs> OnPlayerEntityManagerCreated;

        /// <summary>
        /// Returns a WorldEntity owned by the player with the given id.
        /// </summary>
        /// <param name="ownerId">The id of the owner of the WorldEntity to retrieve.</param>
        /// <returns></returns>
        public IReadOnlyWorldEntity GetWorldEntity(Guid ownerId)
        {
            var success = _worldEntities.TryGetValue(ownerId, out WorldEntity entity);
            if (success) return entity;
            else return null;
        }

        /// <summary>
        /// Creates a new WorldEntity and assigns it to the PlayerEntityManager.
        /// </summary>
        /// <param name="ownerId">The id of the player who will own this new entity.</param>
        /// <param name="activeFormation">The Formation this WorldEntity represents.</param>
        /// <returns></returns>
        public IReadOnlyWorldEntity AssignWorldEntity(Guid ownerId, Formation activeFormation)
        {
            var newManager = _playerEntityManagerFactory.Create(ownerId, this);
            var success = _playerEntityManagers.TryAdd(ownerId, newManager);
            PlayerEntityManager manager;

            if (success)
            {
                manager = newManager;
                OnPlayerEntityManagerCreated?.Invoke(this, new PlayerEntityManagerCreatedEventArgs
                {
                    CreatedManager = newManager
                });
            }
            else
            {
                _playerEntityManagers.TryGetValue(ownerId, out manager);
            }

            manager.Entity = _worldEntityFactory.Create(ownerId, activeFormation);

            _worldEntities.AddOrUpdate(ownerId, manager.Entity, (id, entity) => manager.Entity);

            return manager.Entity;
        }

        /// <summary>
        /// Gets a WorldEntity that has the given id.
        /// <para>Will return null if no entity was found with the given id.</para>
        /// </summary>
        /// <param name="entityId">The id of the WorldEntity to return.</param>
        /// <returns></returns>
        public IReadOnlyWorldEntity GetEntity(int entityId)
        {
            var entity = _playerEntityManagers.Values.Select(manager => manager.Entity)
                                                     .FirstOrDefault(e => e.Id == entityId);
            return entity;
        }

        /// <summary>
        /// Gets and returns an enumerable of read-only World Entities that fulfill a given predicate.
        /// </summary>
        /// <param name="predicate">A function to filter the entities from.</param>
        /// <returns>An enumerable of read-only World Entities filtered by the predicate.</returns>
        public IEnumerable<IReadOnlyWorldEntity> GetEntities(Func<IReadOnlyWorldEntity, bool> predicate)
        {
            var entities = _playerEntityManagers.Values.Select(manager => manager.Entity)
                                                       .Where(entity => predicate(entity));
            return entities;
        }

        /// <summary>
        /// Ends the connection of and removes inactive player managers from memory if their inactivity timeout
        /// exceeds that of the InactivityTimeoutDuration specified.
        /// </summary>
        public void RemoveInactiveManagers()
        {
            foreach (var idManagerPair in _playerEntityManagers)
            {
                var inactivePeriod = DateTime.Now - idManagerPair.Value.LastAccessed;
                if ((int)inactivePeriod.TotalMinutes >= GameplayConstants.InactiveTimeoutDuration)
                {
                    _playerEntityManagers.TryRemove(idManagerPair.Key, out PlayerEntityManager manager);
                }
            }
        }

        /// <summary>
        /// Gets the PlayerEntityManager responsible for managing the current player's entity.
        /// </summary>
        /// <param name="ownerId">The Guid of the player to get the PlayerEntityManager for.</param>
        /// <returns></returns>
        public PlayerEntityManager GetPlayerEntityManager(Guid ownerId)
        {
            var success = _playerEntityManagers.TryGetValue(ownerId, out PlayerEntityManager manager);
            if (success)
            {
                if (manager.Entity == null)
                {
                    var entitySuccess = _worldEntities.TryGetValue(ownerId, out WorldEntity entity);
                    if (entitySuccess) manager.Entity = entity;
                }
                return manager;
            }
            else
            {
                if (TryCreateManager(ownerId, out PlayerEntityManager created))
                {
                    return created;
                }
                else return null;
            }
        }

        /// <summary>
        /// Tries to create a new PlayerEntityManager. If successful, returns true and the value of the out
        /// parameter manager is set to the newly created PlayerEntityManager.
        /// </summary>
        /// <param name="ownerId">The id of the user to create a PlayerEntityManager for.</param>
        /// <param name="manager">The manager that will be created by this function.</param>
        /// <returns>Returns true if a new PlayerEntityManager was successfully created.</returns>
        private bool TryCreateManager(Guid ownerId, out PlayerEntityManager manager)
        {
            manager = _playerEntityManagerFactory.Create(ownerId, this);
            var entitySuccess = _worldEntities.TryGetValue(ownerId, out WorldEntity entity);

            if (entitySuccess) manager.Entity = entity;
            else
            {
                manager = null;
                return false;
            }

            var success = _playerEntityManagers.TryAdd(ownerId, manager);

            if (success)
            {
                OnPlayerEntityManagerCreated?.Invoke(this, new PlayerEntityManagerCreatedEventArgs
                {
                    CreatedManager = manager
                });
            }

            return success;
        }

        /// <summary>
        /// Attempts to remove the PlayerEntityManager with the given guid.
        /// </summary>
        /// <param name="ownerId">The Guid representing the player id.</param>
        /// <returns>Returns the removed PlayerEntityManager, if any.</returns>
        public PlayerEntityManager RemovePlayerEntityManager(Guid ownerId)
        {
            _playerEntityManagers.TryRemove(ownerId, out PlayerEntityManager manager);
            return manager;
        }
    }
}
