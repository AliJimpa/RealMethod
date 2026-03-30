using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(TypeSelectorAttribute))]
    public class TypeSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TypeSelectorAttribute typeSelector = (TypeSelectorAttribute)attribute;


            if (property.propertyType == SerializedPropertyType.String)
            {
                // Get all types derived from the specified base type
                Type[] types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(t => typeSelector.BaseType.IsAssignableFrom(t) && !t.IsAbstract)
                    .ToArray();

                // Get the current type
                string ClassName = property.stringValue;
                Type currentType = !string.IsNullOrEmpty(ClassName) ? Type.GetType(ClassName) : null;

                // Get type names for the dropdown
                string[] typeNames = types.Select(t => t.FullName).ToArray();
                int currentIndex = Array.IndexOf(typeNames, currentType?.FullName);

                // Show the dropdown in the Inspector
                int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, typeNames);

                // Update the selected type
                if (selectedIndex >= 0 && selectedIndex < types.Length)
                {
                    property.stringValue = types[selectedIndex].AssemblyQualifiedName;
                }
                else
                {
                    property.stringValue = string.Empty;
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use TypeSelector with string.");
            }
        }
    }
}