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

        public static DamageTypes operator + (DamageTypes first, DamageTypes addTo)
        {
            return new DamageTypes
            {
                Blunt = first.Blunt + addTo.Blunt,
                Sharp = first.Sharp + addTo.Sharp,
                Fire = first.Fire + addTo.Fire,
                Frost = first.Frost + addTo.Frost,
                Lightning = first.Lightning + addTo.Lightning,
                Earth = first.Earth + addTo.Earth,
                Holy = first.Holy + addTo.Holy,
                Shadow = first.Shadow + addTo.Shadow,
            };
        }

        public static DamageTypes operator +(DamageTypes first, int value)
        {
            return new DamageTypes
            {
                Blunt = first.Blunt + value,
                Sharp = first.Sharp + value,
                Fire = first.Fire + value,
                Frost = first.Frost + value,
                Lightning = first.Lightning + value,
                Earth = first.Earth + value,
                Holy = first.Holy + value,
                Shadow = first.Shadow + value,
            };
        }

        public static DamageTypes operator -(DamageTypes first, DamageTypes subtract)
        {
            return new DamageTypes
            {
                Blunt = first.Blunt - subtract.Blunt,
                Sharp = first.Sharp - subtract.Sharp,
                Fire = first.Fire - subtract.Fire,
                Frost = first.Frost - subtract.Frost,
                Lightning = first.Lightning - subtract.Lightning,
                Earth = first.Earth - subtract.Earth,
                Holy = first.Holy - subtract.Holy,
                Shadow = first.Shadow - subtract.Shadow,
            };
        }

        public static DamageTypes operator *(DamageTypes first, int multiplier)
        {
            return new DamageTypes
            {
                Blunt = first.Blunt * multiplier,
                Sharp = first.Sharp * multiplier,
                Fire = first.Fire * multiplier,
                Frost = first.Frost * multiplier,
                Lightning = first.Lightning * multiplier,
                Earth = first.Earth * multiplier,
                Holy = first.Holy * multiplier,
                Shadow = first.Shadow * multiplier
            };
        }

        public static DamageTypes operator *(DamageTypes first, DamageTypes second)
        {
            return new DamageTypes
            {
                Blunt = first.Blunt * second.Blunt,
                Sharp = first.Sharp * second.Sharp,
                Fire = first.Fire * second.Fire,
                Frost = first.Frost * second.Frost,
                Lightning = first.Lightning * second.Lightning,
                Earth = first.Earth * second.Earth,
                Holy = first.Holy * second.Holy,
                Shadow = first.Shadow * second.Shadow,
            };
        }

        public static DamageTypes operator /(DamageTypes first, int divideBy)
        {
            return new DamageTypes
            {
                Blunt = first.Blunt / divideBy,
                Sharp = first.Sharp / divideBy,
                Fire = first.Fire / divideBy,
                Frost = first.Frost / divideBy,
                Lightning = first.Lightning / divideBy,
                Earth = first.Earth / divideBy,
                Holy = first.Holy / divideBy,
                Shadow = first.Shadow / divideBy
            };
        }
    }
}
