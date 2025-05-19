using UnityEditor;
using UnityEngine;

namespace RealMethod
{
    public class DropdownFromArrayAttribute : PropertyAttribute
    {
        public string arrayFieldName;

        public DropdownFromArrayAttribute(string arrayFieldName)
        {
            this.arrayFieldName = arrayFieldName;
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(DropdownFromArrayAttribute))]
    public class DropdownFromArrayDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DropdownFromArrayAttribute dropdownAttribute = (DropdownFromArrayAttribute)attribute;
            string[] options = GetOptions(property, dropdownAttribute.arrayFieldName);

            if (options != null && options.Length > 0)
            {
                if (property.propertyType == SerializedPropertyType.String)
                {
                    int index = Mathf.Max(0, System.Array.IndexOf(options, property.stringValue));
                    index = EditorGUI.Popup(position, label.text, index, options);
                    property.stringValue = options[index];
                }
                else
                {
                    EditorGUI.LabelField(position, label.text, "Use DropdownFromArray with string.");
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Array not found or empty.");
            }
        }

        private string[] GetOptions(SerializedProperty property, string arrayFieldName)
        {
            string[] options = null;
            Object targetObject = property.serializedObject.targetObject;
            System.Type targetType = targetObject.GetType();
            System.Reflection.FieldInfo fieldInfo = targetType.GetField(arrayFieldName);

            if (fieldInfo != null)
            {
                options = fieldInfo.GetValue(targetObject) as string[];
            }

            return options;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }

#endif
}
// ---Use
// public string[] options = new string[] { "Option1", "Option2", "Option3" };

// [DropdownFromArray("options")]
// public string selectedOption;
