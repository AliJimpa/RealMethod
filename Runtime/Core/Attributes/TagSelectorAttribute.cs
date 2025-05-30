using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RealMethod
{
    public class TagSelectorAttribute : PropertyAttribute
    {
        public bool UseDefaultTagFieldDrawer = false;
    }


#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            TagSelectorAttribute tagSelector = (TagSelectorAttribute)attribute;

            if (tagSelector.UseDefaultTagFieldDrawer)
            {
                EditorGUI.PropertyField(position, property, label);
            }
            else
            {
                if (property.propertyType == SerializedPropertyType.String)
                {
                    property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
                }
                else
                {
                    EditorGUI.PropertyField(position, property, label);
                    EditorGUI.HelpBox(position, "TagSelector can only be used with strings.", MessageType.Error);
                }
            }
        }
    }
#endif
}

//Use
// [TagSelector]
