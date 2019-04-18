using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities.Data;

namespace TRPGGame.Entities.Combat
{
    /// <summary>
    /// Represents a CombatEntity state frozen in time.
    /// </summary>
    public class CombatEntitySnapshot
    {
        /// <summary>
        /// Creates a snapshot of a CombatEntity.
        /// </summary>
        /// <param name="entity">The entity to create a snapshot of.</param>
        /// <param name="removedStatusIds">Contains any removed StatusEffect ids.</param>
        /// <param name="appliedStatusEffects">Contains any status effects applied onto the entity.</param>
        /// <param name="removedAbilityIds">Contains any removed Ability ids.</param>
        /// <param name="removedItemIds">Contains any removed Item ids.</param>
        public CombatEntitySnapshot(CombatEntity entity,
                                    IEnumerable<int> removedStatusIds = null,
                                    IEnumerable<AppliedStatusEffect> appliedStatusEffects = null,
                                    IEnumerable<int> removedAbilityIds = null,
                                    IEnumerable<int> removedItemIds = null)
        {
            Id = entity.Id;
            Resources = entity.Resources.Copy();
            Stats = entity.Stats.Copy();
            SecondaryStats = entity.SecondaryStats.Copy();
            RemovedItemIds = removedItemIds;
            RemovedStatusEffectIds = removedStatusIds;
            AddedStatusEffects = appliedStatusEffects;
        }

        /// <summary>
        /// The id of the entity this is a snapshot of.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The amount of resources the entity had at the point this snapshot was taken.
        /// </summary>
        public ResourceStats Resources { get; set; }

        /// <summary>
        /// The amount of primary stats the entity had at the point this snapshot was taken.
        /// </summary>
        public CharacterStats Stats { get; set; }

        /// <summary>
        /// The amount of secondary stats the entity had at the point this snapshot was taken.
        /// </summary>
        public SecondaryStat SecondaryStats { get; set; }

        /// <summary>
        /// The ids of the status effects that were removed at the point this snapshot was taken.
        /// </summary>
        public IEnumerable<int> RemovedStatusEffectIds { get; set; }

        /// <summary>
        /// The ids of the abilities that were removed from the entity at the point this snapshot was taken.
        /// </summary>
        public IEnumerable<int> RemovedAbilityIds { get; set; }

        /// <summary>
        /// The ids of the items that were removed at the point this snapshot was taken.
        /// </summary>
        public IEnumerable<int> RemovedItemIds { get; set; }

        /// <summary>
        /// The StatusEffects that were applied at the point this snapshot was taken.
        /// </summary>
        public IEnumerable<AppliedStatusEffect> AddedStatusEffects { get; set; }
    }
}
