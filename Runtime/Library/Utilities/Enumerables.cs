using System;
using System.Collections.Generic;

namespace RealMethod
{
    static class Enumerables
    {
        public static T GetGameObject<T>(IEnumerable<T> items, int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be non-negative.");

            // Convert to array or list if you need index access
            var itemList = items as IList<T> ?? new List<T>(items);

            if (index >= itemList.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index exceeds the collection count.");

            return itemList[index];
        }
    }
}