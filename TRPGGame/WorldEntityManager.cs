using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Services;

namespace TRPGGame
{
    /// <summary>
    /// Responsible for managing all the entities across all maps.
    /// </summary>
    public class WorldEntityManager
    {
        private readonly WorldEntityFactory _worldEntityFactory;
        private readonly IWorldState _worldState;
        private ConcurrentDictionary<Guid, WorldEntity> _worldEntities;
        private ConcurrentDictionary<Guid, PlayerEntityManager> _playerEntityManagers;
        private readonly List<CombatEntity> _combatEntities;

        /// <summary>
        /// The time in minutes until a player is logged off due to inactivity.
        /// </summary>
        public const int InactiveTimeoutDuration = 10;

        public WorldEntityManager(WorldEntityFactory worldEntityFactory,
                                  IWorldState worldState)
        {
            _worldEntityFactory = worldEntityFactory;
            _worldState = worldState;
            _worldEntities = new ConcurrentDictionary<Guid, WorldEntity>();
            _playerEntityManagers = new ConcurrentDictionary<Guid, PlayerEntityManager>();
            _combatEntities = new List<CombatEntity>();
        }

        public async Task<IReadOnlyWorldEntity> GetOrAddEntityAsync(Guid ownerId)
        {
            bool success = _worldEntities.TryGetValue(ownerId, out WorldEntity entity);

            if (success) return entity;
            else
            {
                entity = await _worldEntityFactory.CreateAsync(ownerId);
                entity = _worldEntities.GetOrAdd(ownerId, entity);
                return entity;
            }
        }

        /// <summary>
        /// Gets a WorldEntity that has the given id.
        /// <para>Will return null if no entity was found with the given id.</para>
        /// </summary>
        /// <param name="entityId">The id of the WorldEntity to return.</param>
        /// <returns></returns>
        public IReadOnlyWorldEntity GetEntity(int entityId)
        {
            var entity = _playerEntityManagers.Values.Select(manager => manager.GetEntity())
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
            var entities = _playerEntityManagers.Values.Select(manager => manager.GetEntity())
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
                if ((int)inactivePeriod.TotalMinutes >= InactiveTimeoutDuration)
                {
                    _playerEntityManagers.TryRemove(idManagerPair.Key, out PlayerEntityManager manager);
                    manager.EndConnection();
                }
            }
        }

        /// <summary>
        /// Gets the PlayerEntityManager responsible for managing the current player's entity.
        /// </summary>
        /// <param name="ownerId">The Guid of the player to get the PlayerEntityManager for.</param>
        /// <returns></returns>
        public async Task<PlayerEntityManager> GetPlayerEntityManagerAsync(Guid ownerId)
        {
            var success = _playerEntityManagers.TryGetValue(ownerId, out PlayerEntityManager manager);
            if (success) return manager;

            var worldEntity = await _worldEntityFactory.CreateAsync(ownerId);
            return _playerEntityManagers.GetOrAdd(ownerId, new PlayerEntityManager(worldEntity, _worldState));
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

        [Obsolete]
        public void SaveCombatEntity(CombatEntity entity)
        {
            _combatEntities.Add(entity);
        }

        [Obsolete]
        public IEnumerable<CombatEntity> GetCombatEntities()
        {
            return _combatEntities;
        }
    }
}
