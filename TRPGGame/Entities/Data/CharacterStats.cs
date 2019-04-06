using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities.Data
{
    public class CharacterStats : IReadOnlyCharacterStats
    {
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }
        public int Constitution { get; set; }

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
    }
}
