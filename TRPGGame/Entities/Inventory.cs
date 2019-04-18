using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a player's item stash.
    /// </summary>
    public class Inventory
    {
        /// <summary>
        /// The maximum number of items that a player's inventory can hold.
        /// </summary>
        public int MaxItems { get; set; }

        /// <summary>
        /// Contains all of the items in a player's inventory, not including items equipped by CombatEntities.
        /// </summary>
        public List<Item> Items { get; set; } = new List<Item>();

        /// <summary>
        /// Contains all of the items being tracked for charges and stack sizes.
        /// </summary>
        public List<TrackableItem> TrackedItems { get; set; } = new List<TrackableItem>();
    }
}
