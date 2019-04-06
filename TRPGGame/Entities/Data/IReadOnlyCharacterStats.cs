namespace TRPGGame.Entities.Data
{
    public interface IReadOnlyCharacterStats
    {
        int Agility { get; }
        int Constitution { get; }
        int Dexterity { get; }
        int Intelligence { get; }
        int Strength { get; }

        int[] AsArray();
        int GetTotalStats();
    }
}