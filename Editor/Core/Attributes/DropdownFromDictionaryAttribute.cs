using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RealMethod
{
    public class DropdownFromDictionaryAttribute : PropertyAttribute
    {
        public string dictionaryFieldName;

        public DropdownFromDictionaryAttribute(string dictionaryFieldName)
        {
            this.dictionaryFieldName = dictionaryFieldName;
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(DropdownFromDictionaryAttribute))]
    public class DropdownFromDictionaryDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DropdownFromDictionaryAttribute dropdownAttribute = (DropdownFromDictionaryAttribute)attribute;
            string[] options = GetOptions(property, dropdownAttribute.dictionaryFieldName);

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
                    EditorGUI.LabelField(position, label.text, "Use DropdownFromDictionary with string.");
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Dictionary not found or empty.");
            }
        }

        private string[] GetOptions(SerializedProperty property, string dictionaryFieldName)
        {
            string[] options = null;
            Object targetObject = property.serializedObject.targetObject;
            System.Type targetType = targetObject.GetType();
            System.Reflection.FieldInfo fieldInfo = targetType.GetField(dictionaryFieldName);

            if (fieldInfo != null)
            {
                var dictionary = fieldInfo.GetValue(targetObject) as Dictionary<string, string>;
                if (dictionary != null)
                {
                    options = new string[dictionary.Count];
                    dictionary.Keys.CopyTo(options, 0);
                }
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

// ----Use
// public Dictionary<string, string> options = new Dictionary<string, string>
//     {
//         { "Key1", "Value1" },
//         { "Key2", "Value2" },
//         { "Key3", "Value3" }
//     };

// [DropdownFromDictionary("options")]
// public string selectedOption;
