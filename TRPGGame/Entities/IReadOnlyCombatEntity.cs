using System;
using System.Collections.Generic;
using TRPGGame.Entities.Combat;
using TRPGGame.Entities.Data;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a read only version of a combat entity.
    /// </summary>
    public interface IReadOnlyCombatEntity
    {
        /// <summary>
        /// The unique identifier for the combat group this entity belongs to. Can be null.
        /// </summary>
        int? GroupId { get; }

        /// <summary>
        /// The uris that point to the icons that make up the appearance of this entity. Does not include equipment uris.
        /// </summary>
        IReadOnlyCharacterIconSet IconUris { get; }

        /// <summary>
        /// The unique identifier for this entity.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The display name of this entity.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The guid that represents the player who owns this entity.
        /// </summary>
        Guid OwnerId { get; }

        /// <summary>
        /// The display name of the owner of this entity.
        /// </summary>
        string OwnerName { get; }

        /// <summary>
        /// Represents the character's in-game stats that determine combat efficiency.
        /// </summary>
        IReadOnlyCharacterStats Stats { get; }

        /// <summary>
        /// Represents the character's in-game stats unmodified by any other effects.
        /// </summary>
        IReadOnlyCharacterStats UnmodifiedStats { get; }

        /// <summary>
        /// Represents the amount of each stat to gain whenever this CombatEntity levels up.
        /// </summary>
        IReadOnlyCharacterStats GrowthPoints { get; }

        /// <summary>
        /// Represents a character's in-game secondary stats.
        /// </summary>
        IReadOnlySecondaryStat SecondaryStats { get; }

        /// <summary>
        /// Contains all of the attacks, spells, and skills this character can use.
        /// </summary>
        IEnumerable<IReadOnlyAbility> Abilities { get; }

        /// <summary>
        /// Contains all of the status effects that are affecting this character.
        /// </summary>
        IEnumerable<IReadOnlyAppliedStatusEffect> StatusEffects { get; }

        /// <summary>
        /// Contains all of the items this character currently has equipped.
        /// </summary>
        IEnumerable<IReadOnlyItem> EquippedItems { get; }
    }
}