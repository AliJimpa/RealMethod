using UnityEngine;

namespace RealMethod
{
    public static class RM_Trace
    {
        /// <summary>
        /// Casts a debug line (ray) from a starting point in a given direction
        /// with visual feedback for hits and misses.
        /// </summary>
        /// <param name="Start">The origin point of the ray.</param>
        /// <param name="Direction">The direction the ray will fire in.</param>
        /// <param name="Length">The maximum length of the ray.</param>
        /// <param name="Layer">The layer mask that the ray interacts with.</param>
        /// <param name="TriggerInteraction">
        /// Defines whether the ray should hit trigger colliders.
        /// </param>
        /// <param name="RayColor">Color of the ray if no hit occurs.</param>
        /// <param name="HitColor">Color of the ray when a hit occurs.</param>
        /// <param name="HitResult">
        /// Output RaycastHit containing information about the hit.
        /// </param>
        /// <returns>
        /// True if the ray hits a collider; otherwise false.
        /// </returns>
        /// <remarks>
        /// Useful for debugging raycasts in the Scene view.  
        /// Typically implemented using <c>Physics.Raycast()</c> combined with <c>Debug.DrawLine()</c>.
        /// </remarks>
        public static bool Line(Vector3 Start, Vector3 Direction, float Length, LayerMask Layer, QueryTriggerInteraction TriggerInteraction, Color RayColor, Color HitColor, out RaycastHit HitResult)
        {
            bool Result;
            Result = Physics.Raycast(Start, Direction, out HitResult, Length, Layer, TriggerInteraction);
            float debuglength = HitResult.collider ? HitResult.distance : Length;
            Color debugcolor = HitResult.collider ? HitColor : RayColor;
            Debug.DrawRay(Start, Direction * debuglength, debugcolor);
            return Result;
        }

        public static bool Line(Vector3 Start, Vector3 Direction, float Length, LayerMask Layer, QueryTriggerInteraction TriggerInteraction, out RaycastHit HitResult)
        {
            bool Result;
            Result = Physics.Raycast(Start, Direction, out HitResult, Length, Layer, TriggerInteraction);
            float debuglength = HitResult.collider ? HitResult.distance : Length;
            Color debugcolor = HitResult.collider ? Color.green : Color.red;
            Debug.DrawRay(Start, Direction * debuglength, debugcolor);
            return Result;
        }

        public static bool Line(Vector3 Start, Vector3 Direction, float Length, LayerMask Layer, out RaycastHit HitResult)
        {
            bool Result;
            Result = Physics.Raycast(Start, Direction, out HitResult, Length, Layer);
            float debuglength = HitResult.collider ? HitResult.distance : Length;
            Color debugcolor = HitResult.collider ? Color.green : Color.red;
            Debug.DrawRay(Start, Direction * debuglength, debugcolor);
            return Result;
        }

        public static bool Line(Vector3 Start, Vector3 Direction, float Length, out RaycastHit HitResult)
        {
            bool Result;
            Result = Physics.Raycast(Start, Direction, out HitResult, Length);
            float debuglength = HitResult.collider ? HitResult.distance : Length;
            Color debugcolor = HitResult.collider ? Color.green : Color.red;
            Debug.DrawRay(Start, Direction * debuglength, debugcolor);
            return Result;
        }
    }
}