using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(SoftType), true)]
    public class SoftTypeDrawer : PropertyDrawer
    {
        private string[] displayNames;
        private string[] typeNames;
        private int currentIndex;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typeProp = property.FindPropertyRelative("typeName");

            if (displayNames == null)
            {
                Initialize(typeProp);
                typeProp.stringValue = typeNames[currentIndex];
            }
                

            int newIndex = EditorGUI.Popup(position, label.text, currentIndex, displayNames);

            if (newIndex != currentIndex)
            {
                currentIndex = newIndex;
                typeProp.stringValue = typeNames[newIndex];
            }
        }

        private void Initialize(SerializedProperty property)
        {
            var baseType = fieldInfo.FieldType.GetGenericArguments()[0];

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract)
                .OrderBy(t => t.Name)
                .ToArray();

            displayNames = types.Select(t => t.Name).ToArray();
            typeNames = types.Select(t => t.AssemblyQualifiedName).ToArray();

            currentIndex = Array.IndexOf(typeNames, property.stringValue);
            if (currentIndex < 0) currentIndex = 0;
        }
    }

}