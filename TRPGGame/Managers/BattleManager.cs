using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TRPGGame.Entities;
using TRPGGame.Entities.Combat;
using TRPGGame.EventArgs;
using TRPGGame.Repository;
using TRPGShared;

namespace TRPGGame.Managers
{
    /// <summary>
    /// Manager responsible for handling Battle-related functions for a single battle instance.
    /// </summary>
    public class BattleManager : IBattleManager
    {
        public BattleManager(IAbilityManager abilityManager,
                             IEquipmentManager equipmentManager,
                             IStatusEffectManager statusEffectManager,
                             IRepository<StatusEffect> statusEffectRepo)
        {
            _abilityManager = abilityManager;
            _equipmentManager = equipmentManager;
            _statusEffectManager = statusEffectManager;
            _statusEffectRepo = statusEffectRepo;
            _seed = new Random();
        }

        private Random _seed;
        private Battle _battle;
        private readonly IAbilityManager _abilityManager;
        private readonly IEquipmentManager _equipmentManager;
        private readonly IStatusEffectManager _statusEffectManager;
        private readonly IRepository<StatusEffect> _statusEffectRepo;
        private List<string> _participantIds;

        /// <summary>
        /// Event handler called after the start of turn events happen.
        /// </summary>
        public event EventHandler<StartOfTurnEventArgs> StartOfTurnEvent;

        /// <summary>
        /// Event handler called after the end of turn events happen.
        /// </summary>
        public event EventHandler<EndOfTurnEventArgs> EndOfTurnEvent;

        /// <summary>
        /// Event handler called to signal that the battle is over.
        /// </summary>
        public event EventHandler<EndOfBattleEventArgs> EndOfBattleEvent;

        /// <summary>
        /// Event handler called whenever a new Formation joins the battle.
        /// </summary>
        public event EventHandler<JoinBattleEventArgs> JoinBattleEvent;

        /// <summary>
        /// Starts a Battle instance between the attackers and defenders.
        /// <para>Returns null if a Battle instance already exists for this manager.</para>
        /// </summary>
        /// <param name="attackers">The Formations who initiated combat.</param>
        /// <param name="defenders">The Formations who are being initiated on.</param>
        /// <returns></returns>
        public IReadOnlyBattle StartBattle(List<Formation> attackers, List<Formation> defenders)
        {
            if (_battle != null) return null;
            _participantIds = attackers.Union(defenders)
                                       .Where(formation => formation.OwnerId != GameplayConstants.AiId)
                                       .Select(formation => formation.OwnerId.ToString())
                                       .ToList();

            var actionsPerFormation = new Dictionary<Formation, List<CombatEntity>>();
            var activeEntities = new Dictionary<int, IEnumerable<int>>();
            foreach (var attacker in attackers)
            {
                InitializeFormation(attacker);
                var activeE = ChooseActiveEntities(attacker);
                actionsPerFormation.Add(attacker, activeE);
                activeEntities.Add(attacker.Id, activeE.Select(e => e.Id));
            }

            var battle = new Battle
            {
                Attackers = attackers,
                Defenders = defenders,
                TurnExpiration = DateTime.Now.AddSeconds(GameplayConstants.SecondsPerTurn + 15),
                Round = 1,
                ActionsLeftPerFormation = actionsPerFormation,
                IsDefenderTurn = false
            };

            var nextDateStartDateTime = DateTime.Now.AddDays(1).Subtract(DateTime.Now.TimeOfDay);
            double millisecondsToWait = (nextDateStartDateTime - DateTime.Now).TotalMilliseconds;

            // Set timer to end the turn if time runs out.
            Timer timer = new Timer(
                                    (arg) => { EndTurn(); },
                                    null,
                                    (int)(_battle.TurnExpiration - DateTime.Now).TotalMilliseconds,
                                    0);

            _battle = battle;
            Task.Run(() => StartOfTurnEvent?.Invoke(this, new StartOfTurnEventArgs
            {
                TurnExpiration = battle.TurnExpiration,
                ActiveEntities = activeEntities,
                IsDefendersTurn = false,
                ParticipantIds = _participantIds
            }));

            return battle;
        }

