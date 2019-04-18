using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities
{
    /// <summary>
    /// A basic data object that represents the amount of each resource stat such as health.
    /// </summary>
    public class ResourceStats : IReadOnlyResourceStats
    {
        /// <summary>
        /// The current amount of health an entity has.
        /// </summary>
        public int CurrentHealth { get; set; }

        /// <summary>
        /// The effective amount of max health an entity has, modified by other effects.
        /// </summary>
        public int MaxHealth { get; set; }

        /// <summary>
        /// The amount of max health an entity has before being modified by other effects.
        /// </summary>
        public int UnmodifiedMaxHealth { get; set; }

        /// <summary>
        /// The current amount of mana an entity has.
        /// </summary>
        public int CurrentMana { get; set; }

        /// <summary>
        /// The effective amount of max mana an entity has, modified by other effects.
        /// </summary>
        public int MaxMana { get; set; }

        /// <summary>
        /// The amount of max mana an entity has before being modified by other effects.
        /// </summary>
        public int UnmodifiedMaxMana { get; set; }

        /// <summary>
        /// The amount of action points an entity currently has.
        /// </summary>
        public int CurrentActionPoints { get; set; }
    }
}
