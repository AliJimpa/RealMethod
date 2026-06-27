
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class SpectatorSection : ProjectSettingSection
    {
        private ProjectSettingAsset SettingAsset;
        private SerializedObject projectSettings;
        protected override void Initialized()
        {

        }
        protected override SectionType GetSectionType()
        {
            return SectionType.Runtime;
        }
        protected override string GetTitle()
        {
            return "Spectator";
        }
        protected override void BeginRender(ProjectSettingAsset Storage)
        {
            SettingAsset = Storage;
            projectSettings = new SerializedObject(Storage);
        }
        protected override void UpdateRender()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(projectSettings.FindProperty("SpectatorPrefab"), new GUIContent("SpectatorPrefab"));

            if (EditorGUI.EndChangeCheck())
            {
                if (projectSettings != null)
                {
                    Undo.RecordObject(SettingAsset, "Modify Game Status Names");
                    EditorUtility.SetDirty(SettingAsset);
                }
            }

            if (GUI.changed)
            {
                projectSettings.ApplyModifiedProperties();
                EditorUtility.SetDirty(SettingAsset); // Mark ScriptableObject dirty
                AssetDatabase.SaveAssets();     // Optional: saves to disk immediately
                AssetDatabase.Refresh();
            }

            projectSettings.ApplyModifiedProperties();
        }

        protected override void Fix(int Id)
        {
            ClearError();
        }

    }
}