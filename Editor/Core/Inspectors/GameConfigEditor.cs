using System;
using System.Collections;
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
            EditorGUILayout.HelpBox($"Memory Estimate: {totalSize} bytes", MessageType.Info);
        }

        private int CalculateObjectSize(object obj)
        {
            int total = 0;
            var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                object value = field.GetValue(obj);
                total += GetFieldSize(field.FieldType, value);
            }

            return total;
        }

        private int GetFieldSize(Type type, object value)
        {
            if (type == typeof(int)) return 4;
            if (type == typeof(float)) return 4;
            if (type == typeof(bool)) return 1;
            if (type == typeof(double)) return 8;
            if (type == typeof(long)) return 8;
            if (type == typeof(short)) return 2;
            if (type == typeof(byte)) return 1;
            if (type == typeof(char)) return 2;

            // String (UTF-16 in C#)
            if (type == typeof(string))
            {
                string str = (string)value;
                return str.Length * 2;
            }

            // Arrays
            if (type.IsArray)
            {
                int total = 0;
                Array array = (Array)value;

                foreach (var item in array)
                    total += GetFieldSize(item.GetType(), item);

                return total;
            }

            // Lists
            if (typeof(IList).IsAssignableFrom(type))
            {
                int total = 0;
                IList list = (IList)value;

                foreach (var item in list)
                    total += GetFieldSize(item.GetType(), item);

                return total;
            }

            // Unity Object references → count reference only
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                return IntPtr.Size;
            }

            // Custom classes/structs (recursive)
            int objectSize = 0;
            var Nfields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var Nfield in Nfields)
            {
                object Nvalue = Nfield.GetValue(value);
                objectSize += GetFieldSize(Nfield.FieldType, Nvalue);
            }
            return objectSize;

            // // Reference types (string, classes, arrays, etc.)
            // // only count pointer size (8 bytes on 64-bit)
            // if (!type.IsValueType) return 8;

            // return 0;
        }
    }
}