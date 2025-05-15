using UnityEngine;
using UnityEditor;

namespace RealMethod
{
    public enum HelpBoxMessageType { None, Info, Warning, Error }

    public class HelpBoxAttribute : PropertyAttribute
    {

        public string text;
        public HelpBoxMessageType messageType;
        public int height;

        public HelpBoxAttribute(string text, HelpBoxMessageType messageType = HelpBoxMessageType.None, int Boxheight = 2)
        {
            this.text = text;
            this.messageType = messageType;
            height = Boxheight;
        }
    }

#if UNITY_EDITOR

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

#endif


}
// ----Use
// [HelpBox("This is the first line of the help box.\nThis is the second line of the help box.", HelpBoxMessageType.Info)]
// [TextArea(3, 10)]
// public string myMultiLineText;
