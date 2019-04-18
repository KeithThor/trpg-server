using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TRPGGame.Entities;

namespace TRPGGame.Managers
{
    /// <summary>
    /// Manager responsible for equipping and unequipping items from CombatEntities.
    /// </summary>
    public class EquipmentManager : IEquipmentManager
    {
        private readonly IStatusEffectManager _statusEffectManager;

        public EquipmentManager(IStatusEffectManager statusEffectManager)
        {
            _statusEffectManager = statusEffectManager;
        }

        /// <summary>
        /// Equips an item onto a CombatEntity and applys its effects. Will unequip any items that also belong to the
        /// same EquipPosition as the item being applied.
        /// </summary>
        /// <param name="entity">The CombatEntity to equip the item on to.</param>
        /// <param name="item">The item to equip.</param>
        /// <returns>Returns true if the operation was successful. Will fail if the item is unequippable.</returns>
        public bool Equip(CombatEntity entity, Item item)
        {
            if (!item.IsEquippable) return false;
            if (string.IsNullOrEmpty(item.Type.EquipPosition)) return false;

            var oldItem = entity.EquippedItems.FirstOrDefault(i => i.Type.EquipPosition == item.Type.EquipPosition);
            Unequip(entity, oldItem);

            entity.PlayerInventory.Items.Remove(item);
            var tracking = entity.PlayerInventory.TrackedItems.FirstOrDefault(i => i.BaseItemId == item.Id);
            if (tracking != null)
            {
                entity.PlayerInventory.TrackedItems.Remove(tracking);
                entity.TrackedItems.Add(tracking);
            }

            entity.EquippedItems.Add(item);
            entity.Abilities.AddRange(item.EquippedAbilities);
            entity.SecondaryStats += item.SecondaryStats;
            entity.Stats += item.Stats;
            _statusEffectManager.Apply(entity, entity, item.SelfAppliedStatusEffects);
            ChangeIconUri(entity, item.Type.EquipPosition, item.EquipIconUri);
            return true;
        }

        /// <summary>
        /// Reduces the amount of charges in an item. If the item has no charges left and DestroyedWhenOutOfCharges is true,
        /// will remove the item from existence.
        /// </summary>
        /// <param name="entity">The entity who used the item.</param>
        /// <param name="item">The item to reduce charges of.</param>
        public void ReduceCharges(CombatEntity entity, Item item)
        {
            bool isInPlayerInventory = true;
            if (item.ConsumableCharges == null || item.IsConsumable) return;
            var track = entity.PlayerInventory.TrackedItems
                                              .FirstOrDefault(i => i.BaseItemId == item.Id && i.CurrentCharges > 0);
            if (track == null)
            {
                entity.TrackedItems.FirstOrDefault(i => i.BaseItemId == item.Id);
                isInPlayerInventory = false;
            }
            if (track == null) throw new Exception("The item " + item.Name + " is not being tracked.");

            track.CurrentCharges--;
            if (item.DestroyedWhenOutOfCharges && track.CurrentCharges == 0)
            {
                if (!isInPlayerInventory)
                {
                    Unequip(entity, item);
                }
                entity.PlayerInventory.TrackedItems.Remove(track);
                entity.PlayerInventory.Items.Remove(item);
            }
        }

        /// <summary>
        /// Unequips an item from a CombatEntity, removing all bonuses and abilities granted by the item.
        /// </summary>
        /// <param name="entity">The CombatEntity to unequip the item from.</param>
        /// <param name="item">The Item to unequip.</param>
        public void Unequip(CombatEntity entity, Item item)
        {
            if (item == null) return;
            var index = entity.EquippedItems.FindIndex(i => i.Type.EquipPosition == item.Type.EquipPosition);
            if (index != -1)
            {
                var oldItem = entity.EquippedItems[index];

                entity.PlayerInventory.Items.Add(oldItem);
                var tracking = entity.TrackedItems.FirstOrDefault(i => i.BaseItemId == item.Id);
                if (tracking != null)
                {
                    entity.TrackedItems.Remove(tracking);
                    entity.PlayerInventory.TrackedItems.Add(tracking);
                }

                entity.Abilities.RemoveAll(ability => oldItem.EquippedAbilities.Contains(ability));
                entity.SecondaryStats -= oldItem.SecondaryStats;
                entity.Stats -= oldItem.Stats;
                foreach (var status in oldItem.SelfAppliedStatusEffects)
                {
                    _statusEffectManager.Remove(entity, status);
                }
                ChangeIconUri(entity, oldItem.Type.EquipPosition, null);
                entity.EquippedItems.RemoveAt(index);
            }
        }

        /// <summary>
        /// Changes the IconUri of an EquipPosition on the CombatEntity.
        /// </summary>
        /// <param name="entity">The CombatEntity to change the IconUri for.</param>
        /// <param name="equipPosition">A string containing the position to equip the Item to.</param>
        /// <param name="equipIconUri">The uri pointing to the Icon that represents the item being equipped or removed.</param>
        private void ChangeIconUri(CombatEntity entity, string equipPosition, string equipIconUri)
        {
            switch (equipPosition)
            {
                case EquipmentConstants.Body:
                    entity.IconUris.BodyIconUri = equipIconUri;
                    break;
                case EquipmentConstants.Boots:
                    entity.IconUris.BootsIconUri = equipIconUri;
                    break;
                case EquipmentConstants.Cloak:
                    entity.IconUris.CloakIconUri = equipIconUri;
                    break;
                case EquipmentConstants.Extra:
                    entity.IconUris.ExtraIconUri = equipIconUri;
                    break;
                case EquipmentConstants.Gloves:
                    entity.IconUris.GlovesIconUri = equipIconUri;
                    break;
                case EquipmentConstants.Head:
                    entity.IconUris.HeadIconUri = equipIconUri;
                    break;
                case EquipmentConstants.LeftHand:
                    entity.IconUris.LeftHandIconUri = equipIconUri;
                    break;
                case EquipmentConstants.Legs:
                    entity.IconUris.LegsIconUri = equipIconUri;
                    break;
                case EquipmentConstants.RightHand:
                    entity.IconUris.RightHandIconUri = equipIconUri;
                    break;
                default:
                    break;
            }
        }
    }
}
