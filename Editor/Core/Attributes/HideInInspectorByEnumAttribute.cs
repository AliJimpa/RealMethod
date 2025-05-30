using UnityEditor;
using UnityEngine;


namespace RealMethod
{
    public class HideInInspectorByEnumAttribute : PropertyAttribute
    {
        public string EnumFieldName { get; private set; }
        public int EnumValue { get; private set; }

        public HideInInspectorByEnumAttribute(string enumFieldName, int enumValue)
        {
            EnumFieldName = enumFieldName;
            EnumValue = enumValue;
        }
    }


#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(HideInInspectorByEnumAttribute))]
    public class HideInInspectorByEnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HideInInspectorByEnumAttribute hideAttribute = (HideInInspectorByEnumAttribute)attribute;
            SerializedProperty enumField = property.serializedObject.FindProperty(hideAttribute.EnumFieldName);

            if (enumField != null && enumField.enumValueIndex == hideAttribute.EnumValue)
            {
                return; // Do not draw the property if the condition matches.
            }

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            HideInInspectorByEnumAttribute hideAttribute = (HideInInspectorByEnumAttribute)attribute;
            SerializedProperty enumField = property.serializedObject.FindProperty(hideAttribute.EnumFieldName);

            if (enumField != null && enumField.enumValueIndex == hideAttribute.EnumValue)
            {
                return 0; // Hides the property by returning 0 height.
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
#endif
}


///////////Use
// public enum DisplayMode
//     {
//         Mode1,
//         Mode2,
//         Mode3
//     }

//     [SerializeField] private DisplayMode displayMode;

//     [HideInInspectorByEnum("displayMode", 0)] // Hide when DisplayMode is Mode1
//     public int mode2SpecificValue;
