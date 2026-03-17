using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(Name16))]
    public class Name16_Drawer : PropertyDrawer
    {
        const int MaxLength = 16;
        static readonly Color Tint = new Color(1f, 0.82f, 0.9f); // very light pink


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var aProp = property.FindPropertyRelative("Name_A");
            var bProp = property.FindPropertyRelative("Name_B");

            Name16 value = new Name16();
            value.Name_A = (ulong)aProp.longValue;
            value.Name_B = (ulong)bProp.longValue;

            string text = value.ToString();

            EditorGUI.BeginProperty(position, label, property);

            Color oldColor = GUI.color;
            GUI.color = Tint;   // apply pink tint

            EditorGUI.BeginChangeCheck();
            string newText = EditorGUI.TextField(position, label, text);

            GUI.color = oldColor; // restore color

            if (EditorGUI.EndChangeCheck())
            {
                if (newText.Length > MaxLength)
                    newText = newText.Substring(0, MaxLength);

                Name16 newValue = new Name16(newText);

                aProp.longValue = (long)newValue.Name_A;
                bProp.longValue = (long)newValue.Name_B;
            }

            EditorGUI.EndProperty();
        }
    }

}