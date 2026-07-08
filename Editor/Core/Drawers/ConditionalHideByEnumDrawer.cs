using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalHideByEnumAttribute))]
    public class ConditionalHideByEnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (ConditionalHideByEnumAttribute)attribute;

            // Build sibling path: replace "UseAsset" with "Mode", etc.
            string enumPath = property.propertyPath.Replace(property.name, attr.EnumFieldName);
            SerializedProperty enumField = property.serializedObject.FindProperty(enumPath);

            if (enumField != null && !ShouldHide(enumField, attr.HideValues))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attr = (ConditionalHideByEnumAttribute)attribute;

            // Must use the SAME path logic as in OnGUI
            string enumPath = property.propertyPath.Replace(property.name, attr.EnumFieldName);
            SerializedProperty enumField = property.serializedObject.FindProperty(enumPath);

            if (enumField != null && !ShouldHide(enumField, attr.HideValues))
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            // fully hide line
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        private bool ShouldHide(SerializedProperty enumField, object[] hideValues)
        {
            if (enumField.propertyType == SerializedPropertyType.Enum)
            {
                foreach (var value in hideValues)
                {
                    if (enumField.enumValueIndex == (int)value)
                        return true; // hide when enum matches any of these
                }
            }

            return false;
        }
    }
}
