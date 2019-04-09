namespace TRPGGame.Entities.Combat
{
    /// <summary>
    /// Represents a read-only reference to an AppliedStatusEffect.
    /// </summary>
    public interface IReadOnlyAppliedStatusEffect
    {
        /// <summary>
        /// The StatusEffect that is the base for this AppliedStatusEffect.
        /// </summary>
        IReadOnlyStatusEffect BaseStatus { get; }

        /// <summary>
        /// The total amount of damage dealt every turn to the CombatEntity who is affected by this AppliedStatusEffect.
        /// </summary>
        IReadOnlyDamageTypes CumulativeDamage { get; }

        /// <summary>
        /// The total amount of health healed every turn to the CombatEntity affected by this AppliedStatusEffect.
        /// </summary>
        int CumulativeHeal { get; }

        /// <summary>
        /// The current stack size of the StatusEffect applied to the CombatEntity.
        /// </summary>
        int CurrentStacks { get; }

        /// <summary>
        /// The amount of turns remaining on this AppliedStatusEffect before it is removed.
        /// </summary>
        int Duration { get; }
    }
}