using System;
using System.Reflection;
using UnityEditor;

namespace RealMethod.Editor
{
    /// <summary>
    /// Required for the fetching of a default editor on ScriptableObject objects.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GameConfig), true)]
    public class GameConfigEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            int totalSize = CalculateObjectSize(target);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Memory Estimate", $"{totalSize} bytes");
            EditorGUILayout.HelpBox("Reference types (string, classes, arrays, etc.) ,only count pointer size (8 bytes on 64-bit)", MessageType.Info);
        }

        private int CalculateObjectSize(object obj)
        {
            int total = 0;
            var fields = obj.GetType().GetFields(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            foreach (var field in fields)
            {
                total += GetFieldSize(field.FieldType);
            }

            return total;
        }

        private int GetFieldSize(Type type)
        {
            if (type == typeof(int)) return 4;
            if (type == typeof(float)) return 4;
            if (type == typeof(bool)) return 1;
            if (type == typeof(double)) return 8;
            if (type == typeof(long)) return 8;
            if (type == typeof(short)) return 2;
            if (type == typeof(byte)) return 1;
            if (type == typeof(char)) return 2;

            // Reference types (string, classes, arrays, etc.)
            // only count pointer size (8 bytes on 64-bit)
            if (!type.IsValueType) return 8;

            return 0;
        }
    }
}