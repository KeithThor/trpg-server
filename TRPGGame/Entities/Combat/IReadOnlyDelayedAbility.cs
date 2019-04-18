namespace TRPGGame.Entities.Combat
{
    /// <summary>
    /// Represents a read-only reference to a delayed ability object.
    /// </summary>
    public interface IReadOnlyDelayedAbility
    {
        /// <summary>
        /// The entity who is preparing the DelayedAbility.
        /// </summary>
        IReadOnlyCombatEntity Actor { get; }

        /// <summary>
        /// The ability that is being delayed.
        /// </summary>
        IReadOnlyAbility BaseAbility { get; }

        /// <summary>
        /// Returns true if the DelayedAbility was a critical hit.
        /// </summary>
        bool IsCrit { get; }

        /// <summary>
        /// The amount of damage calculated at the time of preparing to use the ability.
        /// </summary>
        IReadOnlyDamageTypes StoredDamage { get; }

        /// <summary>
        /// The amount of healing calculated at the time of preparing to use the ability.
        /// </summary>
        int StoredHealing { get; }

        /// <summary>
        /// The Formation of this ability's target.
        /// </summary>
        IReadOnlyFormation TargetFormation { get; }

        /// <summary>
        /// The position this ability is targetting.
        /// </summary>
        int TargetPosition { get; }

        /// <summary>
        /// The amount of rounds left before this ability is activated.
        /// </summary>
        int TurnsLeft { get; }
    }
}