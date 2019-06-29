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
        private readonly System.Timers.Timer _timer;
        private readonly IAbilityManager _abilityManager;
        private readonly IEquipmentManager _equipmentManager;
        private readonly IStatusEffectManager _statusEffectManager;
        private readonly IRepository<StatusEffect> _statusEffectRepo;
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
            _secondsElapsedInTurn++;
            if (_secondsElapsedInTurn >= GameplayConstants.SecondsPerTurn)
            {
                 _secondsElapsedInTurn = 0;
                EndTurn();
            }

            // Make Ai act here
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
            if (_battle != null) return null;
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
            var activeEntities = new List<ActiveEntities>();
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
                InitializeFormation(defender, false);
            }

            var nextTurnStartDate = DateTime.Now.AddSeconds(GameplayConstants.SecondsPerTurn);
            double millisecondsToWait = (nextTurnStartDate - DateTime.Now).TotalMilliseconds;

            var battle = new Battle
            {
                Attackers = attackingFormations,
                Defenders = defendingFormations,
                TurnExpiration = nextTurnStartDate,
                Round = 1,
                ActionsLeftPerFormation = actionsPerFormation,
                IsDefenderTurn = false
            };

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
        /// <param name="participant">The WorldEntity to add to the battle.</param>
        /// <param name="isAttacker">If true, will join the side of the attackers. If false, will join the defenders.</param>
        /// <returns></returns>
        public IReadOnlyBattle JoinBattle(WorldEntity participant, bool isAttacker)
        {
            if (_battle == null) return null;

            InitializeFormation(participant.ActiveFormation, isAttacker);

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
            var activeEntities = new List<ActiveEntities>();

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
            if (_battle.Attackers.Any(formation => formation == host.ActiveFormation)) return JoinBattle(joiner, true);
            else if (_battle.Defenders.Any(formation => formation == host.ActiveFormation)) return JoinBattle(joiner, false);

            // No formation was found for the given host's formation
            else return null;
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

            var attackersWin = _numOfDefenders <= 0;
            Task.Run(() => EndOfBattleEvent(this, new EndOfBattleEventArgs
            {
                ParticipantIds = _participantIds,
                DidAttackersWin = attackersWin
            }));
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
            var activeEntities = new List<ActiveEntities>();

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
                var actives = ChooseActiveEntities(formation);
                activeEntities.Add(new ActiveEntities
                {
                    EntityIds = actives.Select(ae => ae.Id).ToList(),
                    FormationId = formation.Id,
                    OwnerId = formation.OwnerId
                });
                _battle.ActionsLeftPerFormation.Add(formation, actives);
            }

            _battle.TurnExpiration = DateTime.Now.AddSeconds(GameplayConstants.SecondsPerTurn);

            Task.Run(() => StartOfTurnEvent?.Invoke(this, new StartOfTurnEventArgs
            {
                DelayedAbilities = activatedAbilities,
                AffectedEntities = affectedEntities,
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

            if (_numOfAttackers <= 0 || _numOfDefenders <= 0)
            {
                EndBattle();
            }
            else
            {
                _battle.ActionsLeftPerFormation.Clear();

                // Call start turn after a delay
                var dueDate = DateTime.Now.AddSeconds(GameplayConstants.EndOfTurnDelayInSeconds);
                Timer timer = new Timer(
                                    (arg) => { StartTurn(); },
                                    null,
                                    (int)(dueDate - DateTime.Now).TotalMilliseconds,
                                    0);
            }
        }

        /// <summary>
        /// Performs an action and returns true if the action was a success.
        /// </summary>
        /// <param name="action">The action to perform, containing data about the actor and the abilities used.</param>
        /// <returns></returns>
        public async Task<bool> PerformActionAsync(BattleAction action)
        {
            IEnumerable<CombatEntity> affectedEntities;
            if (_battle == null) return false;

            var actorFormation = GetFormation(action.OwnerId, out bool isAttacker);
            if (actorFormation == null) return false;

            var actor = actorFormation.Positions.FirstOrDefaultTwoD(entity => entity.Id == action.ActorId);
            if (actor == null) return false;

            // If actor should not be acting, return null
            if (!_battle.ActionsLeftPerFormation.ContainsKey(actorFormation)) return false;
            if (!_battle.ActionsLeftPerFormation[actorFormation].Contains(actor)) return false;
            if (actor.StatusEffects.Any(se => se.BaseStatus.IsStunned)) return false;

            Ability ability = null;
            Item item = null;

            if (!action.IsDefending &&
                !action.IsFleeing)
            {
                if (action.IsUsingItem)
                {
                    item = actor.EquippedItems.FirstOrDefault(i => i.ConsumableAbility.Id == action.AbilityId);
                    if (item == null) item = actor.PlayerInventory
                                                  .Items
                                                  .FirstOrDefault(i => i != null && i.ConsumableAbility.Id == action.AbilityId);

                    if (item == null) return false;
                    
                    ability = item.ConsumableAbility;
                }
                else
                {
                    ability = actor.Abilities.FirstOrDefault(abi => abi.Id == action.AbilityId);
                    if (ability == null) return false;
                    if (ability.IsSpell && actor.StatusEffects.Any(se => se.BaseStatus.IsSilenced)) return false;
                    if (!ability.IsSpell && actor.StatusEffects.Any(se => se.BaseStatus.IsRestricted)) return false;
                }

                var targetFormation = GetFormation(action.TargetFormationId, out bool throwAway);
                if (ability.DelayedTurns > 0)
                {
                    var delayedAbility = _abilityManager.CreateDelayedAbility(actor, ability, action, targetFormation);
                    if (isAttacker) _battle.AttackerDelayedAbilities.Add(delayedAbility);
                    else _battle.DefenderDelayedAbilities.Add(delayedAbility);
                    affectedEntities = new List<CombatEntity> { actor };
                }
                else
                {
                    affectedEntities = _abilityManager.Attack(actor, ability, action, targetFormation);
                    if (affectedEntities == null) return false;
                }

                _battle.ActionsLeftPerFormation[actorFormation].Remove(actor);
                if (item != null) _equipmentManager.ReduceCharges(actor, item);

                foreach (var entity in affectedEntities)
                {
                    if (entity.Resources.CurrentHealth <= 0)
                    {
                        if (actorFormation.Positions.ContainsTwoD(entity)) _numOfAttackers--;
                        else _numOfDefenders--;
                    }
                }

                await Task.Run(() => SuccessfulActionEvent.Invoke(this, new SuccessfulActionEventArgs
                {
                    Ability = ability,
                    Action = action,
                    Actor = actor,
                    AffectedEntities = affectedEntities,
                    ParticipantIds = _participantIds
                }));
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

                await Task.Run(() => SuccessfulActionEvent.Invoke(this, new SuccessfulActionEventArgs
                {
                    Ability = null,
                    Action = action,
                    Actor = actor,
                    AffectedEntities = affectedEntities,
                    ParticipantIds = _participantIds
                }));
            }

            return true;
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
        /// <param name="isAttacker">Adds the formation to the attackers group if true, else adds to defenders.</param>
        private void InitializeFormation(Formation formation, bool isAttacker)
        {
            var characters = new List<CombatEntity>();
            foreach (var row in formation.Positions)
            {
                foreach (var entity in row)
                {
                    if (entity == null) continue;
                    entity.Resources.CurrentActionPoints = 0;

                    // Temporary, remove later
                    entity.Resources.CurrentHealth = entity.Resources.MaxHealth;
                    entity.Resources.CurrentMana = entity.Resources.MaxMana;

                    IncreaseActionPoints(entity);
                    characters.Add(entity);

                    if (isAttacker) _numOfAttackers++;
                    else _numOfDefenders++;
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
                    if (entity == null) continue;
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
                if (entity == null) return false;
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
            return _participantIds.Where(id => id != GameplayConstants.AiId.ToString()).ToList();
        }
    }
}
