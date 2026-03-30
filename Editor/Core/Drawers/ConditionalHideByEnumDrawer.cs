using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalHideByEnumAttribute))]
    public class ConditionalHideByEnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ConditionalHideByEnumAttribute hideAttribute = (ConditionalHideByEnumAttribute)attribute;
            SerializedProperty enumField = property.serializedObject.FindProperty(hideAttribute.EnumFieldName);

            if (enumField != null && enumField.enumValueIndex == hideAttribute.EnumValue)
            {
                return; // Do not draw the property if the condition matches.
            }

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ConditionalHideByEnumAttribute hideAttribute = (ConditionalHideByEnumAttribute)attribute;
            SerializedProperty enumField = property.serializedObject.FindProperty(hideAttribute.EnumFieldName);

            if (enumField != null && enumField.enumValueIndex == hideAttribute.EnumValue)
            {
                return 0; // Hides the property by returning 0 height.
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}