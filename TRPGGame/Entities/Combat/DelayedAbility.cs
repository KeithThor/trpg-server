using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities.Combat
{
    /// <summary>
    /// Represents an in-game combat ability that is delayed for another turn before activating.
    /// </summary>
    public class DelayedAbility : IReadOnlyDelayedAbility
    {
        /// <summary>
        /// The ability that is being delayed.
        /// </summary>
        public Ability BaseAbility { get; set; }

        /// <summary>
        /// The amount of rounds left before this ability is activated.
        /// </summary>
        public int TurnsLeft { get; set; }

        /// <summary>
        /// The entity who is preparing the DelayedAbility.
        /// </summary>
        public CombatEntity Actor { get; set; }

        /// <summary>
        /// The amount of healing calculated at the time of preparing to use the ability.
        /// </summary>
        public int StoredHealing { get; set; }

        /// <summary>
        /// Returns true if the DelayedAbility was a critical hit.
        /// </summary>
        public bool IsCrit { get; set; }

        /// <summary>
        /// The amount of damage calculated at the time of preparing to use the ability.
        /// </summary>
        public DamageTypes StoredDamage { get; set; }

        /// <summary>
        /// The Formation of this ability's target.
        /// </summary>
        public Formation TargetFormation { get; set; }

        /// <summary>
        /// The position this ability is targetting.
        /// </summary>
        public int TargetPosition { get; set; }

        IReadOnlyCombatEntity IReadOnlyDelayedAbility.Actor => Actor;
        IReadOnlyAbility IReadOnlyDelayedAbility.BaseAbility => BaseAbility;
        IReadOnlyDamageTypes IReadOnlyDelayedAbility.StoredDamage => StoredDamage;
        IReadOnlyFormation IReadOnlyDelayedAbility.TargetFormation => TargetFormation;
    }
}