        /// <summary>
        /// Joins the battle as an observer.
        /// </summary>
        /// <param name="observerId">The id of the user who is joining as an observer.</param>
        /// <returns></returns>
        public IReadOnlyBattle JoinBattle(Guid observerId)
        {
            _participantIds.Add(observerId.ToString());
            return _battle;
        }

        /// <summary>
        /// Adds a participant to the battle and returns the battle instance.
        /// <para>Returns null if no battles were found or the battle group is full.</para>
        /// </summary>
        /// <param name="participant">The Formation to add to the battle.</param>
        /// <param name="isAttacker">If true, will join the side of the attackers. If false, will join the defenders.</param>
        /// <returns></returns>
        public IReadOnlyBattle JoinBattle(Formation participant, bool isAttacker)
        {
            if (_battle == null) return null;

            InitializeFormation(participant);

            if (isAttacker)
            {
                if (_battle.Attackers.Count >= GameplayConstants.MaxFormationsPerSide) return null;
                _battle.Attackers.Add(participant);
            }
            else
            {
                if (_battle.Defenders.Count >= GameplayConstants.MaxFormationsPerSide) return null;
                _battle.Defenders.Add(participant);
            }

            if (isAttacker != _battle.IsDefenderTurn && _battle.Round == 1)
            {
                IncreaseActionPoints(participant);
                _battle.ActionsLeftPerFormation.Add(participant, ChooseActiveEntities(participant));
            }

            Task.Run(() =>
            {
                JoinBattleEvent?.Invoke(this, new JoinBattleEventArgs
                {
                    IsAttacker = isAttacker,
                    JoinedFormation = participant,
                    ParticipantIds = _participantIds
                });
            });

            return _battle;
        }

        /// <summary>
        /// Calls the end of battle for all participants.
        /// </summary>
        private void EndBattle()
        {
            foreach (var formation in _battle.Attackers)
            {
                SanitizeFormation(formation);
            }
            foreach (var formation in _battle.Defenders)
            {
                SanitizeFormation(formation);
            }

            EndOfBattleEvent?.Invoke(this, new EndOfBattleEventArgs
            {
                ParticipantIds = _participantIds
            });
        }

        /// <summary>
        /// Starts the turn for a group.
        /// </summary>
        private void StartTurn()
        {
            _battle.IsDefenderTurn = !_battle.IsDefenderTurn;
            IEnumerable<Formation> activeGroup = null;
            if (_battle.IsDefenderTurn) activeGroup = _battle.Defenders;
            else activeGroup = _battle.Attackers;

            List<DelayedAbility> startOfTurnAbilities = new List<DelayedAbility>();
            List<DelayedAbility> delayedAbilities = null;
            if (_battle.IsDefenderTurn) delayedAbilities = _battle.DefenderDelayedAbilities;
            else delayedAbilities = _battle.AttackerDelayedAbilities;

            // Apply start of turn DelayedAbilities
            for (int i = delayedAbilities.Count() - 1; i >= 0; i++)
            {
                delayedAbilities[i].TurnsLeft--;
                if (delayedAbilities[i].TurnsLeft == 0)
                {
                    startOfTurnAbilities.Add(delayedAbilities[i]);
                    delayedAbilities.RemoveAt(i);
                }
            }

            var affectedEntities = new List<IReadOnlyCombatEntity>();
            var activatedAbilities = new List<IReadOnlyAbility>();
            var activeEntityIds = new Dictionary<int, IEnumerable<int>>();

            if (startOfTurnAbilities != null && startOfTurnAbilities.Count() > 0)
            {
                foreach (var ability in startOfTurnAbilities)
                {
                    var entities = _abilityManager.Attack(ability);
                    foreach (var entity in entities)
                    {
                        if (!affectedEntities.Contains(entity)) affectedEntities.Add(entity);
                    }
                    activatedAbilities.Add(ability.BaseAbility);
                }
            }

            foreach (var formation in activeGroup)
            {
                IncreaseActionPoints(formation);
                var activeEntities = ChooseActiveEntities(formation);
                activeEntityIds.Add(formation.Id, activeEntities.Select(ce => ce.Id));
                _battle.ActionsLeftPerFormation.Add(formation, activeEntities);
            }

            _battle.TurnExpiration = DateTime.Now.AddSeconds(GameplayConstants.SecondsPerTurn);

            Task.Run(() => StartOfTurnEvent?.Invoke(this, new StartOfTurnEventArgs
            {
                DelayedAbilities = activatedAbilities,
                AffectedEntities = affectedEntities,
                TurnExpiration = _battle.TurnExpiration,
                ActiveEntities = activeEntityIds,
                IsDefendersTurn = _battle.IsDefenderTurn,
                ParticipantIds = _participantIds
            }));
        }

