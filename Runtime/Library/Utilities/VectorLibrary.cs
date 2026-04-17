using UnityEngine;

namespace RealMethod
{
    public static class RM_Vector
    {
        /// <summary>
        /// Determines whether two vectors are aligned within a specified tolerance.
        /// </summary>
        /// <param name="vectorA">The first vector.</param>
        /// <param name="vectorB">The second vector.</param>
        /// <param name="tolerance">
        /// The allowed deviation when comparing alignment.
        /// Smaller values require closer directional similarity.
        /// </param>
        /// <returns>
        /// True if the vectors are aligned within the given tolerance; otherwise false.
        /// </returns>
        /// <remarks>
        /// Alignment usually means the vectors point in nearly the same direction,
        /// typically evaluated using the dot product or angle between them.
        /// </remarks>
        public static bool IsAligned(Vector2 vectorA, Vector2 vectorB, float tolerance)
        {
            float angle = Vector2.Angle(vectorA.normalized, vectorB.normalized);
            if (angle <= tolerance) return true;

            return false;
        }
    }
}