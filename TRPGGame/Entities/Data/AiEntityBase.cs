using System.Collections.Generic;

namespace TRPGGame.Entities.Data
{
    /// <summary>
    /// Represents a base CombatEntity to create clones of for AI controlled Formations.
    /// </summary>
    public class AiEntityBase
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
        public ResourceStats Resources { get; set; } = new ResourceStats();

        /// <summary>
        /// The icon uris that make up the visuals for the CombatEntity created from this base.
        /// </summary>
        public IEnumerable<string> IconUris { get; set; }

        /// <summary>
        /// The primary stats given to a CombatEntity created from this base.
        /// </summary>
        public CharacterStats Stats { get; set; } = new CharacterStats();

        /// <summary>
        /// The secondary stats given to a CombatEntity created from this base.
        /// </summary>
        public SecondaryStat SecondaryStats { get; set; } = new SecondaryStat();

        /// <summary>
        /// The abilities given to a CombatEntity created from this base.
        /// </summary>
        public List<Ability> Abilities { get; set; } = new List<Ability>();

        /// <summary>
        /// The status effects applied to a CombatEntity created from this base.
        /// </summary>
        public List<StatusEffect> StatusEffects { get; set; } = new List<StatusEffect>();
    }
}
