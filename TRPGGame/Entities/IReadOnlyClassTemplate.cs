using System.Collections.Generic;
using TRPGGame.Entities.Data;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a reference to a read-only instance of a ClassTemplate.
    /// </summary>
    public interface IReadOnlyClassTemplate
    {
        /// <summary>
        /// The unique identifier for this ClassTemplate.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the class this template represents.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description of the class this template represents.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The starting stats for a CombatEntity who chooses this class.
        /// </summary>
        IReadOnlyCharacterStats Stats { get; }

        /// <summary>
        /// The secondary stats for a CombatEntity who chooses this class.
        /// </summary>
        IReadOnlySecondaryStat SecondaryStats { get; }

        /// <summary>
        /// The abilities a CombatEntity starts with when choosing this class.
        /// </summary>
        IEnumerable<IReadOnlyAbility> Abilities { get; }

        /// <summary>
        /// The items that are equipped onto the CombatEntity when choosing this class.
        /// </summary>
        IEnumerable<IReadOnlyItem> EquippedItems { get; }
    }
}