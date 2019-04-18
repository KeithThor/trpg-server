using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TRPGGame.Entities;
using TRPGGame.Entities.Combat;
using TRPGGame.Services;

namespace TRPGGame.Managers
{
    /// <summary>
    /// Manager responsible for applying and removing StatusEffects and their effects on CombatEntities.
    /// </summary>
    public class StatusEffectManager : IStatusEffectManager
    {
        /// <summary>
        /// Applies a StatusEffect onto a CombatEntity.
        /// </summary>
        /// <param name="recipient">The CombatEntity who is receiving the StatusEffect.</param>
        /// <param name="applicator">The CombatEntity applying the StatusEffect on the receiver.</param>
        /// <param name="statusEffect">The StatusEffect to apply onto the receiver.</param>
        /// <param name="isCrit">If true, will include critical damage in the calculations.</param>
        public void Apply(CombatEntity recipient, 
                          CombatEntity applicator, 
                          StatusEffect statusEffect,
                          bool isCrit)
        {
            var status = recipient.StatusEffects.FirstOrDefault(se => se.BaseStatus.Id == statusEffect.Id);
            if (status == null)
            {
                var appliedStatusEffect = CreateAppliedStatus(applicator, recipient, statusEffect);
                ApplyStatEffects(recipient, statusEffect);
                recipient.StatusEffects.Add(appliedStatusEffect);
            }
            else
            {
                // Apply another stack of StatusEffect
                if (statusEffect.StackSize > 1 && status.CurrentStacks < statusEffect.StackSize)
                {
                    status.CumulativeDamage += DamageCalculator.GetDamage(applicator, statusEffect, isCrit);
                    status.CumulativeHeal += DamageCalculator.GetHeal(applicator, statusEffect, isCrit);
                    status.CurrentStacks++;
                    status.Duration = statusEffect.Duration;
                }
                // Can't apply another stack, refresh duration instead
                else
                {
                    status.Duration = statusEffect.Duration;
                }
            }
        }

        /// <summary>
        /// Applies many StatusEffects onto a CombatEntity.
        /// </summary>
        /// <param name="recipient">The CombatEntity who is receiving the StatusEffects.</param>
        /// <param name="applicator">The CombatEntity applying the StatusEffects on the receiver.</param>
        /// <param name="statusEffects">The StatusEffects to apply onto the receiver.</param>
        public void Apply(CombatEntity recipient, 
                          CombatEntity applicator, 
                          IEnumerable<StatusEffect> statusEffects,
                          bool isCrit)
        {
            foreach (var statusEffect in statusEffects)
            {
                Apply(recipient, applicator, statusEffect, isCrit);
            }
        }

        /// <summary>
        /// Creates an instance of an AppliedStatusEffect from the given StatusEffect.
        /// </summary>
        /// <param name="applicator">The CombatEntity applying this instance of the AppliedStatusEffect.</param>
        /// <param name="statusEffect">The base to use for the AppliedStatusEffect.</param>
        /// <returns>Returns an instance of an AppliedStatusEffect that keeps track of a StatusEffect on a CombatEntity.</returns>
        private AppliedStatusEffect CreateAppliedStatus(CombatEntity applicator, CombatEntity recipient, StatusEffect statusEffect)
        {
            var appliedStatus = new AppliedStatusEffect();
            appliedStatus.CumulativeDamage = DamageCalculator.GetDamage(applicator, statusEffect);
            appliedStatus.CumulativeHeal = DamageCalculator.GetHeal(applicator, statusEffect);
            appliedStatus.CumulativeHeal += DamageCalculator.GetPercentageHeal(recipient.Resources.MaxHealth, statusEffect.PercentHealPerTurn);

            appliedStatus.CurrentStacks = 1;
            appliedStatus.Duration = statusEffect.Duration;

            return appliedStatus;
        }

