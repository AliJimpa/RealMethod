using UnityEngine;

namespace RealMethod
{
    public static class RM_Draw
    {
        public class gizmos
        {
            public static void DrawCurve(AnimationCurve curve)
            {
                for (float t = 0; t < curve.keys[curve.length - 1].time; t += 0.1f)
                {
                    Gizmos.DrawSphere(new Vector3(t, curve.Evaluate(t), 0), 0.1f);
                }
            }
            // Draws a directional arrow using Gizmos (no color)
            public static void DrawArrow(Vector3 position, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Gizmos.DrawRay(position, direction);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
                Gizmos.DrawRay(position + direction, right * arrowHeadLength);
                Gizmos.DrawRay(position + direction, left * arrowHeadLength);
            }
            // Draws a directional arrow using Gizmos (with color)
            public static void DrawArrow(Vector3 position, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Gizmos.color = color;
                Gizmos.DrawRay(position, direction);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
                Gizmos.DrawRay(position + direction, right * arrowHeadLength);
                Gizmos.DrawRay(position + direction, left * arrowHeadLength);
            }
        }

        public class debug
        {
            // Draws a directional arrow using Debug.DrawRay (no color)
            public static void DrawArrow(Vector3 position, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Debug.DrawRay(position, direction);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
                Debug.DrawRay(position + direction, right * arrowHeadLength);
                Debug.DrawRay(position + direction, left * arrowHeadLength);
            }
            // Draws a directional arrow using Debug.DrawRay (with color)
            public static void DrawArrow(Vector3 position, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Debug.DrawRay(position, direction, color);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
                Debug.DrawRay(position + direction, right * arrowHeadLength, color);
                Debug.DrawRay(position + direction, left * arrowHeadLength, color);
            }
        }

    }
}