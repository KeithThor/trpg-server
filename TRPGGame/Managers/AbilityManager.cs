using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TRPGGame.Entities;
using TRPGGame.Entities.Combat;
using TRPGGame.Services;
using TRPGGame.Static;
using TRPGShared;

namespace TRPGGame.Managers
{
    /// <summary>
    /// Manager responsible for applying the effects of Abilities onto the correct targets in combat.
    /// </summary>
    public class AbilityManager : IAbilityManager
    {
        private readonly IStatusEffectManager _statusEffectManager;
        private readonly Random _seed;

        public AbilityManager(IStatusEffectManager statusEffectManager)
        {
            _statusEffectManager = statusEffectManager;
            _seed = new Random();
        }

        /// <summary>
        /// Performs an ability on a target location, applying damage, healing, and status effects to any targets.
        /// </summary>
        /// <param name="attacker">The attacker performing the ability.</param>
        /// <param name="ability">The ability used to attack the enemy.</param>
        /// <param name="action">Contains info about the targets for the attack.</param>
        /// <param name="targetFormation">The Formation that the CombatEntity is targeting with its action.</param>
        /// <returns></returns>
        public AbilityResult PerformAbility(CombatEntity attacker,
                                            Ability ability,
                                            BattleAction action,
                                            Formation targetFormation)
        {
            var result = new AbilityResult();

            // Target position is out of bounds
            if (!ability.IsPointBlank && !ability.IsPositionStatic &&
                (action.TargetPosition > 9 || action.TargetPosition < 1))
            {
                result.FailureReason = BattleErrorWriter.WriteTargetPositionOutOfBounds();
                return result;
            }

            if (!HasEnoughResources(attacker, ability, out string failureReason))
            {
                result.FailureReason = failureReason;
                return result;
            }

            int targetPosition = action.TargetPosition;
            if (ability.IsPointBlank)
            {
                targetPosition = GetTargetPosition(attacker, targetFormation);
            }
            var targets = FormationTargeter.GetTargets(ability, targetPosition, targetFormation).ToList();
            if (targets == null || targets.Count == 0)
            {
                result.FailureReason = BattleErrorWriter.WriteNoTargets();
                return result;
            }

            // Filter out all dead targets
            targets = targets.Where(entity => entity != null && entity.Resources.CurrentHealth > 0).ToList();
            // Will be an invalid Ability usage if all targets are dead (later on will add ability to revive)
            if (targets.Count == 0)
            {
                result.FailureReason = BattleErrorWriter.WriteAllTargetsAreDead();
                return result;
            }

            ConsumeResources(attacker, ability);
            ApplyEffects(attacker, ability, targets);

            targets.Add(attacker);

            result.AffectedEntities = targets;
            return result;
        }
        
        /// <summary>
        /// Activates the effects of a DelayedAbility on its target formation, returning an IEnumerable of
        /// CombatEntities affected by the DelayedAbility.
        /// </summary>
        /// <param name="ability">The DelayedAbility to activate the effects of.</param>
        /// <returns></returns>
        public IEnumerable<CombatEntity> PerformDelayedAbility(DelayedAbility ability)
        {
            var affectedEntities = new List<CombatEntity>();
            var targetPosition = ability.TargetPosition;
            if (ability.BaseAbility.IsPositionStatic) targetPosition = ability.BaseAbility.CenterOfTargets;

            var targets = FormationTargeter.GetTargets(ability.BaseAbility, targetPosition, ability.TargetFormation);
            if (targets == null || targets.Count() == 0) return affectedEntities;

            ApplyEffects(ability, targets);

            return targets;
        }

        /// <summary>
        /// Consumes the resources required to use the ability from the attacker.
        /// </summary>
        /// <param name="attacker">The CombatEntity using the ability.</param>
        /// <param name="ability">The Ability to get consumed resource amounts from.</param>
        private static void ConsumeResources(CombatEntity attacker, Ability ability)
        {
            int actionPointCost = ResourceCalculator.GetTotalActionPointCost(attacker, ability);
            int manaCost = ResourceCalculator.GetTotalManaCost(attacker, ability);
            int healthCost = ResourceCalculator.GetTotalHealthCost(attacker, ability);

            attacker.Resources.CurrentActionPoints -= actionPointCost;
            attacker.Resources.CurrentMana -= manaCost;
            attacker.Resources.CurrentHealth -= healthCost;
        }

        /// <summary>
        /// Creates an instance of a DelayedAbility.
        /// </summary>
        /// <param name="attacker">The CombatEntity starting the effect of a DelayedAbility.</param>
        /// <param name="ability">The Ability to convert into a DelayedAbility.</param>
        /// <param name="action">The object containing details about the action being performed.</param>
        /// <param name="targetFormation">The Formation that the CombatEntity is targeting with its action.</param>
        /// <returns></returns>
        public DelayedAbilityResult CreateDelayedAbility(CombatEntity attacker,
                                                         Ability ability,
                                                         BattleAction action,
                                                         Formation targetFormation)
        {
            var result = new DelayedAbilityResult();

            // Target out of bounds
            if (!ability.IsPointBlank && !ability.IsPositionStatic &&
                (action.TargetPosition > 9 || action.TargetPosition < 1))
            {
                result.FailureReason = BattleErrorWriter.WriteTargetPositionOutOfBounds();
                return result;
            }

            if (!HasEnoughResources(attacker, ability, out string failureReason))
            {
                result.FailureReason = failureReason;
                return result;
            }
            
            int targetPosition = action.TargetPosition;
            if (ability.IsPointBlank) targetPosition = GetTargetPosition(attacker, targetFormation);

            ConsumeResources(attacker, ability);
            bool isCrit = IsCritical(attacker, ability);

            result.DelayedAbility = new DelayedAbility
            {
                Actor = attacker,
                BaseAbility = ability,
                StoredDamage = DamageCalculator.GetDamage(attacker, ability, isCrit),
                StoredHealing = DamageCalculator.GetHeal(attacker, ability, isCrit),
                TargetFormation = targetFormation,
                TargetPosition = action.TargetPosition,
                IsCrit = isCrit,
                TurnsLeft = ability.DelayedTurns
            };

            return result;
        }

