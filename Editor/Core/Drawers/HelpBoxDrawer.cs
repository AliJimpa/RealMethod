using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            HelpBoxAttribute helpBoxAttribute = (HelpBoxAttribute)attribute;
            EditorGUI.BeginProperty(position, label, property);
            // Calculate the height of the help box
            var helpBoxHeight = EditorGUIUtility.singleLineHeight * helpBoxAttribute.height;
            var helpBoxRect = new Rect(position.x, position.y, position.width, helpBoxHeight);
            EditorGUI.HelpBox(helpBoxRect, helpBoxAttribute.text, (MessageType)helpBoxAttribute.messageType);
            // Calculate the position of the property field
            var propertyRect = new Rect(position.x, position.y + helpBoxHeight + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(propertyRect, property, label);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            HelpBoxAttribute helpBoxAttribute = (HelpBoxAttribute)attribute;
            var helpBoxHeight = EditorGUIUtility.singleLineHeight * 2;
            return helpBoxHeight + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}