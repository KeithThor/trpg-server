using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a type of item such as Helmet, Chestpiece etc.
    /// </summary>
    public class ItemType : IReadOnlyItemType
    {
        /// <summary>
        /// The unique identifier of this ItemType.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of this ItemType.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of this ItemType. Might not be used.
        /// </summary>
        public string Description { get; set; }
    }
}
