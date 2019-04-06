using System;
using System.Collections.Generic;
using System.Text;
using TRPGGame.Entities.Data;

namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents an in-game buff or debuff.
    /// </summary>
    public class StatusEffect : IReadOnlyStatusEffect
    {
        /// <summary>
        /// The unique identifier for this StatusEffect.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The display name of this StatusEffect.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of this StatusEffect.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The string paths of the uris that creates the icons that represents this StatusEffect.
        /// </summary>
        public IEnumerable<string> IconUris { get; set; }

        /// <summary>
        /// The max duration of this StatusEffect.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// The max amount of stacks of this StatusEffect that can be applied to the same target.
        /// <para>An amount of one will mean the StatusEffect cannot stack.</para>
        /// </summary>
        public int StackSize { get; set; }

        /// <summary>
        /// Whether or not the StatusEffect is magical in nature. Used to determine if a StatusEffect can be dispelled or healed.
        /// </summary>
        public bool IsMagical { get; set; }

        /// <summary>
        /// If true, this StatusEffect will be considered a debuff, if false, it will be considered a buff.
        /// </summary>
        public bool IsDebuff { get; set; }

        /// <summary>
        /// Initial damage dealt per turn to the target of this StatusEffect.
        /// </summary>
        public DamageTypes DamagePerTurn { get; set; }

        /// <summary>
        /// Damage dealt per turn per point of stat of the character who applied the StatusEffect.
        /// </summary>
        public DamagePerStat DamagePerStatPerTurn { get; set; }

        /// <summary>
        /// Initial health healed per turn to the target of this StatusEffect.
        /// </summary>
        public int HealPerTurn { get; set; }

        /// <summary>
        /// Amount of health in percentage of max health healed per turn.
        /// </summary>
        public int PercentHealPerTurn { get; set; }

        /// <summary>
        /// Amount of health healed per point of stat of the character who applied the StatusEffect.
        /// </summary>
        public CharacterStats HealPerStatPerTurn { get; set; }

        /// <summary>
        /// Amount of primary stats modified by this StatusEffect.
        /// </summary>
        public CharacterStats ModifiedStats { get; set; }

        /// <summary>
        /// Amount of primary stats modified in percentage by this StatusEffect.
        /// </summary>
        public CharacterStats ModifiedStatPercentages { get; set; }

        /// <summary>
        /// The amount of secondary stats modified by this StatusEffect.
        /// </summary>
        public SecondaryStat ModifiedSecondaryStats { get; set; }

        IReadOnlyDamageTypes IReadOnlyStatusEffect.DamagePerTurn => DamagePerTurn;

        IReadOnlyDamagePerStat IReadOnlyStatusEffect.DamagePerStatPerTurn => DamagePerStatPerTurn;

        IReadOnlyCharacterStats IReadOnlyStatusEffect.HealPerStatPerTurn => HealPerStatPerTurn;

        IReadOnlyCharacterStats IReadOnlyStatusEffect.ModifiedStats => ModifiedStats;

        IReadOnlyCharacterStats IReadOnlyStatusEffect.ModifiedStatPercentages => ModifiedStatPercentages;

        IReadOnlySecondaryStat IReadOnlyStatusEffect.ModifiedSecondaryStats => ModifiedSecondaryStats;
    }
}
