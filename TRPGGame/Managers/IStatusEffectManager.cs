using System;
using System.Collections.Generic;
using TRPGGame.Entities;
using TRPGGame.Entities.Combat;

namespace TRPGGame.Managers
{
    /// <summary>
    /// Interface that applies and removes StatusEffects from CombatEntities.
    /// </summary>
    public interface IStatusEffectManager
    {
        /// <summary>
        /// Applies a StatusEffect onto a CombatEntity.
        /// </summary>
        /// <param name="recipient">The CombatEntity who is receiving the StatusEffect.</param>
        /// <param name="applicator">The CombatEntity applying the StatusEffect on the receiver.</param>
        /// <param name="statusEffect">The StatusEffect to apply onto the receiver.</param>
        /// <param name="isCrit">If true, will include critical damage in the calculations.</param>
        void Apply(CombatEntity recipient, CombatEntity applicator, StatusEffect statusEffect, bool isCrit = false);

        /// <summary>
        /// Applies many StatusEffects onto a CombatEntity.
        /// </summary>
        /// <param name="recipient">The CombatEntity who is receiving the StatusEffects.</param>
        /// <param name="applicator">The CombatEntity applying the StatusEffects on the receiver.</param>
        /// <param name="statusEffects">The StatusEffects to apply onto the receiver.</param>
        /// <param name="isCrit">If true, will include critical damage in the calculations.</param>
        void Apply(CombatEntity recipient, CombatEntity applicator, IEnumerable<StatusEffect> statusEffects, bool isCrit = false);

        /// <summary>
        /// Applies damage and healing from all StatusEffects afflicting a CombatEntity and reduces their timers by 1.
        /// Removes any StatusEffects that are expired.
        /// </summary>
        /// <param name="entity">The CombatEntity to apply damage and healing to.</param>
        void ApplyEffects(CombatEntity entity);

        /// <summary>
        /// Removes an AppliedStatusEffect from a CombatEntity, removing all the beneficial and detrimental effects.
        /// </summary>
        /// <param name="entity">The CombatEntity to remove the StatusEffect from.</param>
        /// <param name="appliedStatusEffect">The AppliedStatusEffect type to remove from the CombatEntity.</param>
        /// <returns>Returns true if removal was successful. Returns false if no StatusEffect was found or
        /// removal failed.</returns>
        bool Remove(CombatEntity entity, AppliedStatusEffect appliedStatusEffect);

        /// <summary>
        /// Removes a StatusEffect from a CombatEntity given the id of the StatusEffect, removing all the 
        /// beneficial and detrimental effects.
        /// </summary>
        /// <param name="entity">The CombatEntity to remove the StatusEffect from.</param>
        /// <param name="statusId">The id of the StatusEffect type to remove from the CombatEntity.</param>
        /// <returns>Returns true if removal was successful. Returns false if no StatusEffect was found or
        /// removal failed.</returns>
        bool Remove(CombatEntity entity, int statusId);

        /// <summary>
        /// Removes a StatusEffect from a CombatEntity, removing all the beneficial and detrimental effects.
        /// </summary>
        /// <param name="entity">The CombatEntity to remove the StatusEffect from.</param>
        /// <param name="statusEffect">The StatusEffect type to remove from the CombatEntity.</param>
        /// <returns>Returns true if removal was successful. Returns false if no StatusEffect was found or
        /// removal failed.</returns>
        bool Remove(CombatEntity entity, StatusEffect statusEffect);

        /// <summary>
        /// Removes all non-permanent StatusEffects from a CombatEntity.
        /// </summary>
        /// <param name="entity">The CombatEntity to remove StatusEffects from.</param>
        void RemoveAll(CombatEntity entity);

        /// <summary>
        /// Removes all non-permanent StatusEffects from a CombatEntity that satisfies a given predicate.
        /// </summary>
        /// <param name="entity">The CombatEntity to remove StatusEffects from.</param>
        /// <param name="predicate">A function used to filter out which StatusEffects to remove from the CombatEntity.</param>
        void RemoveAll(CombatEntity entity, Func<AppliedStatusEffect, bool> predicate);
    }
}