using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities.Data;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents an in-game combat ability that can be an attack, spell, skill, or an item spell.
    /// </summary>
    public class Ability : IReadOnlyAbility
    {
        /// <summary>
        /// The unique identifier for this ability.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of this ability.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The category this ability belongs to.
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// The description of this ability.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The string paths of the uris that creates the icons that represents this ability.
        /// </summary>
        public IEnumerable<string> IconUris { get; set; }

        /// <summary>
        /// The positions this ability targets.
        /// <para>Friendly positions are 1-9, enemy positions are 10-18.</para>
        /// </summary>
        public IEnumerable<int> Targets { get; set; }

        /// <summary>
        /// The center of this ability's target positions. This is what the player uses to aim the ability.
        /// </summary>
        public int CenterOfTargets { get; set; }

        /// <summary>
        /// Whether or not the ability uses the caster's position as the target position.
        /// <para>Will ignore IsPositionStatic and CanTargetBeBlocked if true.</para>
        /// </summary>
        public bool IsPointBlank { get; set; }

        /// <summary>
        /// Whether or not the player can move the CenterOfTargets position for this ability.
        /// </summary>
        public bool IsPositionStatic { get; set; }

        /// <summary>
        /// Whether or not the ability can be used against targets hiding behind other characters in the formation.
        /// </summary>
        public bool CanTargetBeBlocked { get; set; }

        /// <summary>
        /// If true, places this ability in the Spells command slot.
        /// </summary>
        public bool IsSpell { get; set; }

        /// <summary>
        /// If true, plaes this ability in the Skills command slot.
        /// </summary>
        public bool IsSkill { get; set; }

        /// <summary>
        /// The amount of action points the character loses (or gains) by using this ability.
        /// </summary>
        public int ActionPointCost { get; set; }

        /// <summary>
        /// The amount of health the character loses (or gains) by using this ability.
        /// </summary>
        public int HealthCost { get; set; }

        /// <summary>
        /// The amount of health in percentage the character loses (or gains) by using this ability.
        /// </summary>
        public int HealthPercentCost { get; set; }

        /// <summary>
        /// The amount of mana the character loses (or gains) by using this ability.
        /// </summary>
        public int ManaCost { get; set; }

        /// <summary>
        /// The amount of mana in percentage the character loses (or gains) by using this ability.
        /// </summary>
        public int ManaPercentCost { get; set; }

        /// <summary>
        /// The initial amount of damage this ability deals without being modified by anything else.
        /// </summary>
        public DamageTypes Damage { get; set; }

        /// <summary>
        /// The amount of each type of damage increased by each point of stat the caster has.
        /// </summary>
        public DamagePerStat DamagePerStat { get; set; }

        /// <summary>
        /// The initial amount of hit points healed to the targets of this ability before any bonuses are applied.
        /// </summary>
        public int Heal { get; set; }

        /// <summary>
        /// The amount of hit points healed increased by each point of stat the caster has.
        /// </summary>
        public CharacterStats HealPerStat { get; set; }

        /// <summary>
        /// The amount of damage in percentage of max health done by this ability against the targets.
        /// </summary>
        public DamageTypes PercentDamage { get; set; }

        /// <summary>
        /// The amount of health in percentage of max health healed by this ability.
        /// </summary>
        public int PercentHeal { get; set; }

        /// <summary>
        /// Status effects applied by this ability to its targets.
        /// </summary>
        public IEnumerable<StatusEffect> AppliedStatusEffects { get; set; }

        /// <summary>
        /// Status effects applied by thisa ability to the caster.
        /// </summary>
        public IEnumerable<StatusEffect> SelfAppliedStatusEffects { get; set; }

        IReadOnlyDamageTypes IReadOnlyAbility.Damage => Damage;

        IReadOnlyDamagePerStat IReadOnlyAbility.DamagePerStat => DamagePerStat;

        IReadOnlyCharacterStats IReadOnlyAbility.HealPerStat => HealPerStat;

        IReadOnlyDamageTypes IReadOnlyAbility.PercentDamage => PercentDamage;

        IEnumerable<IReadOnlyStatusEffect> IReadOnlyAbility.AppliedStatusEffects => AppliedStatusEffects;

        IEnumerable<IReadOnlyStatusEffect> IReadOnlyAbility.SelfAppliedStatusEffects => SelfAppliedStatusEffects;

        IReadOnlyCategory IReadOnlyAbility.Category => Category;
    }
}
