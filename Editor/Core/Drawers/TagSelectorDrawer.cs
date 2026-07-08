using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorDrawer : PropertyDrawer
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
}