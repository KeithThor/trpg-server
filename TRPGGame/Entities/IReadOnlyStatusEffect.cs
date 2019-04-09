using System.Collections.Generic;
using TRPGGame.Entities.Data;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a read-only reference to an in-game status effect.
    /// </summary>
    public interface IReadOnlyStatusEffect
    {
        /// <summary>
        /// The unique identifier for this StatusEffect.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The display name of this StatusEffect.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description of this StatusEffect.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The string paths that point to the uris of the icons that represent this StatusEffect.
        /// </summary>
        IEnumerable<string> IconUris { get; }

        /// <summary>
        /// The max duration of this StatusEffect.
        /// </summary>
        int Duration { get; }

        /// <summary>
        /// The max amount of stacks of this StatusEffect that can be applied to the same target.
        /// <para>An amount of one will mean the StatusEffect cannot stack.</para>
        /// </summary>
        int StackSize { get; }

        /// <summary>
        /// Whether or not the StatusEffect is magical in nature. Used to determine if a StatusEffect can be dispelled or healed.
        /// </summary>
        bool IsMagical { get; }

        /// <summary>
        /// If true, this StatusEffect will not be able to be removed in combat.
        /// </summary>
        bool IsPermanent { get; }

        /// <summary>
        /// If true, the CombatEntity this StatusEffect is applied to will not be able to act in combat.
        /// </summary>
        bool IsStunned { get; }

        /// <summary>
        /// If true, the CombatEntity this StatusEffect is applied to will not be able to cast spells in combat. 
        /// </summary>
        bool IsSilenced { get; }

        /// <summary>
        /// If true, the CombatEntity this StatusEffect is applied to will not be able to use attacks or skills in combat. 
        /// </summary>
        bool IsRestricted { get; }

        /// <summary>
        /// If true, will not display this StatusEffect in the UI.
        /// </summary>
        bool IsHidden { get; }

        /// <summary>
        /// If true, this StatusEffect will be considered a debuff, if false, it will be considered a buff.
        /// </summary>
        bool IsDebuff { get; }

        /// <summary>
        /// Initial damage dealt per turn to the target of this StatusEffect.
        /// </summary>
        IReadOnlyDamageTypes DamagePerTurn { get; }

        /// <summary>
        /// Damage dealt per turn per point of stat of the character who applied the StatusEffect.
        /// </summary>
        IReadOnlyDamagePerStat DamagePerStatPerTurn { get; }

        /// <summary>
        /// Initial health healed per turn to the target of this StatusEffect.
        /// </summary>
        int HealPerTurn { get; }

        /// <summary>
        /// Amount of health in percentage of max health healed per turn.
        /// </summary>
        int PercentHealPerTurn { get; }

        /// <summary>
        /// Amount of health healed per point of stat of the character who applied the StatusEffect.
        /// </summary>
        IReadOnlyCharacterStats HealPerStatPerTurn { get; }

        /// <summary>
        /// Amount of primary stats modified by this StatusEffect.
        /// </summary>
        IReadOnlyCharacterStats ModifiedStats { get; }

        /// <summary>
        /// Amount of primary stats modified in percentage by this StatusEffect.
        /// </summary>
        IReadOnlyCharacterStats ModifiedStatPercentages { get; }

        /// <summary>
        /// The amount of secondary stats modified by this StatusEffect.
        /// </summary>
        IReadOnlySecondaryStat ModifiedSecondaryStats { get; }
    }
}