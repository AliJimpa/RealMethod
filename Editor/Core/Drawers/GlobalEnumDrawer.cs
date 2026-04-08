using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(GlobalEnum))]
    public class GlobalEnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProp = property.FindPropertyRelative("Value");
            int index = valueProp.intValue;

            if (RM_Editor.TryGetSettingAsset(out ProjectSettingAsset setting))
            {
                string[] names = setting.Status.ToArray();

                if (index >= 0 && index < names.Length)
                {
                    int newIndex = EditorGUI.Popup(position, label.text, index, names);
                    valueProp.intValue = newIndex;
                }
                else
                {
                    Color prev = GUI.color;
                    GUI.color = Color.red;
                    EditorGUI.LabelField(position, $"{label.text} [{index}]: Not Defined!");
                    GUI.color = prev;
                }
            }
            else
            {
                Color prev = GUI.color;
                GUI.color = Color.yellow;
                EditorGUI.LabelField(position, $"{label.text} [{index}]: Can't load ProjectSettingAsset.");
                GUI.color = prev;
            }
        }
    }
}