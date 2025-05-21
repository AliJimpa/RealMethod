using UnityEditor;
using UnityEngine;

namespace RealMethod
{
    public class InspectorGuideSection : ProjectSettingSection
    {
        private SerializedObject projectSettings;


        protected override void Initialized()
        {
        }
        protected override void FirstSelected(ProjectSettingAsset Storage)
        {
            projectSettings = new SerializedObject(Storage);
        }
        protected override void Draw()
        {
            if (projectSettings == null) return;
            EditorGUILayout.PropertyField(projectSettings.FindProperty("ShowHideAbblity"), new GUIContent("Show Inside Abblity"));
            projectSettings.ApplyModifiedProperties();
        }
        protected override void Fix(int Id)
        {
        }
        protected override string GetTitle()
        {
            return "Inspector";
        }


    }
}