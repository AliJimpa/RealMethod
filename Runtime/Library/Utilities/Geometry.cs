using UnityEngine;

namespace RealMethod
{
    public static class RM_Geometry
    {
        public class Space
        {
            public static Vector3 GetWorldPositionFromRelative(Transform from, Vector3 relativePos)
            {
                return from.TransformPoint(relativePos);
            }
            public static Vector3 GetRelativePosition(Transform from, Transform to)
            {
                return from.InverseTransformPoint(to.position);
            }
        }
        public class Plane
        {
            public static bool IsVectorAlignedWithVector(Vector2 vectorA, Vector2 vectorB, float tolerance)
            {
                float angle = Vector2.Angle(vectorA.normalized, vectorB.normalized);
                if (angle <= tolerance) return true;

                return false;
            }
        }
        public class Angle
        {

        }
        public class Point
        {

        }
    }
}