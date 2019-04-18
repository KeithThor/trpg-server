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

        /// <summary>
        /// Returns true if any object in the array matches the given predicate.
        /// </summary>
        /// <typeparam name="T">The type of object of the array.</typeparam>
        /// <param name="arr">The array to filter through.</param>
        /// <param name="predicate">The function used to filter through the array.</param>
        /// <returns></returns>
        public static bool AnyTwoD<T>(this T[][] arr, Func<T, bool> predicate)
        {
            foreach (var row in arr)
            {
                if (row.Any(predicate)) return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if a given two dimensional array contains the object.
        /// </summary>
        /// <typeparam name="T">The type of the object and array.</typeparam>
        /// <param name="arr">The two dimensional array to search through.</param>
        /// <param name="obj">The object to find.</param>
        /// <returns></returns>
        public static bool ContainsTwoD<T>(this T[][] arr, T obj)
        {
            foreach (var row in arr)
            {
                if (row.Contains(obj)) return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the x-index and y-index of an object if it exists in the given
        /// two dimensional array.
        /// </summary>
        /// <typeparam name="T">The type of the object and array.</typeparam>
        /// <param name="arr">The two dimensional array to search through.</param>
        /// <param name="obj">The object to find.</param>
        /// <returns>An integer array containing the x-index and y-index of the found object.</returns>
        public static int[] FindIndexTwoD<T>(this T[][] arr, T obj)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = 0; j < arr[i].Length; j++)
                {
                    if (arr[i][j].Equals(obj))
                    {
                        return new int[] { j, i };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the first object that matches a given predicate, or a default value if no matching
        /// objects are found.
        /// </summary>
        /// <typeparam name="T">The type of the object and array.</typeparam>
        /// <param name="arr">The two dimensional array to search through.</param>
        /// <param name="predicate">A function used to filter out the objects in the arrays.</param>
        /// <returns></returns>
        public static T FirstOrDefaultTwoD<T>(this T[][] arr, Func<T, bool> predicate)
        {
            foreach (var row in arr)
            {
                var obj = row.FirstOrDefault(predicate);
                if (obj != null) return obj;
            }

            return default(T);
        }

        /// <summary>
        /// Returns an IEnumerable of objects that match a given predicate.
        /// </summary>
        /// <typeparam name="T">The type of the object and array.</typeparam>
        /// <param name="arr">The two dimensional array to search through.</param>
        /// <param name="predicate">A function used to filter out the objects in the arrays.</param>
        /// <returns></returns>
        public static IEnumerable<T> WhereTwoD<T>(this T[][] arr, Func<T, bool> predicate)
        {
            var matching = new List<T>();

            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = 0; j < arr[i].Length; j++)
                {
                    if (arr[i][j] == null) continue;
                    if (predicate(arr[i][j]))
                    {
                        matching.Add(arr[i][j]);
                    }
                }
            }

            return matching;
        }
    }
}
