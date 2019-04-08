using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities.Data;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Object that represents an in-game item.
    /// </summary>
    public class Item : IReadOnlyItem
    {
        /// <summary>
        /// The unique identifier for this item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of this item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of this item.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// An IEnumerable of strings that contain the uris to the paths of the icons that represent this item.
        /// </summary>
        public IEnumerable<string> IconUris { get; set; }

        /// <summary>
        /// The type this item is categorized as.
        /// </summary>
        public ItemType Type { get; set; }

        /// <summary>
        /// The amount of primary stats this item grants the user while equipped.
        /// </summary>
        public CharacterStats Stats { get; set; }

        /// <summary>
        /// The amount of secondary stats this item grants the user while equipped.
        /// </summary>
        public SecondaryStat SecondaryStats { get; set; }

        /// <summary>
        /// Returns true if this item can be equipped by a character.
        /// </summary>
        public bool IsEquippable { get; set; }

        /// <summary>
        /// An IEnumerable of Abilities that this item grants a character when equipped.
        /// </summary>
        public IEnumerable<Ability> EquippedAbilities { get; set; }

        /// <summary>
        /// Returns true if this item has an ability that can be activated by the character without being equipped.
        /// </summary>
        public bool IsConsumable { get; set; }

        /// <summary>
        /// The ability that can be activated by a character if this item is consumable.
        /// </summary>
        public Ability ConsumableAbility { get; set; }

        /// <summary>
        /// The amount of times this item can be activated by a character.
        /// </summary>
        public int ConsumableCharges { get; set; }

        /// <summary>
        /// If true, this item is removed from the inventory of a character when it is out of charges.
        /// </summary>
        public bool DestroyedWhenOutOfCharges { get; set; }

        /// <summary>
        /// Contains status effects that are applied to the character whenever this item is equipped. The status
        /// effects are applied on the first turn of combat.
        /// </summary>
        public IEnumerable<StatusEffect> SelfAppliedStatusEffects { get; set; }

        /// <summary>
        /// Contains status effects that are applied to any character struck by the character who has this item
        /// equipped.
        /// </summary>
        public IEnumerable<StatusEffect> AppliedStatusEffects { get; set; }

        IEnumerable<IReadOnlyStatusEffect> IReadOnlyItem.AppliedStatusEffects => AppliedStatusEffects;

        IReadOnlyAbility IReadOnlyItem.ConsumableAbility => ConsumableAbility;

        IEnumerable<IReadOnlyAbility> IReadOnlyItem.EquippedAbilities => EquippedAbilities;

        IEnumerable<IReadOnlyStatusEffect> IReadOnlyItem.SelfAppliedStatusEffects => SelfAppliedStatusEffects;

        IReadOnlyItemType IReadOnlyItem.Type => Type;

        IReadOnlyCharacterStats IReadOnlyItem.Stats => Stats;

        IReadOnlySecondaryStat IReadOnlyItem.SecondaryStats => SecondaryStats;
    }
}
