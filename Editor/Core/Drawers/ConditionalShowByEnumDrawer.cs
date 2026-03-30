using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalShowByEnumAttribute))]
    public class ConditionalShowByEnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ConditionalShowByEnumAttribute showInInspector = (ConditionalShowByEnumAttribute)attribute;
            SerializedProperty enumField = property.serializedObject.FindProperty(showInInspector.EnumFieldName);

            if (enumField != null && IsVisible(enumField, showInInspector.ShowValues))
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ConditionalShowByEnumAttribute showInInspector = (ConditionalShowByEnumAttribute)attribute;
            SerializedProperty enumField = property.serializedObject.FindProperty(showInInspector.EnumFieldName);

            if (enumField != null && IsVisible(enumField, showInInspector.ShowValues))
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            return 0; // Hides the property
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