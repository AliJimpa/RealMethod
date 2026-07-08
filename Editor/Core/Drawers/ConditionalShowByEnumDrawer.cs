using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalShowByEnumAttribute))]
    public class ConditionalShowByEnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (ConditionalShowByEnumAttribute)attribute;

            // Build sibling path: replace "UseAsset" with "Mode", etc.
            string enumPath = property.propertyPath.Replace(property.name, attr.EnumFieldName);
            SerializedProperty enumField = property.serializedObject.FindProperty(enumPath);

            if (enumField != null && IsVisible(enumField, attr.ShowValues))
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attr = (ConditionalShowByEnumAttribute)attribute;

            // Must use the SAME path logic as in OnGUI
            string enumPath = property.propertyPath.Replace(property.name, attr.EnumFieldName);
            SerializedProperty enumField = property.serializedObject.FindProperty(enumPath);

            if (enumField != null && IsVisible(enumField, attr.ShowValues))
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            // fully hide line
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        private bool IsVisible(SerializedProperty enumField, object[] showValues)
        {
            foreach (var value in showValues)
            {
                if (enumField.propertyType == SerializedPropertyType.Enum && enumField.enumValueIndex == (int)value)
                {
                    return true;
                }
            }

            return false;
        }
    }
}