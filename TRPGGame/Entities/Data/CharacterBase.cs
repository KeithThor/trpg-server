namespace TRPGGame.Entities.Data
{
    /// <summary>
    /// Represents a simple data object for a deserialized CharacterBase.
    /// </summary>
    public class CharacterBase
    {
        public int Id { get; set; }
        public string IconUri { get; set; }
        public string Name { get; set; }
        public CharacterStats MaxStats { get; set; } = new CharacterStats();
        public CharacterStats BonusStats { get; set; } = new CharacterStats();
    }
}
