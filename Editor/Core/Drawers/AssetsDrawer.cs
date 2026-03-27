using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(PrimitiveAsset), true)]
    public class Asset_Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Color prevColor = GUI.color;
            GUI.color = new Color(1f, 0.8f, 0.8f);
            EditorGUI.PropertyField(position, property, label, true);
            GUI.color = prevColor;
        }
    }
}