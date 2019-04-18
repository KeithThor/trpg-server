using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace TRPGGame.Entities.Data
{
    /// <summary>
    /// Class containing the icon Uri's for all of a character entity's visuals.
    /// </summary>
    [Owned]
    public class CharacterIconSet : IReadOnlyCharacterIconSet
    {
        public CharacterIconSet() { }
        public CharacterIconSet(IEnumerable<string> iconUris)
        {
            var iconUriArray = iconUris.ToArray();
            for (int i = 0; i < iconUris.Count(); i++)
            {
                switch(i)
                {
                    case 0:
                        BaseIconUri = iconUriArray[i];
                        break;
                    case 1:
                        BootsIconUri = iconUriArray[i];
                        break;
                    case 2:
                        HairIconUri = iconUriArray[i];
                        break;
                    case 3:
                        CloakIconUri = iconUriArray[i];
                        break;
                    case 4:
                        LeftHandIconUri = iconUriArray[i];
                        break;
                    case 5:
                        RightHandIconUri = iconUriArray[i];
                        break;
                    case 6:
                        HeadIconUri = iconUriArray[i];
                        break;
                    case 7:
                        GlovesIconUri = iconUriArray[i];
                        break;
                    case 8:
                        LegsIconUri = iconUriArray[i];
                        break;
                    case 9:
                        BodyIconUri = iconUriArray[i];
                        break;
                    case 10:
                        ExtraIconUri = iconUriArray[i];
                        break;
                    default:
                        break;
                }
            }
        }
        public CharacterIconSet(CharacterIconSet iconSet)
        {
            BaseIconUri = iconSet.BaseIconUri;
            BootsIconUri = iconSet.BootsIconUri;
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
        public string BootsIconUri { get; set; }
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
