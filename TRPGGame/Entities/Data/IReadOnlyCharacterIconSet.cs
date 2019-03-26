namespace TRPGGame.Entities.Data
{
    /// <summary>
    /// Represents a read only character icon set.
    /// </summary>
    public interface IReadOnlyCharacterIconSet
    {
        string BaseIconUri { get; }
        string HairIconUri { get; }
        string CloakIconUri { get; }
        string LeftHandIconUri { get; }
        string RightHandIconUri { get; }
        string HeadIconUri { get; }
        string GlovesIconUri { get; }
        string LegsIconUri { get; }
        string BodyIconUri { get; }
        string ExtraIconUri { get; }
    }
}