namespace TRPGGame.Entities
{
    /// <summary>
    /// A read-only reference to a ResourceStats object.
    /// </summary>
    public interface IReadOnlyResourceStats
    {
        /// <summary>
        /// The amount of action points an entity currently has.
        /// </summary>
        int CurrentActionPoints { get; }

        /// <summary>
        /// The current amount of health an entity has.
        /// </summary>
        int CurrentHealth { get; }

        /// <summary>
        /// The current amount of mana an entity has.
        /// </summary>
        int CurrentMana { get; }

        /// <summary>
        /// The effective amount of max health an entity has, modified by other effects.
        /// </summary>
        int MaxHealth { get; }

        /// <summary>
        /// The effective amount of max mana an entity has, modified by other effects.
        /// </summary>
        int MaxMana { get; }

        /// <summary>
        /// The amount of max health an entity has before being modified by other effects.
        /// </summary>
        int UnmodifiedMaxHealth { get; }

        /// <summary>
        /// The amount of max mana an entity has before being modified by other effects.
        /// </summary>
        int UnmodifiedMaxMana { get; }
    }
}