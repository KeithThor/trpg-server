using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities
{
    /// <summary>
    /// A class that represents each different type of Discipline for each ability Category.
    /// </summary>
    public class Discipline : IReadOnlyDiscipline
    {
        /// <summary>
        /// The id of this Discipline object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Category this Discipline corresponds to.
        /// </summary>
        public Category Type { get; set; }

        /// <summary>
        /// A dictionary where the keys are the levels at which a CombatEntity is awarded a new permanent StatusEffect.
        /// </summary>
        public Dictionary<int, StatusEffect> LevelAwards { get; set; }

        IReadOnlyDictionary<int, IReadOnlyStatusEffect> IReadOnlyDiscipline.LevelAwards
        {
            get
            {
                return (IReadOnlyDictionary<int, IReadOnlyStatusEffect>)LevelAwards;
            }
        }

        IReadOnlyCategory IReadOnlyDiscipline.Type => Type;
    }
}
