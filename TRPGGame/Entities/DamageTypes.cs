using System;
using System.Collections.Generic;
using System.Text;

namespace TRPGGame.Entities
{
    /// <summary>
    /// A POCO that contains all the possible different types of damage.
    /// </summary>
    public class DamageTypes : IReadOnlyDamageTypes
    {
        public int Blunt { get; set; }
        public int Sharp { get; set; }
        public int Fire { get; set; }
        public int Frost { get; set; }
        public int Lightning { get; set; }
        public int Earth { get; set; }
        public int Holy { get; set; }
        public int Shadow { get; set; }

        /// <summary>
        /// Returns all the different types of damage as an integer array.
        /// <para>Order matters.</para>
        /// </summary>
        /// <returns></returns>
        public int[] AsArray()
        {
            return new int[]
            {
                Blunt,
                Sharp,
                Fire,
                Frost,
                Lightning,
                Earth,
                Holy,
                Shadow
            };
        }
    }
}