        /// <summary>
        /// Applies end of turn effects, then ends the current turn. Calls StartTurn after.
        /// </summary>
        private void EndTurn()
        {
            IEnumerable<DelayedAbility> endOfTurnAbilities = null;
            var affectedEntities = new List<IReadOnlyCombatEntity>();
            var activatedAbilities = new List<IReadOnlyAbility>();

            // Apply end of turn DelayedAbilities
            if (_battle.IsDefenderTurn)
            {
                endOfTurnAbilities = _battle.DefenderDelayedAbilities
                                           .Where(abi => abi.TurnsLeft == 0 && !abi.BaseAbility.ActivatesBeforeTurnStart);
            }
            else
            {
                endOfTurnAbilities = _battle.AttackerDelayedAbilities
                                           .Where(abi => abi.TurnsLeft == 0 && !abi.BaseAbility.ActivatesBeforeTurnStart);
            }
            if (endOfTurnAbilities != null && endOfTurnAbilities.Count() > 0)
            {
                foreach (var ability in endOfTurnAbilities)
                {
                    var entities = _abilityManager.Attack(ability);
                    foreach (var entity in entities)
                    {
                        if (!affectedEntities.Contains(entity)) affectedEntities.Add(entity);
                    }
                    activatedAbilities.Add(ability.BaseAbility);
                }
            }

            Task.Run(() => EndOfTurnEvent?.Invoke(this, new EndOfTurnEventArgs
            {
                DelayedAbilities = activatedAbilities,
                AffectedEntities = affectedEntities,
                ParticipantIds = _participantIds
            }));

            _battle.ActionsLeftPerFormation.Clear();
            StartTurn();
        }

