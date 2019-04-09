using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities.Combat
{
    /// <summary>
    /// Represents a StatusEffect that has been applied to a CombatEntity.
    /// </summary>
    public class AppliedStatusEffect : IReadOnlyAppliedStatusEffect
    {
        /// <summary>
        /// The StatusEffect that is the base for this AppliedStatusEffect.
        /// </summary>
        public StatusEffect BaseStatus { get; set; }

        /// <summary>
        /// The total amount of damage dealt every turn to the CombatEntity who is affected by this AppliedStatusEffect.
        /// </summary>
        public DamageTypes CumulativeDamage { get; set; }

        /// <summary>
        /// The total amount of health healed every turn to the CombatEntity affected by this AppliedStatusEffect.
        /// </summary>
        public int CumulativeHeal { get; set; }

        /// <summary>
        /// The current stack size of the StatusEffect applied to the CombatEntity.
        /// </summary>
        public int CurrentStacks { get; set; }

        /// <summary>
        /// The amount of turns remaining on this AppliedStatusEffect before it is removed.
        /// </summary>
        public int Duration { get; set; }

        IReadOnlyStatusEffect IReadOnlyAppliedStatusEffect.BaseStatus => BaseStatus;

        IReadOnlyDamageTypes IReadOnlyAppliedStatusEffect.CumulativeDamage => CumulativeDamage;
    }
}
