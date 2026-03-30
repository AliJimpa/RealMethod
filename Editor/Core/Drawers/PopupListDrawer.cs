using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(PopupListAttribute))]
    public class PopupListDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            PopupListAttribute atb = attribute as PopupListAttribute;
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
}