        /// <summary>
        /// Performs an action and returns an IEnumerable of all of the CombatEntities affected by the action.
        /// <para>If the action was invalid, will return null.</para>
        /// </summary>
        /// <param name="action">The action to perform, containing data about the actor and the abilities used.</param>
        /// <returns></returns>
        public async Task<IEnumerable<IReadOnlyCombatEntity>> PerformActionAsync(BattleAction action)
        {
            IEnumerable<CombatEntity> affectedEntities;
            if (_battle == null) return null;

            var actorFormation = GetFormation(action.OwnerId, out bool isAttacker);
            if (actorFormation == null) return null;

            var actor = actorFormation.Positions.FirstOrDefaultTwoD(entity => entity.Id == action.ActorId);
            if (actor == null) return null;

            // If actor should not be acting, return null
            if (!_battle.ActionsLeftPerFormation.ContainsKey(actorFormation)) return null;
            if (!_battle.ActionsLeftPerFormation[actorFormation].Contains(actor)) return null;
            if (actor.StatusEffects.Any(se => se.BaseStatus.IsStunned)) return null;

            Ability ability = null;
            Item item = null;

            if (!action.IsDefending &&
                !action.IsFleeing)
            {
                if (action.IsUsingItem)
                {
                    item = actor.EquippedItems.FirstOrDefault(i => i.ConsumableAbility.Id == action.AbilityId);
                    if (item == null) item = actor.PlayerInventory.Items.FirstOrDefault(i => i.ConsumableAbility.Id == action.AbilityId);
                    if (item == null) return null;
                    
                    ability = item.ConsumableAbility;
                }
                else
                {
                    ability = actor.Abilities.FirstOrDefault(abi => abi.Id == action.AbilityId);
                    if (ability == null) return null;
                    if (ability.IsSpell && actor.StatusEffects.Any(se => se.BaseStatus.IsSilenced)) return null;
                    if (!ability.IsSpell && actor.StatusEffects.Any(se => se.BaseStatus.IsRestricted)) return null;
                }

                var targetFormation = GetFormation(action.TargetFormationId, out bool throwAway);
                if (ability.DelayedTurns > 0)
                {
                    var delayedAbility = _abilityManager.CreateDelayedAbility(actor, ability, action, targetFormation);
                    if (isAttacker) _battle.AttackerDelayedAbilities.Add(delayedAbility);
                    else _battle.DefenderDelayedAbilities.Add(delayedAbility);
                    return new List<CombatEntity> { actor };
                }
                else
                {
                    affectedEntities = _abilityManager.Attack(actor, ability, action, targetFormation);
                    if (affectedEntities == null) return null;
                }

                _battle.ActionsLeftPerFormation[actorFormation].Remove(actor);
                if (item != null) _equipmentManager.ReduceCharges(actor, item);
            }
            // Is defending or fleeing
            else
            {
                var statusEffects = await _statusEffectRepo.GetDataAsync();
                StatusEffect statusEffect;
                if (action.IsDefending)
                {
                    statusEffect = statusEffects.FirstOrDefault(se => se.Id == GameplayConstants.DefendingStatusEffectId);
                }
                else statusEffect = statusEffects.FirstOrDefault(se => se.Id == GameplayConstants.FleeingStatusEffectId);

                _statusEffectManager.Apply(actor, actor, statusEffect);
                _battle.ActionsLeftPerFormation[actorFormation].Remove(actor);
                affectedEntities = new List<CombatEntity> { actor };

                if (action.IsDefending)
                {
                    var nextActiveEntity = GetNextActiveEntity(actorFormation, _battle.ActionsLeftPerFormation[actorFormation]);
                    if (nextActiveEntity != null) _battle.ActionsLeftPerFormation[actorFormation].Add(nextActiveEntity);
                }
            }

            return affectedEntities;
        }

        /// <summary>
        /// Gets a Formation from the Battle instance given the Guid of the user who owns the Formation.
        /// Also returns whether the Formation is part of the attackers group or defenders.
        /// </summary>
        /// <param name="ownerId">The Guid used to identify the owner of the Formation.</param>
        /// <param name="isAttacker">Returns true if the Formation is part of the attackers group. False if the Formation
        /// is part of the defenders group.</param>
        /// <returns></returns>
        private Formation GetFormation(Guid ownerId, out bool isAttacker)
        {
            isAttacker = true;
            var formation = _battle.Attackers.FirstOrDefault(form => form.OwnerId == ownerId);
            if (formation == null)
            {
                isAttacker = false;
                formation = _battle.Defenders.FirstOrDefault(form => form.OwnerId == ownerId);
            }
            return formation;
        }

        /// <summary>
        /// Gets a Formation from the Battle instance given the id of the Formation.
        /// Also returns whether the Formation is part of the attackers group or defenders.
        /// </summary>
        /// <param name="ownerId">The id used to identify the Formation to return.</param>
        /// <param name="isAttacker">Returns true if the Formation is part of the attackers group. False if the Formation
        /// is part of the defenders group.</param>
        /// <returns></returns>
        private Formation GetFormation(int formationId, out bool isAttacker)
        {
            isAttacker = true;
            var formation = _battle.Attackers.FirstOrDefault(form => form.Id == formationId);
            if (formation == null)
            {
                isAttacker = false;
                formation = _battle.Defenders.FirstOrDefault(form => form.Id == formationId);
            }
            return formation;
        }

        /// <summary>
        /// Brings a Formation to a fresh state ready for the start of battle.
        /// </summary>
        /// <param name="formation">The Formation to initialize.</param>
        private void InitializeFormation(Formation formation)
        {
            foreach (var row in formation.Positions)
            {
                foreach (var entity in row)
                {
                    entity.Resources.CurrentActionPoints = 0;

                    // Temporary, remove later
                    entity.Resources.CurrentHealth = entity.Resources.MaxHealth;
                    entity.Resources.CurrentMana = entity.Resources.MaxMana;

                    IncreaseActionPoints(entity);
                }
            }
        }

