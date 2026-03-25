using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(SettingEnum))]
    public class SettingEnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative("Value");

            var ProjectSettings = AssetDatabase.LoadAssetAtPath<ProjectSettingAsset>("Assets/Resources/RealMethod/RealMethodSetting.asset");
            string[] names = ProjectSettings.GetStatus();

            int index = valueProp.intValue;

            if (index >= names.Length)
                index = 0;

            int newIndex = EditorGUI.Popup(position, label.text, index, names);

            valueProp.intValue = newIndex;
        }
    }
}