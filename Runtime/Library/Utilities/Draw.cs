using UnityEditor;
using UnityEngine;

namespace RealMethod
{
    public static class RM_Draw
    {
        public class gizmos
        {
            public static void Curve(AnimationCurve curve)
            {
                for (float t = 0; t < curve.keys[curve.length - 1].time; t += 0.1f)
                {
                    Gizmos.DrawSphere(new Vector3(t, curve.Evaluate(t), 0), 0.1f);
                }
            }
            // Draws a directional arrow using Gizmos (no color)
            public static void Arrow(Vector3 position, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Gizmos.DrawRay(position, direction);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
                Gizmos.DrawRay(position + direction, right * arrowHeadLength);
                Gizmos.DrawRay(position + direction, left * arrowHeadLength);
            }
            // Draws a directional arrow using Gizmos (with color)
            public static void Arrow(Vector3 position, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
            {
                Gizmos.color = color;
                Gizmos.DrawRay(position, direction);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
                Gizmos.DrawRay(position + direction, right * arrowHeadLength);
                Gizmos.DrawRay(position + direction, left * arrowHeadLength);
            }
            // Draws a Capsule using Gizmos (no color)
            public static void Capsule(Vector3 position, float height = 2f, float radius = 0.5f)
            {
                // Draw top sphere
                Vector3 top = position + Vector3.up * (height / 2 - radius);
                Gizmos.DrawWireSphere(top, radius);

                // Draw bottom sphere
                Vector3 bottom = position + Vector3.down * (height / 2 - radius);
                Gizmos.DrawWireSphere(bottom, radius);

                // Draw cylinder (approximation)
                Gizmos.DrawLine(top + Vector3.forward * radius, bottom + Vector3.forward * radius);
                Gizmos.DrawLine(top - Vector3.forward * radius, bottom - Vector3.forward * radius);
                Gizmos.DrawLine(top + Vector3.right * radius, bottom + Vector3.right * radius);
                Gizmos.DrawLine(top - Vector3.right * radius, bottom - Vector3.right * radius);
            }
            // Draws a Capsule using Gizmos (with color)
            public static void Capsule(Vector3 position, Color color, float height = 2f, float radius = 0.5f)
            {
                // Save previous Gizmos color
                Color prevColor = Gizmos.color;
                Gizmos.color = color;

                // Draw top sphere
                Vector3 top = position + Vector3.up * (height / 2 - radius);
                Gizmos.DrawWireSphere(top, radius);

                // Draw bottom sphere
                Vector3 bottom = position + Vector3.down * (height / 2 - radius);
                Gizmos.DrawWireSphere(bottom, radius);

                // Draw cylinder (approximation)
                Gizmos.DrawLine(top + Vector3.forward * radius, bottom + Vector3.forward * radius);
                Gizmos.DrawLine(top - Vector3.forward * radius, bottom - Vector3.forward * radius);
                Gizmos.DrawLine(top + Vector3.right * radius, bottom + Vector3.right * radius);
                Gizmos.DrawLine(top - Vector3.right * radius, bottom - Vector3.right * radius);

                // Restore previous color
                Gizmos.color = prevColor;
            }
            // Draw a Text using Gizmos (With Colort)
            public static void Text(string text, Vector3 position, Color color, FontStyle font = FontStyle.Bold, TextAnchor ancher = TextAnchor.MiddleCenter)
            {
                // Set color
                GUIStyle style = new GUIStyle();
                style.normal.textColor = color;
                style.alignment = ancher;
                style.fontStyle = font;

                // Draw the label above the GameObject
                Handles.Label(position, text, style);
            }
        }

        public class debug
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
}