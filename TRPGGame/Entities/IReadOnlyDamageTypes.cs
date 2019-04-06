namespace TRPGGame.Entities
{
    /// <summary>
    /// Represents a read-only reference to a DamageType object.
    /// </summary>
    public interface IReadOnlyDamageTypes
    {
        int Blunt { get; }
        int Earth { get; }
        int Fire { get; }
        int Frost { get; }
        int Holy { get; }
        int Lightning { get; }
        int Shadow { get; }
        int Sharp { get; }

        int[] AsArray();
    }
}