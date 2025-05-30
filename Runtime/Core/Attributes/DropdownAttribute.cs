#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RealMethod
{

    public class DropdownAttribute : PropertyAttribute
    {
        public string[] options;

        public DropdownAttribute(params string[] options)
        {
            this.options = options;
        }
    }

#if UNITY_EDITOR

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

#endif
}

// ----Use
// [Dropdown("Option1", "Option2", "Option3")]
// public string selectedOption;

// [Dropdown("Option1", "Option2", "Option3")]
// public int selectedOptionIndex;
