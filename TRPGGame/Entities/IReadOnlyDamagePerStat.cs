namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a read-only reference to a DamagePerStat object.
    /// </summary>
    public interface IReadOnlyDamagePerStat
    {
        IReadOnlyDamageTypes Agility { get; }
        IReadOnlyDamageTypes Constitution { get; }
        IReadOnlyDamageTypes Dexterity { get; }
        IReadOnlyDamageTypes Intelligence { get; }
        IReadOnlyDamageTypes Strength { get; }
    }
}