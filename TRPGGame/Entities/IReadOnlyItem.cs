using System.Collections.Generic;
using TRPGGame.Entities.Data;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a read-only version of an in-game item.
    /// </summary>
    public interface IReadOnlyItem
    {
        /// <summary>
        /// Contains status effects that are applied to any character struck by the character who has this item
        /// equipped.
        /// </summary>
        IEnumerable<IReadOnlyStatusEffect> AppliedStatusEffects { get; }

        /// <summary>
        /// The ability that can be activated by a character if this item is consumable.
        /// </summary>
        IReadOnlyAbility ConsumableAbility { get; }

        /// <summary>
        /// The amount of times this item can be activated by a character.
        /// </summary>
        int ConsumableCharges { get; }

        /// <summary>
        /// The description of this item.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// If true, this item is removed from the inventory of a character when it is out of charges.
        /// </summary>
        bool DestroyedWhenOutOfCharges { get; }

        /// <summary>
        /// The amount of primary stats this item grants the user while equipped.
        /// </summary>
        IReadOnlyCharacterStats Stats { get; }

        /// <summary>
        /// The amount of secondary stats this item grants the user while equipped.
        /// </summary>
        IReadOnlySecondaryStat SecondaryStats { get; }

        /// <summary>
        /// An IEnumerable of Abilities that this item grants a character when equipped.
        /// </summary>
        IEnumerable<IReadOnlyAbility> EquippedAbilities { get; }

        /// <summary>
        /// An IEnumerable of strings that contain the uris to the paths of the icons that represent this item.
        /// </summary>
        IEnumerable<string> IconUris { get; }

        /// <summary>
        /// The unique identifier for this item.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Returns true if this item has an ability that can be activated by the character without being equipped.
        /// </summary>
        bool IsConsumable { get; }

        /// <summary>
        /// Returns true if this item can be equipped by a character.
        /// </summary>
        bool IsEquippable { get; }

        /// <summary>
        /// The name of this item.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Contains status effects that are applied to the character whenever this item is equipped. The status
        /// effects are applied on the first turn of combat.
        /// </summary>
        IEnumerable<IReadOnlyStatusEffect> SelfAppliedStatusEffects { get; }

        /// <summary>
        /// The type this item is categorized as.
        /// </summary>
        IReadOnlyItemType Type { get; }
    }
}