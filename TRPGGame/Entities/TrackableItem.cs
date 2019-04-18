using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities
{
    /// <summary>
    /// A object that keeps track of how many charges and stacks are in an item.
    /// </summary>
    public class TrackableItem
    {
        public int BaseItemId { get; set; }
        public int CurrentCharges { get; set; }
        public int CurrentStacks { get; set; }
    }
}
