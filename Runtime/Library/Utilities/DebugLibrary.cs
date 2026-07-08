using UnityEngine;

namespace RealMethod
{
    public static class RM_Debug
    {
        // Draws a directional arrow using Debug.DrawRay (no color)
        public static void Arrow(Vector3 position, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Debug.DrawRay(position, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
            Debug.DrawRay(position + direction, right * arrowHeadLength);
            Debug.DrawRay(position + direction, left * arrowHeadLength);
        }
        // Draws a directional arrow using Debug.DrawRay (with color)
        public static void Arrow(Vector3 position, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Debug.DrawRay(position, direction, color);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
            Debug.DrawRay(position + direction, right * arrowHeadLength, color);
            Debug.DrawRay(position + direction, left * arrowHeadLength, color);
        }
    }

}