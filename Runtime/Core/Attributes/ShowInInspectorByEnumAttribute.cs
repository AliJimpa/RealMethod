using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class ShowInInspectorByEnumAttribute : PropertyAttribute
{
    public string EnumFieldName { get; }
    public object[] ShowValues { get; }

    public ShowInInspectorByEnumAttribute(string enumFieldName, params object[] showValues)
    {
        EnumFieldName = enumFieldName;
        ShowValues = showValues;
    }
}


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ShowInInspectorByEnumAttribute))]
public class ShowInInspectorByEnumDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowInInspectorByEnumAttribute showInInspector = (ShowInInspectorByEnumAttribute)attribute;
        SerializedProperty enumField = property.serializedObject.FindProperty(showInInspector.EnumFieldName);

        if (enumField != null && IsVisible(enumField, showInInspector.ShowValues))
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowInInspectorByEnumAttribute showInInspector = (ShowInInspectorByEnumAttribute)attribute;
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
#endif




////Use
// public enum ExampleEnum
// {
//     Option1,
//     Option2,
//     Option3
// }
// public ExampleEnum exampleEnum;

// [ShowInInspectorByEnum("exampleEnum", ExampleEnum.Option1, ExampleEnum.Option3)]
// public string visibleOnlyForOption1And3;

// [ShowInInspectorByEnum("exampleEnum", ExampleEnum.Option2)]
// public int visibleOnlyForOption2;