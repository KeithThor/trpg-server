using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities.Data;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents an ingame entity that only appears when in combat.
    /// </summary>
    public class CombatEntity : IReadOnlyCombatEntity
    {
        /// <summary>
        /// The unique identifier for this entity.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The unique identifier for the combat group this entity belongs to. Can be null.
        /// </summary>
        public int? GroupId { get; set; }

        /// <summary>
        /// The guid that represents the player who owns this entity.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// The display name of this entity.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The display name of the owner of this entity.
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// The uris that point to the icons that make up the appearance of this entity. Does not include equipment uris.
        /// </summary>
        public CharacterIconSet IconUris { get; set; }

        /// <summary>
        /// Represents the character's in-game stats that determine combat efficiency.
        /// </summary>
        public CharacterStats Stats { get; set; }

        /// <summary>
        /// Represents a character's in-game secondary stats.
        /// </summary>
        public SecondaryStat SecondaryStats { get; set; }

        /// <summary>
        /// Contains all of the attacks, spells, and skills this character can use.
        /// </summary>
        public List<Ability> Abilities { get; set; }

        /// <summary>
        /// Contains all of the status effects that are affecting this character.
        /// </summary>
        public List<StatusEffect> StatusEffects { get; set; }

        IReadOnlyCharacterIconSet IReadOnlyCombatEntity.IconUris => IconUris;
        IReadOnlyCharacterStats IReadOnlyCombatEntity.Stats => Stats;
        IEnumerable<Ability> IReadOnlyCombatEntity.Abilities => Abilities;
        IEnumerable<StatusEffect> IReadOnlyCombatEntity.StatusEffects => StatusEffects;
        IReadOnlySecondaryStat IReadOnlyCombatEntity.SecondaryStats => SecondaryStats;
    }
}
