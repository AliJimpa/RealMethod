using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class DropdownDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DropdownAttribute dropdownAttribute = (DropdownAttribute)attribute;

            if (property.propertyType == SerializedPropertyType.String)
            {
                int index = Mathf.Max(0, System.Array.IndexOf(dropdownAttribute.options, property.stringValue));
                index = EditorGUI.Popup(position, label.text, index, dropdownAttribute.options);
                property.stringValue = dropdownAttribute.options[index];
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                int index = Mathf.Clamp(property.intValue, 0, dropdownAttribute.options.Length - 1);
                index = EditorGUI.Popup(position, label.text, index, dropdownAttribute.options);
                property.intValue = index;
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use Dropdown with string or int.");
            }
        }
    }
}