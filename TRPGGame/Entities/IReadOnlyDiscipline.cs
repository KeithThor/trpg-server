using System.Collections.Generic;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a reference to a read-only instance of an in-game Discipline.
    /// </summary>
    public interface IReadOnlyDiscipline
    {
        /// <summary>
        /// The id of this Discipline object.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// A dictionary where the keys are the levels at which a CombatEntity is awarded a new permanent StatusEffect.
        /// </summary>
        IReadOnlyDictionary<int, IReadOnlyStatusEffect> LevelAwards { get; }

        /// <summary>
        /// The Category this Discipline corresponds to.
        /// </summary>
        IReadOnlyCategory Type { get; }
    }
}