using System;
using System.Collections.Generic;

namespace RealMethod
{
    public static class RM_Collection
    {
        /// <summary>
        /// Retrieves an element from a collection at the specified index.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="items">The collection of items to retrieve from.</param>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <returns>
        /// The element located at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the index is negative or exceeds the number of elements in the collection.
        /// </exception>
        /// <remarks>
        /// If the provided collection does not support indexed access (<see cref="IList{T}"/>),
        /// it will be converted to a list internally to allow indexing.
        /// </remarks>
        public static T SafeGet<T>(IEnumerable<T> items, int index)
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