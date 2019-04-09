using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities.Data;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a template that shapes a CombatEntity's stats and starting items when choosing this class.
    /// </summary>
    public class ClassTemplate : IReadOnlyClassTemplate
    {
        /// <summary>
        /// The unique identifier for this ClassTemplate.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the class this template represents.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the class this template represents.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The starting stats for a CombatEntity who chooses this class.
        /// </summary>
        public CharacterStats Stats { get; set; } = new CharacterStats();

        /// <summary>
        /// The secondary stats for a CombatEntity who chooses this class.
        /// </summary>
        public SecondaryStat SecondaryStats { get; set; } = new SecondaryStat();

        /// <summary>
        /// The abilities a CombatEntity starts with when choosing this class.
        /// </summary>
        public IEnumerable<Ability> Abilities { get; set; } = new List<Ability>();

        /// <summary>
        /// The items that are equipped onto the CombatEntity when choosing this class.
        /// </summary>
        public IEnumerable<Item> EquippedItems { get; set; } = new List<Item>();

        IReadOnlyCharacterStats IReadOnlyClassTemplate.Stats => Stats;
        IReadOnlySecondaryStat IReadOnlyClassTemplate.SecondaryStats => SecondaryStats;
        IEnumerable<IReadOnlyAbility> IReadOnlyClassTemplate.Abilities => Abilities;
        IEnumerable<IReadOnlyItem> IReadOnlyClassTemplate.EquippedItems => EquippedItems;
    }
}
