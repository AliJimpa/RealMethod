using System;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(EnumDescriptionAttribute))]
    public class EnumDescriptionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Enum)
            {
                // Get the enum type and value.
                Type enumType = fieldInfo.FieldType;
                int selectedIndex = property.enumValueIndex;

                // Get the descriptions for each enum value.
                string[] descriptions = GetEnumDescriptions(enumType);

                if (descriptions != null && descriptions.Length > selectedIndex)
                {
                    // Draw the popup with descriptions.
                    property.enumValueIndex = EditorGUI.Popup(position, label.text, selectedIndex, descriptions);
                }
                else
                {
                    // Fallback to default behavior.
                    EditorGUI.PropertyField(position, property, label);
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        private string[] GetEnumDescriptions(Type enumType)
        {
            var names = Enum.GetNames(enumType);
            string[] descriptions = new string[names.Length];

            for (int i = 0; i < names.Length; i++)
            {
                var field = enumType.GetField(names[i]);
                var descriptionAttribute = Attribute.GetCustomAttribute(field, typeof(EnumDescriptionAttribute)) as EnumDescriptionAttribute;
                descriptions[i] = descriptionAttribute != null ? descriptionAttribute.Description : names[i];
            }

            return descriptions;
        }
    }
}