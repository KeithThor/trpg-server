using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities.Data
{
    public class CharacterStats
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
    }
}
