using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities.Combat;
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
        /// The current combo counter on this character.
        /// </summary>
        public int ComboCounter { get; set; }

        /// <summary>
        /// Represents the amount of health, mana, etc that this CombatEntity has.
        /// </summary>
        public ResourceStats Resources { get; set; }

        /// <summary>
        /// The uris that point to the icons that make up the appearance of this entity. Does not include equipment uris.
        /// </summary>
        public CharacterIconSet IconUris { get; set; } = new CharacterIconSet();

        /// <summary>
        /// Represents the character's in-game stats that determine combat efficiency.
        /// </summary>
        public CharacterStats Stats { get; set; } = new CharacterStats();

        /// <summary>
        /// Represents the amount of threat this CombatEntity poses to ai enemies.
        /// </summary>
        public int Threat { get; set; } = 0;

        /// <summary>
        /// Represents the character's in-game stats unmodified by any other effects.
        /// </summary>
        public CharacterStats UnmodifiedStats { get; set; } = new CharacterStats();

        /// <summary>
        /// Represents the amount of each stat to gain whenever this CombatEntity levels up.
        /// </summary>
        public CharacterStats GrowthPoints { get; set; } = new CharacterStats();

        /// <summary>
        /// Represents a character's in-game secondary stats.
        /// </summary>
        public SecondaryStat SecondaryStats { get; set; } = new SecondaryStat();

        /// <summary>
        /// Contains all of the attacks, spells, and skills this character can use.
        /// </summary>
        public List<Ability> Abilities { get; set; } = new List<Ability>();

        /// <summary>
        /// Contains all of the attacks, spells, and skills this character knows that are on cooldown.
        /// <para>The keys are the abilities on cooldown, the values are the turns left until the cooldown is over.</para>
        /// </summary>
        public List<KeyValuePair<Ability, int>> AbilitiesOnCooldown { get; set; } = new List<KeyValuePair<Ability, int>>();

        /// <summary>
        /// Contains all of the status effects that are affecting this character.
        /// </summary>
        public List<AppliedStatusEffect> StatusEffects { get; set; } = new List<AppliedStatusEffect>();

        /// <summary>
        /// Contains a reference to the inventory of the player who owns this CombatEntity.
        /// </summary>
        public Inventory PlayerInventory { get; set; } = new Inventory();

        /// <summary>
        /// Contains all of the items that are equipped on this character.
        /// </summary>
        public List<Item> EquippedItems { get; set; } = new List<Item>();

        /// <summary>
        /// Contains all of the items that are being tracked on this character.
        /// </summary>
        public List<TrackableItem> TrackedItems { get; set; } = new List<TrackableItem>();

        IReadOnlyCharacterIconSet IReadOnlyCombatEntity.IconUris => IconUris;
        IReadOnlyCharacterStats IReadOnlyCombatEntity.Stats => Stats;
        IEnumerable<IReadOnlyAbility> IReadOnlyCombatEntity.Abilities => Abilities;
        IEnumerable<IReadOnlyAppliedStatusEffect> IReadOnlyCombatEntity.StatusEffects => StatusEffects;
        IReadOnlySecondaryStat IReadOnlyCombatEntity.SecondaryStats => SecondaryStats;
        IEnumerable<IReadOnlyItem> IReadOnlyCombatEntity.EquippedItems => EquippedItems;
        IReadOnlyCharacterStats IReadOnlyCombatEntity.UnmodifiedStats => UnmodifiedStats;
        IReadOnlyCharacterStats IReadOnlyCombatEntity.GrowthPoints => GrowthPoints;

        IReadOnlyList<KeyValuePair<IReadOnlyAbility, int>> IReadOnlyCombatEntity.AbilitiesOnCooldown
        {
            get => (IReadOnlyList<KeyValuePair<IReadOnlyAbility, int>>)AbilitiesOnCooldown;
        }
    }
}
