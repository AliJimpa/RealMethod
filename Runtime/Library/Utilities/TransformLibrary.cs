using UnityEngine;

namespace RealMethod
{
    public static class RM_Transform
    {
        /// <summary>
        /// Converts a position defined relative to a transform into world space.
        /// </summary>
        /// <param name="from">The reference transform.</param>
        /// <param name="relativePos">The position relative to the transform.</param>
        /// <returns>The corresponding world space position.</returns>
        /// <remarks>
        /// Useful when you want to compute positions relative to an object
        /// and convert them to world coordinates.
        /// </remarks>
        public static Vector3 GetWorldPositionFromRelative(Transform from, Vector3 relativePos)
        {
            return from.TransformPoint(relativePos);
        }
        public static Vector3 GetRelativePosition(Transform from, Transform to)
        {
            return from.InverseTransformPoint(to.position);
        }
    }
}