        /// <summary>
        /// Applies the effects of the ability being used on all of it's targets.
        /// </summary>
        /// <param name="attacker">The entity performing the ability.</param>
        /// <param name="ability">The ability to apply the effects of.</param>
        /// <param name="targets">The targets of the ability.</param>
        private void ApplyEffects(CombatEntity attacker, Ability ability, IEnumerable<CombatEntity> targets)
        {
            bool isCrit = IsCritical(attacker, ability);

            foreach (var target in targets)
            {
                var damage = DamageCalculator.GetTotalDamage(attacker, target, ability, isCrit);
                var healing = DamageCalculator.GetHeal(attacker, ability, isCrit);
                healing += target.Resources.MaxHealth * ability.PercentHeal / 100;

                target.Resources.CurrentHealth += healing;
                target.Resources.CurrentHealth -= DamageCalculator.GetDamageTypesAsInt(damage);
                if (target.Resources.CurrentHealth > 0)
                {
                    _statusEffectManager.Apply(target, attacker, ability.AppliedStatusEffects, isCrit);
                }
                else
                {
                    _statusEffectManager.RemoveAll(target);
                    target.Resources.CurrentHealth = 0;
                }
            }

            if (ability.SelfAppliedStatusEffects.Any())
            {
                _statusEffectManager.Apply(attacker, attacker, ability.SelfAppliedStatusEffects, isCrit);
            }
        }

        /// <summary>
        /// Applies the effects of a DelayedAbility to all of its targets.
        /// </summary>
        /// <param name="ability">The DelayedAbility to apply.</param>
        /// <param name="targets">The CombatEntity targets of the DelayedAbility.</param>
        private void ApplyEffects(DelayedAbility ability, IEnumerable<CombatEntity> targets)
        {
            foreach (var target in targets)
            {
                var damage = DamageCalculator.GetTotalDamage(ability.StoredDamage, target);
                int healing = ability.StoredHealing;
                healing += target.Resources.MaxHealth * ability.BaseAbility.PercentHeal / 100;

                target.Resources.CurrentHealth += healing;
                target.Resources.CurrentHealth -= DamageCalculator.GetDamageTypesAsInt(damage);
                if (target.Resources.CurrentHealth > 0)
                {
                    _statusEffectManager.Apply(target, ability.Actor, ability.BaseAbility.AppliedStatusEffects, ability.IsCrit);
                }
                else
                {
                    _statusEffectManager.RemoveAll(target);
                }
            }

            if (ability.BaseAbility.SelfAppliedStatusEffects.Any())
            {
                _statusEffectManager.Apply(ability.Actor, ability.Actor, ability.BaseAbility.SelfAppliedStatusEffects, ability.IsCrit);
            }
        }

        /// <summary>
        /// Returns true if the provided CombatEntity has enough resources to use an Ability.
        /// </summary>
        /// <param name="attacker">The attacking CombatEntity.</param>
        /// <param name="ability">The ability being used by the CombatEntity.</param>
        /// <returns></returns>
        private bool HasEnoughResources(CombatEntity attacker, Ability ability, out string failureReason)
        {
            failureReason = "";

            int actionPointCost = ResourceCalculator.GetTotalActionPointCost(attacker, ability);
            int manaCost = ResourceCalculator.GetTotalManaCost(attacker, ability);
            int healthCost = ResourceCalculator.GetTotalHealthCost(attacker, ability);

            if (attacker.Resources.CurrentActionPoints < actionPointCost)
            {
                failureReason = BattleErrorWriter.WriteInsufficientActionPoints(attacker, ability);
                return false;
            }

            if (attacker.Resources.CurrentMana < manaCost)
            {
                failureReason = BattleErrorWriter.WriteInsufficientMana(attacker, ability);
                return false;
            }

            if (attacker.Resources.CurrentHealth < healthCost)
            {
                failureReason = BattleErrorWriter.WriteInsufficientHealth(attacker, ability);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if the total critical chance for an attacker is greater than a random number
        /// generated between 1 and 100.
        /// </summary>
        /// <param name="attacker">The attacking CombatEntity.</param>
        /// <param name="ability">The ability used by the attacker.</param>
        /// <returns></returns>
        private bool IsCritical(CombatEntity attacker, Ability ability)
        {
            var totalCritChance = attacker.SecondaryStats.CritChance * attacker.SecondaryStats.CritChancePercentage / 100;
            totalCritChance += attacker.SecondaryStats.CritChance;
            return _seed.Next(0, 101) <= totalCritChance;
        }

        /// <summary>
        /// Returns the position that a CombatEntity exists in a Formation as an integer.
        /// </summary>
        /// <param name="entity">The CombatEntity to find.</param>
        /// <param name="formation">The Formation to look through.</param>
        /// <returns></returns>
        private int GetTargetPosition(CombatEntity entity, Formation formation)
        {
            var coords = formation.Positions.FindIndexTwoD(entity);
            if (coords == null) return -1;
            return coords[0] + 1 + coords[1] * 3;
        }
    }
}
