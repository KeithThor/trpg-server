using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TRPGGame.Entities;
using TRPGGame.EventArgs;
using TRPGGame.Services;

namespace TRPGGame.Managers
{
    /// <summary>
    /// Manager responsible for creating and alerting listeners to battle events in one map.
    /// </summary>
    public class MapBattleManager : IMapBattleManager
    {
        private readonly BattleManagerFactory _battleManagerFactory;
        private readonly Dictionary<string, IBattleManager> _battleManagers;
        private readonly Dictionary<int, IBattleManager> _aiBattleManagers;

        public MapBattleManager(BattleManagerFactory battleManagerFactory)
        {
            _battleManagerFactory = battleManagerFactory;
            _battleManagers = new Dictionary<string, IBattleManager>();
            _aiBattleManagers = new Dictionary<int, IBattleManager>();
        }

        /// <summary>
        /// Event invoked whenever a Battle is created.
        /// </summary>
        public event EventHandler<CreatedBattleEventArgs> OnCreatedBattle;

        /// <summary>
        /// Creates a new battle using the given attacking and defending WorldEntities.
        /// 
        /// <para>Will send the resulting IBattleManager instance through the OnCreatedBattle event.</para>
        /// </summary>
        /// <param name="attackers">An IEnumerable containing the attacking WorldEntities.</param>
        /// <param name="defenders">An IEnumerable containing the defending WorldEntities.</param>
        public bool CreateBattle(IEnumerable<WorldEntity> attackers, IEnumerable<WorldEntity> defenders)
        {
            if (attackers == null || defenders == null || attackers.Count() <= 0 || defenders.Count() <= 0)
            {
                throw new Exception("Attempted to create battle with no attackers or defenders!");
            }

            if (AreEntitiesValid(attackers, defenders)) return false;

            var manager = _battleManagerFactory.Create();

            var entitiesInBattle = attackers.Union(defenders);

            var playersInBattle = entitiesInBattle.Where(entity => entity.OwnerGuid != GameplayConstants.AiId)
                                                  .Select(entity => entity.OwnerGuid)
                                                  .ToHashSet();

            var aiEntitiesInBattle = entitiesInBattle.Where(entity => entity.OwnerGuid == GameplayConstants.AiId)
                                                     .Select(entity => entity.Id)
                                                     .ToHashSet();

            foreach (var entity in entitiesInBattle)
            {
                if (entity.OwnerGuid == GameplayConstants.AiId) _aiBattleManagers.Add(entity.Id, manager);
                else _battleManagers.Add(entity.OwnerGuid.ToString(), manager);
            }

            manager.EndOfBattleEvent += OnEndOfBattle;

            manager.StartBattle(attackers.ToList(), defenders.ToList());

            OnCreatedBattle?.Invoke(this, new CreatedBattleEventArgs(manager, aiEntitiesInBattle, playersInBattle));

            return true;
        }

        /// <summary>
        /// Checks if the given attacking and defending WorldEntities are allowed to start a battle together.
        /// </summary>
        /// <param name="attackers">The WorldEntities that will start battle as attackers.</param>
        /// <param name="defenders">The WorldEntities that will start battle as defenders.</param>
        /// <returns>Returns true if the given attacking and defending WorldEntities are allowed to start a battle together</returns>
        private bool AreEntitiesValid(IEnumerable<WorldEntity> attackers, IEnumerable<WorldEntity> defenders)
        {
            var combined = attackers.Concat(defenders);

            return !combined.Any(entity => TryGetBattle(entity, out IBattleManager manager));
        }

        /// <summary>
        /// Tries to get the IBattleManager instance for the given WorldEntity.
        /// </summary>
        /// <param name="entity">The WorldEntity to get the battle for.</param>
        /// <param name="battleManager">The IBattleManager instance that is managing the player's battle.</param>
        /// <returns>Returns true if the IBattleManager was successfully retrieved.</returns>
        public bool TryGetBattle(IReadOnlyWorldEntity entity, out IBattleManager battleManager)
        {
            if (entity.OwnerGuid == GameplayConstants.AiId)
            {
                return _aiBattleManagers.TryGetValue(entity.Id, out battleManager);
            }
            return _battleManagers.TryGetValue(entity.OwnerGuid.ToString(), out battleManager);
        }

        /// <summary>
        /// Invoked at the end of a battle; Removes the IBattleManager instance from the list of BattleManagers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnEndOfBattle(object sender, EndOfBattleEventArgs args)
        {
            var manager = sender as IBattleManager;

            manager.EndOfBattleEvent -= OnEndOfBattle;

            foreach (var playerId in args.ParticipantIds)
            {
                _battleManagers.Remove(playerId);
            }

            foreach (var entityId in args.AiWorldEntityIds)
            {
                _aiBattleManagers.Remove(entityId);
            }
        }
    }
}
