using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Utilities
{
    static class LinqExtensions
    {
        /// <summary>
        /// Swaps two items in a list.
        /// </summary>
        public static void Swap<T>(this List<T> list, int indexA, int indexB)
        {
            var itemA = list.At(indexA);
            var itemB = list.At(indexB);

            list[indexB] = itemA;
            list[indexA] = itemB;
        }

        /// <summary>
        /// Gets an item at an index, throws exception if not.
        /// </summary>
        public static T At<T>(this List<T> list, int index)
        {
            if (index >= list.Count || index < 0)
                throw new ArgumentException("Index out of bounds");

            return list[index];
        }

        /// <summary>
        /// Returns first intex that is not assigned, returns null(the irony) if everything is assigned
        /// </summary>
        public static int? FirstNullIndexAt<T>(this List<T> list)
        {
            for(int i = 0; i < list.Count; ++i)
            {
                if (list.At(i) == null)
                    return i;
            }
            return null;
        }
    }
}
