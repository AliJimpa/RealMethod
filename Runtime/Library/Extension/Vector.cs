using UnityEngine;

namespace RealMethod
{
    public static class Vector3_Extension
    {
        /// <summary>
        /// Returns a Vector3 with only the X component.
        /// </summary>
        public static Vector3 OnlyX(this Vector3 v) => new Vector3(v.x, 0f, 0f);

        /// <summary>
        /// Returns a Vector3 with only the Y component.
        /// </summary>
        public static Vector3 OnlyY(this Vector3 v) => new Vector3(0f, v.y, 0f);

        /// <summary>
        /// Returns a Vector3 with only the Z component.
        /// </summary>
        public static Vector3 OnlyZ(this Vector3 v) => new Vector3(0f, 0f, v.z);

        /// <summary>
        /// Sets the X component of a Vector3.
        /// </summary>
        public static Vector3 WithX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);

        /// <summary>
        /// Sets the Y component of a Vector3.
        /// </summary>
        public static Vector3 WithY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);

        /// <summary>
        /// Sets the Z component of a Vector3.
        /// </summary>
        public static Vector3 WithZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);

        /// <summary>
        /// Projects the vector onto a plane defined by a normal.
        /// </summary>
        public static Vector3 ProjectOnPlane(this Vector3 v, Vector3 planeNormal)
            => Vector3.ProjectOnPlane(v, planeNormal);

        /// <summary>
        /// Checks if a Vector3 is nearly zero (within a small threshold).
        /// </summary>
        public static bool IsNearlyZero(this Vector3 v, float epsilon = 0.0001f)
            => v.sqrMagnitude < epsilon * epsilon;

        /// <summary>
        /// Converts Vector3 to Vector2 by dropping the Z component.
        /// </summary>
        public static Vector2 ToVector2XZ(this Vector3 v) => new Vector2(v.x, v.z);

        /// <summary>
        /// Converts Vector3 to Vector2 by dropping the Y component.
        /// </summary>
        public static Vector2 ToVector2XY(this Vector3 v) => new Vector2(v.x, v.y);

        /// <summary>
        /// Rounds the components of the vector to the nearest integer.
        /// </summary>
        public static Vector3 Round(this Vector3 v) => new Vector3(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));

        /// <summary>
        /// Clamps each component of the vector independently.
        /// </summary>
        public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max) =>
            new Vector3(Mathf.Clamp(v.x, min.x, max.x), Mathf.Clamp(v.y, min.y, max.y), Mathf.Clamp(v.z, min.z, max.z));

        /// <summary>
        /// Moves the vector toward a target by maxDistanceDelta.
        /// </summary>
        public static Vector3 MoveTowards(this Vector3 current, Vector3 target, float maxDistanceDelta) =>
            Vector3.MoveTowards(current, target, maxDistanceDelta);

        public static Vector3 Multiply(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 Multiply(this Vector3 a, float b)
        {
            return new Vector3(a.x * b, a.y * b, a.z * b);
        }
        public static Hash128 ConvertToHash(this Vector3 Target)
        {
            return Hash128.Compute(Target.x + "." + Target.y + "." + Target.z);
        }
        public static Vector2 ConvertToVector2(this Vector3 Target)
        {
            return new Vector2(Target.x, Target.z);
        }
        public static Vector2Int ConvertToVector2Int(this Vector3 Target)
        {
            return new Vector2Int((int)Target.x, (int)Target.z);
        }

    }
    public static class Vector3Int_Extension
    {
        public static Hash128 ConvertToHash(this Vector3Int Target)
        {
            return Hash128.Compute(Target.x + "." + Target.y + "." + Target.z);
        }
    }
    public static class Vector2_Extension
    {
        public static Hash128 ConvertToHash(this Vector2 Target)
        {
            return Hash128.Compute(Target.x + "." + Target.y);
        }
        public static Vector3 ConvertToVector3(this Vector2 Target)
        {
            return new Vector3(Target.x, 0, Target.y);
        }
    }
    public static class Vector2Int_Extension
    {
        public static Hash128 ConvertToHash(this Vector2Int Target)
        {
            return Hash128.Compute(Target.x + "." + Target.y);
        }

    }

}