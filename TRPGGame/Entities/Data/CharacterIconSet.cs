using Microsoft.EntityFrameworkCore;

namespace TRPGGame.Entities.Data
{
    /// <summary>
    /// Class containing the icon Uri's for all of a character entity's visuals.
    /// </summary>
    [Owned]
    public class CharacterIconSet : IReadOnlyCharacterIconSet
    {
        public CharacterIconSet() { }
        public CharacterIconSet(CharacterIconSet iconSet)
        {
            BaseIconUri = iconSet.BaseIconUri;
            HairIconUri = iconSet.HairIconUri;
            CloakIconUri = iconSet.CloakIconUri;
            LeftHandIconUri = iconSet.LeftHandIconUri;
            RightHandIconUri = iconSet.RightHandIconUri;
            HeadIconUri = iconSet.HeadIconUri;
            GlovesIconUri = iconSet.GlovesIconUri;
            LegsIconUri = iconSet.LegsIconUri;
            BodyIconUri = iconSet.BodyIconUri;
            ExtraIconUri = iconSet.ExtraIconUri;
        }

        public string BaseIconUri { get; set; }
        public string HairIconUri { get; set; }
        public string CloakIconUri { get; set; }
        public string LeftHandIconUri { get; set; }
        public string RightHandIconUri { get; set; }
        public string HeadIconUri { get; set; }
        public string GlovesIconUri { get; set; }
        public string LegsIconUri { get; set; }
        public string BodyIconUri { get; set; }
        public string ExtraIconUri { get; set; }
    }
}
