using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(SettingEnum))]
    public class SettingEnumDrawer : PropertyDrawer
    {
        private SerializedProperty MyParam = null;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (MyParam == null)
            {
                MyParam = property.FindPropertyRelative("Value");
            }
            int index = MyParam.intValue;
            if (RM_Editor.TryGetSettingAsset(out ProjectSettingAsset Setting))
            {
                string[] names = Setting.Status.ToArray();
                if (index < names.Length && index >= 0)
                {
                    int newIndex = EditorGUI.Popup(position, label.text, index, names);
                    MyParam.intValue = newIndex;
                }
                else
                {
                    Color prevColor = GUI.color;
                    GUI.color = Color.red; // light blue
                    EditorGUI.LabelField(position, $"{label.text}                   [{index}]: Not Define!");
                    GUI.color = prevColor;
                }
            }
            else
            {
                Color prevColor = GUI.color;
                GUI.color = Color.yellow; // light blue
                EditorGUI.LabelField(position, $"{label.text}                   [{index}]: Can't load RealMethod ProjectSettingAsset.");
                GUI.color = prevColor;
            }
        }
    }
}