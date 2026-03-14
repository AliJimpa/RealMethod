using System;

namespace RealMethod
{
    public static class RM_Enum
    {
        /// <summary>
        /// Compares two enum values, even if they are of different enum types,
        /// by comparing their underlying numeric values.
        /// </summary>
        /// <typeparam name="T">The type of the first enum.</typeparam>
        /// <typeparam name="J">The type of the second enum.</typeparam>
        /// <param name="a">The first enum value.</param>
        /// <param name="b">The second enum value.</param>
        /// <returns>
        /// True if both enum values have the same underlying numeric value; otherwise false.
        /// </returns>
        /// <remarks>
        /// Useful when working with different enum types that share the same value mapping.
        /// </remarks>

        public static bool AreEnumValuesEqual<T, J>(T a, J b) where T : Enum where J : Enum
        {
            return Convert.ToInt32(a) == Convert.ToInt32(b);
        }
    }
}