        /// <summary>
        /// Applies stat modifications from a StatusEffect onto a recipient CombatEntity.
        /// </summary>
        /// <param name="recipient">The CombatEntity who will get its stats modified.</param>
        /// <param name="statusEffect">The StatusEffect to apply the stat modifications from.</param>
        private void ApplyStatEffects(CombatEntity recipient, StatusEffect statusEffect)
        {
            recipient.SecondaryStats += statusEffect.ModifiedSecondaryStats;
            recipient.Stats += statusEffect.ModifiedStats;

            var statsArray = recipient.UnmodifiedStats.AsArray();
            var percentageArray = statusEffect.ModifiedStatPercentages.AsArray();

            for (int i = 0; i < statsArray.Length; i++)
            {
                statsArray[i] += (statsArray[i] * (percentageArray[i] + 100)) / 100;
            }

            recipient.Stats += new Entities.Data.CharacterStats(statsArray);
        }

        /// <summary>
        /// Removes a StatusEffect from a CombatEntity, removing all the beneficial and detrimental effects.
        /// </summary>
        /// <param name="entity">The CombatEntity to remove the StatusEffect from.</param>
        /// <param name="statusEffect">The StatusEffect type to remove from the CombatEntity.</param>
        /// <returns>Returns true if removal was successful. Returns false if no StatusEffect was found or
        /// removal failed.</returns>
        public bool Remove(CombatEntity entity, StatusEffect statusEffect)
        {
            var status = entity.StatusEffects.FirstOrDefault(se => se.BaseStatus == statusEffect);
            if (status == null) return false;

            entity.SecondaryStats -= status.BaseStatus.ModifiedSecondaryStats;
            entity.Stats -= status.BaseStatus.ModifiedStats;

            var statsArray = entity.UnmodifiedStats.AsArray();
            var percentageArray = status.BaseStatus.ModifiedStatPercentages.AsArray();

            for (int i = 0; i < statsArray.Length; i++)
            {
                statsArray[i] -= (statsArray[i] * (percentageArray[i] + 100)) / 100;
            }

            entity.Stats -= new Entities.Data.CharacterStats(statsArray);
            entity.StatusEffects.Remove(status);
            return true;
        }

        /// <summary>
        /// Removes an AppliedStatusEffect from a CombatEntity, removing all the beneficial and detrimental effects.
        /// </summary>
        /// <param name="entity">The CombatEntity to remove the StatusEffect from.</param>
        /// <param name="appliedStatusEffect">The AppliedStatusEffect type to remove from the CombatEntity.</param>
        /// <returns>Returns true if removal was successful. Returns false if no StatusEffect was found or
        /// removal failed.</returns>
        public bool Remove(CombatEntity entity, AppliedStatusEffect appliedStatusEffect)
        {
            return Remove(entity, appliedStatusEffect.BaseStatus);
        }

        /// <summary>
        /// Removes a StatusEffect from a CombatEntity given the id of the StatusEffect, removing all the 
        /// beneficial and detrimental effects.
        /// </summary>
        /// <param name="entity">The CombatEntity to remove the StatusEffect from.</param>
        /// <param name="statusId">The id of the StatusEffect type to remove from the CombatEntity.</param>
        /// <returns>Returns true if removal was successful. Returns false if no StatusEffect was found or
        /// removal failed.</returns>
        public bool Remove(CombatEntity entity, int statusId)
        {
            var status = entity.StatusEffects.FirstOrDefault(se => se.BaseStatus.Id == statusId);
            if (status == null) return false;
            else return Remove(entity, status.BaseStatus);
        }

        /// <summary>
        /// Removes all non-permanent StatusEffects from a CombatEntity.
        /// </summary>
        /// <param name="entity">The CombatEntity to remove StatusEffects from.</param>
        public void RemoveAll(CombatEntity entity)
        {
            RemoveAll(entity, (status) => true);
        }

        /// <summary>
        /// Removes all non-permanent StatusEffects from a CombatEntity that satisfies a given predicate.
        /// </summary>
        /// <param name="entity">The CombatEntity to remove StatusEffects from.</param>
        /// <param name="predicate">A function used to filter out which StatusEffects to remove from the CombatEntity.</param>
        public void RemoveAll(CombatEntity entity, Func<AppliedStatusEffect, bool> predicate)
        {
            var remove = entity.StatusEffects.Where(se => !se.BaseStatus.IsPermanent && predicate(se));
            foreach (var statusEffect in remove)
            {
                Remove(entity, statusEffect);
            }
        }
    }
}
