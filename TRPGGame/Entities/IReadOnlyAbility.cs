using System.Collections.Generic;
using TRPGGame.Entities.Data;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a read-only reference to an Ability.
    /// </summary>
    public interface IReadOnlyAbility
    {
        /// <summary>
        /// The unique identifier for this ability.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of this ability.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description of this ability.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The category this ability belongs to.
        /// </summary>
        IReadOnlyCategory Category { get; }

        /// <summary>
        /// The string paths of the uris that creates the icons that represents this ability.
        /// </summary>
        IEnumerable<string> IconUris { get; }

        /// <summary>
        /// The positions this ability targets.
        /// <para>Friendly positions are 1-9, enemy positions are 10-18.</para>
        /// </summary>
        IEnumerable<int> Targets { get; }

        /// <summary>
        /// The center of this ability's target positions. This is what the player uses to aim the ability.
        /// </summary>
        int CenterOfTargets { get; }

        /// <summary>
        /// Whether or not the ability uses the caster's position as the target position.
        /// <para>Will ignore IsPositionStatic and CanTargetBeBlocked if true.</para>
        /// </summary>
        bool IsPointBlank { get; }

        /// <summary>
        /// Whether or not the player can move the CenterOfTargets position for this ability.
        /// </summary>
        bool IsPositionStatic { get; }

        /// <summary>
        /// Whether or not the ability can be used against targets hiding behind other characters in the formation.
        /// </summary>
        bool CanTargetBeBlocked { get; }

        /// <summary>
        /// If true, places this ability in the Spells command slot.
        /// </summary>
        bool IsSpell { get; }

        /// <summary>
        /// If true, plaes this ability in the Skills command slot.
        /// </summary>
        bool IsSkill { get; }

        /// <summary>
        /// The amount of action points the character loses (or gains) by using this ability.
        /// </summary>
        int ActionPointCost { get; }

        /// <summary>
        /// The amount of health the character loses (or gains) by using this ability.
        /// </summary>
        int HealthCost { get; }

        /// <summary>
        /// The amount of health in percentage the character loses (or gains) by using this ability.
        /// </summary>
        int HealthPercentCost { get; }

        /// <summary>
        /// The amount of mana the character loses (or gains) by using this ability.
        /// </summary>
        int ManaCost { get; }

        /// <summary>
        /// The amount of mana in percentage the character loses (or gains) by using this ability.
        /// </summary>
        int ManaPercentCost { get; }

        /// <summary>
        /// The initial amount of damage this ability deals without being modified by anything else.
        /// </summary>
        IReadOnlyDamageTypes Damage { get; }

        /// <summary>
        /// The amount of each type of damage increased by each point of stat the caster has.
        /// </summary>
        IReadOnlyDamagePerStat DamagePerStat { get; }

        /// <summary>
        /// The initial amount of hit points healed to the targets of this ability before any bonuses are applied.
        /// </summary>
        int Heal { get; }

        /// <summary>
        /// The amount of hit points healed increased by each point of stat the caster has.
        /// </summary>
        IReadOnlyCharacterStats HealPerStat { get; }

        /// <summary>
        /// The amount of damage in percentage of max health done by this ability against the targets.
        /// </summary>
        IReadOnlyDamageTypes PercentDamage { get; }

        /// <summary>
        /// The amount of health in percentage of max health healed by this ability.
        /// </summary>
        int PercentHeal { get; }

        /// <summary>
        /// Status effects applied by this ability to its targets.
        /// </summary>
        IEnumerable<IReadOnlyStatusEffect> AppliedStatusEffects { get; }

        /// <summary>
        /// Status effects applied by thisa ability to the caster.
        /// </summary>
        IEnumerable<IReadOnlyStatusEffect> SelfAppliedStatusEffects { get; }
    }
}