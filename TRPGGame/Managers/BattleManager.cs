﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TRPGGame.Entities;
using TRPGGame.Entities.Combat;
using TRPGGame.EventArgs;
using TRPGGame.Managers.Combat.Interfaces;
using TRPGGame.Repository;
using TRPGGame.Static;
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
                             IRepository<StatusEffect> statusEffectRepo,
                             ICombatAi combatAi)
        {
            _abilityManager = abilityManager;
            _equipmentManager = equipmentManager;
            _statusEffectManager = statusEffectManager;
            _statusEffectRepo = statusEffectRepo;
            _combatAi = combatAi;
            _seed = new Random();

            _timer = new Timer();
            _timer.AutoReset = false;
            _timer.Interval = GameplayConstants.EndOfTurnDelayInSeconds;
            _timer.Elapsed += (sender, args) => StartTurn();
        }

        private Random _seed;
        private Battle _battle;
        private readonly object _key = new object();
        private bool _isBattleActive = false;
        private Timer _timer;
        private readonly IAbilityManager _abilityManager;
        private readonly IEquipmentManager _equipmentManager;
        private readonly IStatusEffectManager _statusEffectManager;
        private readonly IRepository<StatusEffect> _statusEffectRepo;
        private readonly ICombatAi _combatAi;
        private List<string> _participantIds;
        private List<int> _aiParticipantIds;
        private int _numOfAttackers = 0;
        private int _numOfDefenders = 0;
        private int _secondsElapsedInTurn = 0;

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
        /// Event handler called whenever a BattleAction is successfully performed.
        /// </summary>
        public event EventHandler<SuccessfulActionEventArgs> SuccessfulActionEvent;

        /// <summary>
        /// Event invoked whenever a second has elapsed. Will make the Ai act and will end the turn when
        /// the amount of seconds elapsed is greater than or equal to the seconds per turn.
        /// </summary>
        public void OnSecondElapsed()
        {
            lock (_key)
            {
                if (!_isBattleActive) return;
                _secondsElapsedInTurn++;
                if (_battle != null && DateTime.Now >= _battle.TurnExpiration)
                {
                    _secondsElapsedInTurn = 0;
                    _battle.TurnExpiration = DateTime.MaxValue;

                    EndTurn();
                }

                // Every 2 seconds make the Ai perform an action
                if (_secondsElapsedInTurn % 2 == 0)
                {
                    PerformAiAction();
                }
            }
        }

        /// <summary>
        /// Performs an action for one Ai CombatEntity that is available to act this turn.
        /// </summary>
        private void PerformAiAction()
        {
            CombatAiDecision decision = null;
            IEnumerable<Formation> myAllies = null;
            IEnumerable<Formation> myEnemies = null;
            IEnumerable<CombatEntity> affectedEntities = null;
            CombatEntity nextActiveEntity = null;

            lock (_key)
            {
                var available = _battle.ActionsLeftPerFormation.Where(kvp => kvp.Key.OwnerId == GameplayConstants.AiId)
                                                           .Where(kvp => kvp.Value != null && kvp.Value.Count > 0)
                                                           .FirstOrDefault();
                // If no available ai actions left in turn
                if (available.Equals(default(KeyValuePair<Formation, List<CombatEntity>>))) return;

                var myFormation = available.Key;
                var myEntity = available.Value.FirstOrDefault();

                // If defenders turn, ai is a defender, all defenders are allies and attackers are enemies
                if (_battle.IsDefenderTurn)
                {
                    myAllies = _battle.Defenders;
                    myEnemies = _battle.Attackers;
                }
                else
                {
                    myAllies = _battle.Attackers;
                    myEnemies = _battle.Defenders;
                }

                decision = _combatAi.MakeDecision(myFormation, myEntity, myAllies, myEnemies);
                if (decision == null) return;

                if (decision.IsDefending)
                {
                    affectedEntities = PerformDefend(myEntity, myFormation, out nextActiveEntity);
                }
                else
                {
                    var result = PerformAiDecision(decision, myFormation);
                    affectedEntities = result.AffectedEntities;

                    CheckForDeadEntities(affectedEntities, decision.TargetFormation);
                }
            }

            Task.Run(() => SuccessfulActionEvent.Invoke(this, new SuccessfulActionEventArgs
            {
                Ability = decision.Ability,
                Actor = decision.Actor,
                AffectedEntities = affectedEntities,
                ParticipantIds = _participantIds,
                NextActiveEntityId = nextActiveEntity != null ? nextActiveEntity.Id : -1
            })).Wait();
        }

        /// <summary>
        /// Starts a Battle instance between the attackers and defenders.
        /// <para>Returns null if a Battle instance already exists for this manager.</para>
        /// </summary>
        /// <param name="attackers">The WorldEntities who initiated combat.</param>
        /// <param name="defenders">The WorldEntities who are being initiated on.</param>
        /// <returns></returns>
        public IReadOnlyBattle StartBattle(List<WorldEntity> attackers, List<WorldEntity> defenders)
        {
            Battle battle = null;
            List<ActiveEntities> activeEntities = null;
            
            lock (_key)
            {
                if (_battle != null) return null;
                if (_isBattleActive) return null;

                _participantIds = attackers.Union(defenders)
                                       .Where(entity => entity.OwnerGuid != GameplayConstants.AiId)
                                       .Select(entity => entity.OwnerGuid.ToString())
                                       .ToList();

                _aiParticipantIds = attackers.Union(defenders)
                                             .Where(entity => entity.OwnerGuid == GameplayConstants.AiId)
                                             .Select(entity => entity.Id)
                                             .ToList();

                var attackingFormations = attackers.Select(entity => entity.ActiveFormation)
                                                   .ToList();
                var defendingFormations = defenders.Select(entity => entity.ActiveFormation)
                                                   .ToList();

                var actionsPerFormation = new Dictionary<Formation, List<CombatEntity>>();
                activeEntities = new List<ActiveEntities>();
                foreach (var attacker in attackingFormations)
                {
                    InitializeFormation(attacker, true);
                    var activeE = ChooseActiveEntities(attacker);
                    actionsPerFormation.Add(attacker, activeE);
                    activeEntities.Add(new ActiveEntities
                    {
                        EntityIds = activeE.Select(e => e.Id).ToList(),
                        FormationId = attacker.Id,
                        OwnerId = attacker.OwnerId
                    });
                }

                foreach (var defender in defendingFormations)
                {
                    InitializeFormation(defender, false, true);
                }

                var nextTurnStartDate = DateTime.Now.AddSeconds(GameplayConstants.SecondsPerTurn);

                battle = new Battle
                {
                    Attackers = attackingFormations,
                    Defenders = defendingFormations,
                    TurnExpiration = nextTurnStartDate,
                    Round = 1,
                    ActionsLeftPerFormation = actionsPerFormation,
                    IsDefenderTurn = false
                };

                _battle = battle;
                _isBattleActive = true;
            }

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
            lock (_key)
            {
                _participantIds.Add(observerId.ToString());
                return _battle;
            }
        }

        /// <summary>
        /// Adds a participant to the battle and returns the battle instance.
        /// <para>Returns null if no battles were found or the battle group is full.</para>
        /// </summary>
        /// <param name="participant">The WorldEntity to add to the battle.</param>
        /// <param name="isAttacker">If true, will join the side of the attackers. If false, will join the defenders.</param>
        /// <returns></returns>
        public IReadOnlyBattle JoinBattle(WorldEntity participant, bool isAttacker)
        {
            List<ActiveEntities> activeEntities = null;

            lock (_key)
            {
                if (_battle == null) return null;
                if (!_isBattleActive) return null;

                // Add to list of attackers if joining as an attacker
                if (isAttacker)
                {
                    if (_battle.Attackers.Count >= GameplayConstants.MaxFormationsPerSide) return null;
                    _battle.Attackers.Add(participant.ActiveFormation);
                }
                else
                {
                    if (_battle.Defenders.Count >= GameplayConstants.MaxFormationsPerSide) return null;
                    _battle.Defenders.Add(participant.ActiveFormation);
                }

                InitializeFormation(participant.ActiveFormation, isAttacker);

                activeEntities = new List<ActiveEntities>();

                // If this is the first round and it is the turn of the side the formation is joining, allow it to act
                // this turn
                if (isAttacker != _battle.IsDefenderTurn && _battle.Round == 1)
                {
                    IncreaseActionPoints(participant.ActiveFormation);
                    var aEntities = ChooseActiveEntities(participant.ActiveFormation);
                    _battle.ActionsLeftPerFormation.Add(participant.ActiveFormation, aEntities);

                    activeEntities.Add(new ActiveEntities
                    {
                        EntityIds = aEntities.Select(entity => entity.Id).ToList(),
                        FormationId = participant.ActiveFormation.Id,
                        OwnerId = participant.OwnerGuid
                    });
                }

                if (participant.OwnerGuid == GameplayConstants.AiId) _aiParticipantIds.Add(participant.Id);
                else _participantIds.Add(participant.OwnerGuid.ToString());
            }

            Task.Run(() =>
            {
                JoinBattleEvent?.Invoke(this, new JoinBattleEventArgs
                {
                    IsAttacker = isAttacker,
                    JoinedFormation = participant.ActiveFormation,
                    ParticipantIds = _participantIds,
                    ActiveEntities = activeEntities
                });
            });

            return _battle;
        }

        /// <summary>
        /// Attempts to join the side of the provided host WorldEntity. Returns the Battle instance if successful.
        /// <para>Returns null if the join was invalid.</para>
        /// </summary>
        /// <param name="host">The WorldEntity to join the side of.</param>
        /// <param name="joiner">The WorldEntity joining the battle.</param>
        /// <returns></returns>
        public IReadOnlyBattle JoinBattle(WorldEntity host, WorldEntity joiner)
        {
            bool isAttacker = false;

            lock (_key)
            {
                if (!_isBattleActive) return null;

                if (_battle.Attackers.Any(formation => formation == host.ActiveFormation)) isAttacker = true;
                else if (_battle.Defenders.Any(formation => formation == host.ActiveFormation)) isAttacker = false;

                // No formation was found for the given host's formation
                else return null;
            }

            return JoinBattle(joiner, isAttacker);
        }

        /// <summary>
        /// Calls the end of battle for all participants.
        /// </summary>
        private void EndBattle()
        {
            bool attackersWin = false;

            lock (_key)
            {
                if (!_isBattleActive) return;
                _isBattleActive = false;

                foreach (var formation in _battle.Attackers)
                {
                    SanitizeFormation(formation);
                }
                foreach (var formation in _battle.Defenders)
                {
                    SanitizeFormation(formation);
                }

                attackersWin = _numOfDefenders <= 0;
            }
            
            Task.Run(() => EndOfBattleEvent(this, new EndOfBattleEventArgs
            {
                ParticipantIds = _participantIds,
                AiWorldEntityIds = _aiParticipantIds,
                DidAttackersWin = attackersWin
            }));
        }

        /// <summary>
        /// Starts the turn for a group.
        /// </summary>
        private void StartTurn()
        {
            var affectedEntities = new List<IReadOnlyCombatEntity>();
            var activatedAbilities = new List<IReadOnlyAbility>();
            var activeEntities = new List<ActiveEntities>();
            var actionPointData = new Dictionary<int, IEnumerable<ActionPointData>>();

            lock (_key)
            {
                if (!_isBattleActive) return;

                _battle.IsDefenderTurn = !_battle.IsDefenderTurn;
                IEnumerable<Formation> activeGroup = null;
                if (_battle.IsDefenderTurn) activeGroup = _battle.Defenders;
                else activeGroup = _battle.Attackers;

                List<DelayedAbility> startOfTurnAbilities = new List<DelayedAbility>();
                List<DelayedAbility> delayedAbilities = null;
                if (_battle.IsDefenderTurn) delayedAbilities = _battle.DefenderDelayedAbilities;
                else delayedAbilities = _battle.AttackerDelayedAbilities;

                // Decrement turns left and queues delayed abilities to be activated
                for (int i = delayedAbilities.Count() - 1; i >= 0; i++)
                {
                    delayedAbilities[i].TurnsLeft--;
                    if (delayedAbilities[i].TurnsLeft == 0)
                    {
                        startOfTurnAbilities.Add(delayedAbilities[i]);
                        delayedAbilities.RemoveAt(i);
                    }
                }

                // Activates delayed abilities
                if (startOfTurnAbilities != null && startOfTurnAbilities.Count() > 0)
                {
                    foreach (var ability in startOfTurnAbilities)
                    {
                        var entities = _abilityManager.PerformDelayedAbility(ability);
                        affectedEntities.AddRange(entities);
                        activatedAbilities.Add(ability.BaseAbility);
                    }
                }

                // Increase action points, choose active entities, and apply status effects to all formations that are active
                foreach (var formation in activeGroup)
                {
                    affectedEntities.AddRange(ApplyStatusEffects(formation));

                    var data = IncreaseActionPoints(formation);
                    actionPointData.Add(formation.Id, data);

                    var actives = ChooseActiveEntities(formation);
                    activeEntities.Add(new ActiveEntities
                    {
                        EntityIds = actives.Select(ae => ae.Id).ToList(),
                        FormationId = formation.Id,
                        OwnerId = formation.OwnerId
                    });
                    _battle.ActionsLeftPerFormation.Add(formation, actives);
                }
                affectedEntities = affectedEntities.Distinct().ToList();
                _battle.TurnExpiration = DateTime.Now.AddSeconds(GameplayConstants.SecondsPerTurn);

                if (!_battle.IsDefenderTurn) _battle.Round++;
            }

            Task.Run(() => StartOfTurnEvent?.Invoke(this, new StartOfTurnEventArgs
            {
                DelayedAbilities = activatedAbilities,
                AffectedEntities = affectedEntities,
                ActionPointData = actionPointData,
                TurnExpiration = _battle.TurnExpiration,
                ActiveEntities = activeEntities,
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

            lock (_key)
            {
                // Apply end of turn DelayedAbilities
                if (_battle.IsDefenderTurn)
                {
                    endOfTurnAbilities = _battle.DefenderDelayedAbilities
                                               .Where(abi => abi.TurnsLeft == 0 && !abi.BaseAbility.ActivatesBeforeTurnStart)
                                               .ToList();
                }
                else
                {
                    endOfTurnAbilities = _battle.AttackerDelayedAbilities
                                               .Where(abi => abi.TurnsLeft == 0 && !abi.BaseAbility.ActivatesBeforeTurnStart)
                                               .ToList();
                }
                if (endOfTurnAbilities != null && endOfTurnAbilities.Count() > 0)
                {
                    foreach (var ability in endOfTurnAbilities)
                    {
                        var entities = _abilityManager.PerformDelayedAbility(ability);
                        foreach (var entity in entities)
                        {
                            if (!affectedEntities.Contains(entity)) affectedEntities.Add(entity);
                        }
                        activatedAbilities.Add(ability.BaseAbility);
                    }
                }

                _battle.ActionsLeftPerFormation.Clear();
            }

            // EndOfTurnEvent called here to give extra time to send message to clients
            Task.Run(() => EndOfTurnEvent?.Invoke(this, new EndOfTurnEventArgs
            {
                DelayedAbilities = activatedAbilities,
                AffectedEntities = affectedEntities,
                ParticipantIds = _participantIds
            }));

            lock (_key)
            {
                if (_numOfAttackers <= 0 || _numOfDefenders <= 0)
                {
                    EndBattle();
                }
                else
                {
                    // Call start turn after a delay
                    var dueDate = DateTime.Now.AddSeconds(GameplayConstants.EndOfTurnDelayInSeconds);
                    
                    _timer.Start();
                }
            }
        }

        /// <summary>
        /// Performs an action and returns true if the action was a success.
        /// </summary>
        /// <param name="action">The action to perform, containing data about the actor and the abilities used.</param>
        /// <returns></returns>
        public async Task<BattleActionResult> PerformActionAsync(BattleAction action)
        {
            IEnumerable<CombatEntity> affectedEntities = null;
            CombatEntity nextActiveEntity = null;
            Ability ability = null;
            CombatEntity actor = null;
            var result = new BattleActionResult { IsSuccess = true };

            lock (_key)
            {
                if (_battle == null)
                {
                    result.FailureReason = BattleErrorWriter.WriteNotInitiated();
                    result.IsSuccess = false;
                    return result;
                }

                var actorFormation = GetFormation(action.OwnerId, out bool isAttacker);
                if (actorFormation == null)
                {
                    result.FailureReason = BattleErrorWriter.WriteNotParticipating();
                    result.IsSuccess = false;
                    return result;
                }

                actor = actorFormation.Positions.FirstOrDefaultTwoD(entity => entity != null && entity.Id == action.ActorId);
                if (actor == null)
                {
                    result.FailureReason = BattleErrorWriter.WriteActorNotFound();
                    result.IsSuccess = false;
                    return result;
                }

                // Not player's turn
                if (_battle.IsDefenderTurn != !isAttacker)
                {
                    result.FailureReason = BattleErrorWriter.WriteNotPlayersTurn();
                    result.IsSuccess = false;
                    return result;
                }

                if (!_battle.ActionsLeftPerFormation.ContainsKey(actorFormation))
                {
                    result.FailureReason = BattleErrorWriter.WriteNoMoreActions();
                    result.IsSuccess = false;
                    return result;
                }

                if (!_battle.ActionsLeftPerFormation[actorFormation].Contains(actor))
                {
                    result.FailureReason = BattleErrorWriter.WriteEntityCannotAct(actor);
                    result.IsSuccess = false;
                    return result;
                }

                if (actor.StatusEffects.Any(se => se.BaseStatus.IsStunned))
                {
                    result.FailureReason = BattleErrorWriter.WriteEntityIsStunned(actor);
                    result.IsSuccess = false;
                    return result;
                }

                if (!action.IsDefending &&
                    !action.IsFleeing)
                {
                    affectedEntities = PerformAbility(action,
                                                      actor,
                                                      actorFormation,
                                                      isAttacker,
                                                      ability,
                                                      ref result);

                    if (affectedEntities == null) return result;
                }
                // Is defending or fleeing
                else
                {
                    if (action.IsDefending)
                    {
                        affectedEntities = PerformDefend(actor, actorFormation, out nextActiveEntity);
                    }
                    else if (action.IsFleeing)
                    {
                        affectedEntities = PerformFlee(actor, actorFormation);
                    }
                }
            }

            await Task.Run(() => SuccessfulActionEvent.Invoke(this, new SuccessfulActionEventArgs
            {
                Ability = ability,
                Action = action,
                Actor = actor,
                AffectedEntities = affectedEntities,
                ParticipantIds = _participantIds,
                NextActiveEntityId = nextActiveEntity != null ? nextActiveEntity.Id : -1
            }));

            return result;
        }

        /// <summary>
        /// Performs an ability using the specified parameters.
        /// </summary>
        /// <param name="action">The object containing the original battle commands.</param>
        /// <param name="actor">The CombatEntity performing the ability.</param>
        /// <param name="actorFormation">The Formation of the CombatEntity performing the Ability.</param>
        /// <param name="isAttacker">Set to true if the CombatEntity is on the attacking side in battle.</param>
        /// <param name="ability">The Ability being performed.</param>
        /// <param name="result">A reference to the BattleActionResult object.</param>
        /// <returns>Returns an IEnumerable of CombatEntities affected by the Ability.</returns>
        private IEnumerable<CombatEntity> PerformAbility(BattleAction action,
                                                         CombatEntity actor,
                                                         Formation actorFormation,
                                                         bool isAttacker,
                                                         Ability ability,
                                                         ref BattleActionResult result)
        {
            IEnumerable<CombatEntity> affectedEntities;
            Item item = null;

            // If using an item, prepare item to have it's charges deducted later
            if (action.IsUsingItem)
            {
                item = actor.EquippedItems.FirstOrDefault(i => i.ConsumableAbility.Id == action.AbilityId);
                if (item == null) item = actor.PlayerInventory
                                              .Items
                                              .FirstOrDefault(i => i != null && i.ConsumableAbility.Id == action.AbilityId);

                if (item == null)
                {
                    result.FailureReason = BattleErrorWriter.WriteItemDoesntExist(actor);
                    result.IsSuccess = false;
                    return null;
                }

                ability = item.ConsumableAbility;
            }
            // Not an item, check to see if character is restricted from using Ability
            else
            {
                ability = actor.Abilities.FirstOrDefault(abi => abi.Id == action.AbilityId);
                if (ability == null)
                {
                    result.FailureReason = BattleErrorWriter.WriteAbilityDoesntExist(actor);
                    result.IsSuccess = false;
                    return null;
                }

                if (ability.IsSpell && actor.StatusEffects.Any(se => se.BaseStatus.IsSilenced))
                {
                    result.FailureReason = BattleErrorWriter.WriteEntityIsSilenced(actor, ability);
                    result.IsSuccess = false;
                    return null;
                }

                if (!ability.IsSpell && actor.StatusEffects.Any(se => se.BaseStatus.IsRestricted))
                {
                    result.FailureReason = BattleErrorWriter.WriteEntityIsRestricted(actor, ability);
                    result.IsSuccess = false;
                    return null;
                }
            }

            var targetFormation = GetFormation(action.TargetFormationId, out bool throwAway);
            // If the ability has a delay, create a DelayedAbility
            if (ability.DelayedTurns > 0)
            {
                var delayedAbilityResult = _abilityManager.CreateDelayedAbility(actor, ability, action, targetFormation);
                if (delayedAbilityResult.DelayedAbility == null)
                {
                    result.FailureReason = delayedAbilityResult.FailureReason;
                    result.IsSuccess = false;
                    return null;
                }

                if (isAttacker) _battle.AttackerDelayedAbilities.Add(delayedAbilityResult.DelayedAbility);
                else _battle.DefenderDelayedAbilities.Add(delayedAbilityResult.DelayedAbility);
                affectedEntities = new List<CombatEntity> { actor };
            }
            // Not a delayed ability, try to apply effects immediately
            else
            {
                var abilityResult = _abilityManager.PerformAbility(actor, ability, action.TargetPosition, targetFormation);
                if (abilityResult.FailureReason != null)
                {
                    result.FailureReason = abilityResult.FailureReason;
                    result.IsSuccess = false;
                    return null;
                }

                affectedEntities = abilityResult.AffectedEntities;
            }

            CheckForDeadEntities(affectedEntities, targetFormation);

            // Action was successful, remove the actor from characters free to act
            _battle.ActionsLeftPerFormation[actorFormation].Remove(actor);

            // If Ability was granted by an item, reduce the charges of the item
            if (item != null) _equipmentManager.ReduceCharges(actor, item);

            

            return affectedEntities;
        }

        /// <summary>
        /// Checks if any of the given CombatEntities are dead.
        /// </summary>
        /// <param name="entities">The CombatEntities to check.</param>
        /// <param name="formation">The Formation the CombatEntities belong to.</param>
        private void CheckForDeadEntities(IEnumerable<CombatEntity> entities, Formation formation)
        {
            // If any of the affected entities died, remove them from the number of living characters
            var deadEntities = entities.Where(entity => entity.Resources.CurrentHealth <= 0).ToList();
            if (deadEntities.Count == 0) return;

            foreach (var entity in deadEntities)
            {
                if (_battle.Attackers.Contains(formation))
                {
                    if (formation.Positions.ContainsTwoD(entity)) _numOfAttackers--;
                    else _numOfDefenders--;
                }
                else
                {
                    if (formation.Positions.ContainsTwoD(entity)) _numOfDefenders--;
                    else _numOfAttackers--;
                }
            }
        }

        /// <summary>
        /// Performs a flee action for the provided CombatEntity.
        /// </summary>
        /// <param name="actor">The CombatEntity performing the flee action.</param>
        /// <param name="actorFormation">The Formation of the CombatEntity performing the flee action.</param>
        /// <returns>Contains an IEnumerable of CombatEntities affected by the flee or defend command.</returns>
        private IEnumerable<CombatEntity> PerformFlee(CombatEntity actor, Formation actorFormation)
        {
            IEnumerable<CombatEntity> affectedEntities = null;

            var task = _statusEffectRepo.GetDataAsync();
            task.Wait();

            var statusEffects = task.Result;
            var fleeStatus = statusEffects.FirstOrDefault(se => se.Id == GameplayConstants.FleeingStatusEffectId);

            lock (_key)
            {
                _statusEffectManager.Apply(actor, actor, fleeStatus);
                _battle.ActionsLeftPerFormation[actorFormation].Remove(actor);
            }
            
            affectedEntities = new List<CombatEntity> { actor };

            return affectedEntities;
        }

        /// <summary>
        /// Performs an ability based on the ai's decision data.
        /// </summary>
        /// <param name="decision">The data object containing the ai's decision of action.</param>
        /// <param name="aiFormation">The Formation of the Ai.</param>
        /// <returns>Returns the result of the ability being performed.</returns>
        private AbilityResult PerformAiDecision(CombatAiDecision decision, Formation aiFormation)
        {
            lock (_key)
            {
                int targetPosition = decision.TargetCoordinate.PositionX +
                                     decision.TargetCoordinate.PositionY * GameplayConstants.MaxFormationRows;

                var result = _abilityManager.PerformAbility(decision.Actor, decision.Ability, targetPosition, decision.TargetFormation);

                if (result.FailureReason == null)
                {
                    _battle.ActionsLeftPerFormation[aiFormation].Remove(decision.Actor);
                }
                return result;
            }
        }

        /// <summary>
        /// Called to make a CombatEntity perform the defend command.
        /// </summary>
        /// <param name="actor">The CombatEntity performing the defend command.</param>
        /// <param name="actorFormation">The Formation of the CombatEntity performing the defend command.</param>
        /// <param name="nextActiveEntity">Contains the next active CombatEntity, if any.</param>
        /// <returns>An IEnumerable of CombatEntity containing the CombatEntity performing the defend command.</returns>
        private IEnumerable<CombatEntity> PerformDefend(CombatEntity actor, Formation actorFormation, out CombatEntity nextActiveEntity)
        {
            IEnumerable<CombatEntity> affectedEntities = null;

            var task = _statusEffectRepo.GetDataAsync();
            task.Wait();

            var statusEffects = task.Result;
            var defendStatus = statusEffects.FirstOrDefault(se => se.Id == GameplayConstants.DefendingStatusEffectId);

            lock (_key)
            {
                // Apply status effect
                _statusEffectManager.Apply(actor, actor, defendStatus);
                _battle.ActionsLeftPerFormation[actorFormation].Remove(actor);
                affectedEntities = new List<CombatEntity> { actor };

                // Choose next active entity
                nextActiveEntity = GetNextActiveEntity(actorFormation, _battle.ActionsLeftPerFormation[actorFormation]);
                if (nextActiveEntity != null) _battle.ActionsLeftPerFormation[actorFormation].Add(nextActiveEntity);
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
            lock (_key)
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
            lock (_key)
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
        }

        /// <summary>
        /// Brings a Formation to a fresh state ready for the start of battle.
        /// </summary>
        /// <param name="formation">The Formation to initialize.</param>
        /// <param name="isAttacker">Adds the formation to the attackers group if true, else adds to defenders.</param>
        /// <param name="dontIncrementActionPoints">If true, will keep the action points for this Formation at 0.</param>
        private void InitializeFormation(Formation formation, bool isAttacker, bool dontIncrementActionPoints = false)
        {
            lock (_key)
            {
                foreach (var row in formation.Positions)
                {
                    foreach (var entity in row)
                    {
                        if (entity == null) continue;
                        entity.Resources.CurrentActionPoints = 0;

                        // Temporary, remove later
                        entity.Resources.CurrentHealth = entity.Resources.MaxHealth;
                        entity.Resources.CurrentMana = entity.Resources.MaxMana;

                        if (!dontIncrementActionPoints) IncreaseActionPoints(entity);
                        
                        if (isAttacker) _numOfAttackers++;
                        else _numOfDefenders++;
                    }
                }
            }
        }

        /// <summary>
        /// Removes all combat effects from all CombatEntities in a formation to prepare to exit combat.
        /// </summary>
        /// <param name="formation">The Formation to sanitize.</param>
        private void SanitizeFormation(Formation formation)
        {
            lock (_key)
            {
                foreach (var row in formation.Positions)
                {
                    foreach (var entity in row)
                    {
                        if (entity == null) continue;
                        entity.Resources.CurrentActionPoints = 0;
                        _statusEffectManager.RemoveAll(entity);
                    }
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
            lock (_key)
            {
                var candidates = formation.Positions.WhereTwoD(entity =>
                {
                    if (entity == null) return false;
                    if (entity.Resources.CurrentActionPoints <= 0 || entity.Resources.CurrentHealth <= 0) return false;
                    if (entity.StatusEffects.Any(se => se.BaseStatus.IsStunned)) return false;
                    return true;
                });

                candidates = candidates.OrderByDescending(entity => entity.Resources.CurrentActionPoints);
                if (candidates.Count() <= GameplayConstants.MaxActionsPerTurn) return candidates.ToList();
                else return candidates.Take(GameplayConstants.MaxActionsPerTurn).ToList();
            }
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
            lock (_key)
            {
                var candidates = formation.Positions.WhereTwoD(ent =>
                {
                    if (ent == null) return false;
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
        }

        /// <summary>
        /// Increases action points for a CombatEntity based on its action point regeneration.
        /// </summary>
        /// <param name="entity">The CombatEntity whose action points should be increased.</param>
        private ActionPointData IncreaseActionPoints(CombatEntity entity)
        {
            lock (_key)
            {
                if (entity == null) return null;
                if (entity.Resources.CurrentHealth <= 0 || entity.StatusEffects.Any(se => se.BaseStatus.IsStunned)) return null;

                int randPoints = _seed.Next(0, entity.SecondaryStats.BonusActionPoints);
                int total = GameplayConstants.ActionPointsPerTurn + randPoints;
                total += total * entity.SecondaryStats.BonusActionPointsPercentage / 100;
                entity.Resources.CurrentActionPoints += total;

                return new ActionPointData
                {
                    EntityId = entity.Id,
                    CurrentActionPoints = entity.Resources.CurrentActionPoints
                };
            }
        }

        /// <summary>
        /// Increases action points for every CombatEntity that is able in a Formation.
        /// </summary>
        /// <param name="formation">The Formation to get the CombatEntities from.</param>
        private IEnumerable<ActionPointData> IncreaseActionPoints(Formation formation)
        {
            lock (_key)
            {
                var actionPointData = new List<ActionPointData>();

                foreach (var row in formation.Positions)
                {
                    foreach (var entity in row)
                    {
                        var data = IncreaseActionPoints(entity);
                        if (data != null) actionPointData.Add(data);
                    }
                }

                return actionPointData;
            }
        }

        /// <summary>
        /// Applies StatusEffects, if any, to all CombatEntities in a Formation. Removes them accordingly.
        /// </summary>
        /// <param name="formation">The Formation to apply StatusEffects to.</param>
        /// <returns>Returns an IEnumerable of CombatEntities affected by StatusEffect changes.</returns>
        private IEnumerable<CombatEntity> ApplyStatusEffects(Formation formation)
        {
            var affectedEntities = new List<CombatEntity>();
            var entities = formation.Positions.Flatten()
                                              .Where(e => e != null && e.Resources.CurrentHealth > 0)
                                              .Where(e => e.StatusEffects.Count > 0)
                                              .ToList();

            foreach (var entity in entities)
            {
                _statusEffectManager.ApplyEffects(entity);
                affectedEntities.Add(entity);
            }
            return affectedEntities;
        }

        /// <summary>
        /// Returns the current battle instance.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyBattle GetBattle()
        {
            lock (_key)
            {
                return _battle;
            }
        }

        /// <summary>
        /// Returns the ids of all players in the current battle.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetParticipantIds()
        {
            lock (_key)
            {
                return _participantIds.Where(id => id != GameplayConstants.AiId.ToString()).ToList();
            }
        }
    }
}
