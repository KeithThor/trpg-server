using TRPGGame.Entities;

namespace TRPGGame.Managers
{
    /// <summary>
    /// An interface that performs operations to equip and unequip items from a CombatEntity.
    /// </summary>
    public interface IEquipmentManager
    {
        /// <summary>
        /// Equips an item onto a CombatEntity and applys its effects. Will unequip any items that also belong to the
        /// same EquipPosition as the item being applied.
        /// </summary>
        /// <param name="entity">The CombatEntity to equip the item on to.</param>
        /// <param name="item">The item to equip.</param>
        /// <returns>Returns true if the operation was successful. Will fail if the item is unequippable.</returns>
        bool Equip(CombatEntity entity, Item item);

        /// <summary>
        /// Unequips an item from a CombatEntity, removing all bonuses and abilities granted by the item.
        /// </summary>
        /// <param name="entity">The CombatEntity to unequip the item from.</param>
        /// <param name="item">The Item to unequip.</param>
        void Unequip(CombatEntity entity, Item item);

        /// <summary>
        /// Reduces the amount of charges in an item. If the item has no charges left and DestroyedWhenOutOfCharges is true,
        /// will remove the item from existence.
        /// </summary>
        /// <param name="entity">The entity who used the item.</param>
        /// <param name="item">The item to reduce charges of.</param>
        void ReduceCharges(CombatEntity entity, Item item);
    }
}