        /// <summary>
        /// Removes all combat effects from all CombatEntities in a formation to prepare to exit combat.
        /// </summary>
        /// <param name="formation">The Formation to sanitize.</param>
        private void SanitizeFormation(Formation formation)
        {
            foreach (var row in formation.Positions)
            {
                foreach (var entity in row)
                {
                    entity.Resources.CurrentActionPoints = 0;
                    _statusEffectManager.RemoveAll(entity);
                }
            }
        }

        /// <summary>
        /// Given a Formation, returns a List of the CombatEntities with the most action points and are
        /// also able to act in battle.
        /// <para>Returns a maximum of GameplayConstants.MaxActionsPerTurn entities.</para>
        /// </summary>
        /// <param name="formation">The Formation to get active entities from.</param>
        /// <returns></returns>
        private List<CombatEntity> ChooseActiveEntities(Formation formation)
        {
            var candidates = formation.Positions.WhereTwoD(entity =>
            {
                if (entity.Resources.CurrentActionPoints <= 0 || entity.Resources.CurrentHealth <= 0) return false;
                if (entity.StatusEffects.Any(se => se.BaseStatus.IsStunned)) return false;
                return true;
            });

            candidates = candidates.OrderByDescending(entity => entity.Resources.CurrentActionPoints);
            if (candidates.Count() <= GameplayConstants.MaxActionsPerTurn) return candidates.ToList();
            else return candidates.Take(GameplayConstants.MaxActionsPerTurn).ToList();
        }

        /// <summary>
        /// Gets the next active entity if an entity passes its turn to another.
        /// <para>Returns null if there are no entities that can act.</para>
        /// </summary>
        /// <param name="formation">The Formation to get the next active entity from.</param>
        /// <param name="activeEntities">The entities that are currently active.</param>
        /// <returns></returns>
        private CombatEntity GetNextActiveEntity(Formation formation, List<CombatEntity> activeEntities)
        {
            var candidates = formation.Positions.WhereTwoD(ent =>
            {
                if (activeEntities.Contains(ent)) return false;
                if (ent.Resources.CurrentActionPoints <= 0 || ent.Resources.CurrentHealth <= 0) return false;
                if (ent.StatusEffects.Any(se =>
                {
                    if (se.BaseStatus.IsStunned) return true;
                    if (se.BaseStatus.Id == GameplayConstants.DefendingStatusEffectId) return true;
                    if (se.BaseStatus.Id == GameplayConstants.FleeingStatusEffectId) return true;
                    return false;
                })) return false;

                return true;
            });

            if (candidates == null || candidates.Count() == 0) return null;
            return candidates.OrderByDescending(entity => entity.Resources.CurrentActionPoints).First();
        }

        /// <summary>
        /// Increases action points for a CombatEntity based on its action point regeneration.
        /// </summary>
        /// <param name="entity">The CombatEntity whose action points should be increased.</param>
        private void IncreaseActionPoints(CombatEntity entity)
        {
            if (entity == null) return;
            if (entity.Resources.CurrentHealth <= 0 || entity.StatusEffects.Any(se => se.BaseStatus.IsStunned)) return;

            int randPoints = _seed.Next(0, entity.SecondaryStats.BonusActionPoints);
            int total = GameplayConstants.ActionPointsPerTurn + randPoints;
            total += total * entity.SecondaryStats.BonusActionPointsPercentage / 100;
            entity.Resources.CurrentActionPoints += total;
        }

        /// <summary>
        /// Increases action points for every CombatEntity that is able in a Formation.
        /// </summary>
        /// <param name="formation">The Formation to get the CombatEntities from.</param>
        private void IncreaseActionPoints(Formation formation)
        {
            foreach (var row in formation.Positions)
            {
                foreach (var entity in row)
                {
                    IncreaseActionPoints(entity);
                }
            }
        }

        /// <summary>
        /// Returns the current battle instance.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyBattle GetBattle()
        {
            return _battle;
        }

        /// <summary>
        /// Returns the ids of all players in the current battle.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetParticipantIds()
        {
            return _participantIds.Where(id => id != GameplayConstants.AiId.ToString());
        }
    }
}
