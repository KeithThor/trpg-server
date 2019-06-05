using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities.Data
{
    public class CharacterStats : IReadOnlyCharacterStats
    {
        public CharacterStats() { }
        public CharacterStats(IEnumerable<int> stats)
        {
            int i = 0;
            foreach (var val in stats)
            {
                switch (i)
                {
                    case 0:
                        Strength = val;
                        break;
                    case 1:
                        Dexterity = val;
                        break;
                    case 2:
                        Agility = val;
                        break;
                    case 3:
                        Intelligence = val;
                        break;
                    case 4:
                        Constitution = val;
                        break;
                    default:
                        break;
                }
                i++;
            }
        }

        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }
        public int Constitution { get; set; }

        /// <summary>
        /// Gets the total of all of the character stats.
        /// </summary>
        /// <returns></returns>
        public int GetTotalStats()
        {
            return Strength + Dexterity + Agility + Intelligence + Constitution;
        }

        /// <summary>
        /// Returns all stat values as an integer array.
        /// <para>Arranged in the following order: Strength, Dexterity, Agility, Intelligence, Constitution.</para>
        /// </summary>
        /// <returns></returns>
        public int[] AsArray()
        {
            return new int[]
            {
                Strength,
                Dexterity,
                Agility,
                Intelligence,
                Constitution
            };
        }
        
        public static CharacterStats operator + (CharacterStats first, CharacterStats addTo)
        {
            var result = new CharacterStats
            {
                Strength = first.Strength + addTo.Strength,
                Dexterity = first.Dexterity + addTo.Dexterity,
                Agility = first.Agility + addTo.Agility,
                Intelligence = first.Intelligence + addTo.Intelligence,
                Constitution = first.Constitution + addTo.Constitution
            };
            return result;
        }

        public static CharacterStats operator - (CharacterStats first, CharacterStats subtract)
        {
            var result = new CharacterStats
            {
                Strength = first.Strength - subtract.Strength,
                Dexterity = first.Dexterity - subtract.Dexterity,
                Agility = first.Agility - subtract.Agility,
                Intelligence = first.Intelligence - subtract.Intelligence,
                Constitution = first.Constitution - subtract.Constitution
            };
            return result;
        }

        public static CharacterStats operator *(CharacterStats first, CharacterStats second)
        {
            var result = new CharacterStats
            {
                Strength = first.Strength * second.Strength,
                Dexterity = first.Dexterity * second.Dexterity,
                Agility = first.Agility * second.Agility,
                Intelligence = first.Intelligence * second.Intelligence,
                Constitution = first.Constitution * second.Constitution
            };
            return result;
        }
    }
}
