using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly List<IBattleManager> _uniqueBattleManagers;
        private int _gameTicks = 0;

        public MapBattleManager(BattleManagerFactory battleManagerFactory)
        {
            _battleManagerFactory = battleManagerFactory;
            _battleManagers = new Dictionary<string, IBattleManager>();
            _aiBattleManagers = new Dictionary<int, IBattleManager>();
            _uniqueBattleManagers = new List<IBattleManager>();
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

            // Remove any duplicates
            attackers = attackers.Distinct().ToList();
            defenders = defenders.Distinct().ToList();

            if (!AreEntitiesValid(attackers, defenders)) return false;

            var manager = _battleManagerFactory.Create();

            var entitiesInBattle = attackers.Union(defenders).ToList();

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
            _uniqueBattleManagers.Add(manager);

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
            // Checks if a WorldEntity is in both lists
            if (attackers.Any(attacker => defenders.Contains(attacker))) return false;

            var combined = attackers.Concat(defenders).ToList();

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
        /// Tries to join the host WorldEntity.
        /// </summary>
        /// <param name="host">The WorldEntity whose battle to join.</param>
        /// <param name="joiner">The WorldEntity that is attempting to join the battle.</param>
        /// <param name="battleManager">The BattleManager instance responsible for handling the battle of the host.</param>
        /// <returns>Returns true if joining was successful.</returns>
        public bool TryJoinBattle(WorldEntity host, WorldEntity joiner, out IBattleManager battleManager)
        {
            var success = TryGetBattle(host, out battleManager);
            if (!success) return false;

            var manager = battleManager.JoinBattle(host, joiner);
            if (manager == null) return false;
            else
            {
                if (joiner.OwnerGuid == GameplayConstants.AiId) _aiBattleManagers.Add(joiner.Id, battleManager);
                else _battleManagers.Add(joiner.OwnerGuid.ToString(), battleManager);

                return true;
            }
        }

        /// <summary>
        /// Called on every game tick. Calls into each BattleManager every second to allow them to run functions
        /// based on time.
        /// </summary>
        public void OnGameTick()
        {
            _gameTicks++;

            // On every 1 second
            if (_gameTicks >= 1000 / GameplayConstants.GameTicksPerSecond)
            {
                _gameTicks = 0;
                Parallel.ForEach(_uniqueBattleManagers, (manager) =>
                {
                    manager.OnSecondElapsed();
                });
            }
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

            _uniqueBattleManagers.Remove(manager);
        }
    }
}
