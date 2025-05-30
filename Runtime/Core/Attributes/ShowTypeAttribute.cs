
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Linq;



namespace RealMethod
{
    [Serializable]
    public class TypeReference
    {
        public string typeName;

        public Type Type
        {
            get { return string.IsNullOrEmpty(typeName) ? null : Type.GetType(typeName); }
            set { typeName = value == null ? string.Empty : value.AssemblyQualifiedName; }
        }

        public TypeReference() { }

        public TypeReference(Type type)
        {
            Type = type;
        }
    }

    public class TypeSelectorAttribute : PropertyAttribute
    {
        public Type BaseType { get; private set; }

        public TypeSelectorAttribute(Type baseType = null)
        {
            BaseType = baseType ?? typeof(object);
        }
    }


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(TypeSelectorAttribute))]
    public class TypeReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TypeSelectorAttribute typeSelector = (TypeSelectorAttribute)attribute;

            // Find the type reference field
            SerializedProperty typeNameProperty = property.FindPropertyRelative("typeName");

            // Get all types derived from the specified base type
            Type[] types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => typeSelector.BaseType.IsAssignableFrom(t) && !t.IsAbstract)
                .ToArray();

            // Get the current type
            string currentTypeName = typeNameProperty.stringValue;
            Type currentType = !string.IsNullOrEmpty(currentTypeName) ? Type.GetType(currentTypeName) : null;

            // Get type names for the dropdown
            string[] typeNames = types.Select(t => t.FullName).ToArray();
            int currentIndex = System.Array.IndexOf(typeNames, currentType?.FullName);

            // Show the dropdown in the Inspector
            int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, typeNames);

            // Update the selected type
            if (selectedIndex >= 0 && selectedIndex < types.Length)
            {
                typeNameProperty.stringValue = types[selectedIndex].AssemblyQualifiedName;
            }
            else
            {
                typeNameProperty.stringValue = string.Empty;
            }
        }
    }
#endif

}

//Use 
//[TypeSelector(typeof(MonoBehaviour))] // You can specify the base type here
//public TypeReference selectedType;