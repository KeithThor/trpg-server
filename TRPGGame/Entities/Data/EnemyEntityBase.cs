using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities.Data
{
    /// <summary>
    /// Represents a base CombatEntity to create clones of for AI controlled Formations.
    /// </summary>
    public class EnemyEntityBase
    {
        /// <summary>
        /// The unique identifier for this EnemyEntityBase.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name given to a CombatEntity created from this base.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name of the Faction this CombatEntity belongs to.
        /// </summary>
        public string FactionName { get; set; }

        /// <summary>
        /// The resource stats given to a CombatEntity created from this base.
        /// </summary>
        public ResourceStats Resources { get; set; }

        /// <summary>
        /// The icon uris that make up the visuals for the CombatEntity created from this base.
        /// </summary>
        public IEnumerable<string> IconUris { get; set; }

        /// <summary>
        /// The primary stats given to a CombatEntity created from this base.
        /// </summary>
        public CharacterStats Stats { get; set; }

        /// <summary>
        /// The secondary stats given to a CombatEntity created from this base.
        /// </summary>
        public SecondaryStat SecondaryStats { get; set; }

        /// <summary>
        /// The abilities given to a CombatEntity created from this base.
        /// </summary>
        public List<Ability> Abilities { get; set; }

        /// <summary>
        /// The status effects applied to a CombatEntity created from this base.
        /// </summary>
        public List<StatusEffect> StatusEffects { get; set; }
    }
}
