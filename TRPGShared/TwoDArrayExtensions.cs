using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TRPGShared
{
    public static class TwoDArrayExtensions
    {
        /// <summary>
        /// Returns the total size of all arrays in the two dimensional array.
        /// 
        /// <para>Includes null objects.</para>
        /// </summary>
        /// <typeparam name="T">The type of the two dimensional array.</typeparam>
        /// <param name="arr">The two dimensional array to get the total size of.</param>
        /// <returns>Returns an int representing the total size of all arrays in the two dimensional array.</returns>
        public static int GetTotalSize<T>(this T[][] arr)
        {
            int total = 0;
            foreach (var row in arr)
            {
                total += row.Count();
            }
            return total;
        }
    }
}
