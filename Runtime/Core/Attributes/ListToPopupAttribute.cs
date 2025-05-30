using System;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RealMethod
{
    public class ListToPopupAttribute : PropertyAttribute
    {
        public Type myType;
        public string propertyName;

        public ListToPopupAttribute(Type _myType, string _propertyName)
        {
            myType = _myType;
            propertyName = _propertyName;
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ListToPopupAttribute))]
    public class ListToPopupDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ListToPopupAttribute atb = attribute as ListToPopupAttribute;
            List<string> stringList = null;

            // Get the field using reflection
            FieldInfo field = atb.myType.GetField(atb.propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
            {
                stringList = field.GetValue(null) as List<string>;
            }

            if (stringList != null && stringList.Count != 0)
            {
                // Determine the current selected index
                int currentIndex = stringList.IndexOf(property.stringValue);
                if (currentIndex == -1)
                {
                    currentIndex = 0; // default to the first item if the value is not found
                }

                // Display the popup and update the property value
                int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, stringList.ToArray());
                property.stringValue = stringList[selectedIndex];
            }
            else
            {
                // If the list is null or empty, fallback to the default property field
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }

#endif
}
// ------------Use
// [ListToPopup(typeof(ExampleClass), "options")]
// public string selectedOption;
// public static List<string> options = new List<string> { "Option1", "Option2", "Option3" };
