
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class DebugSection : ProjectSettingSection
    {
        private ProjectSettingAsset SettingAsset;
        private SerializedObject projectSettings;

        protected override void Initialized()
        {
        }
        protected override string GetTitle()
        {
            return "Debug";
        }
        protected override SectionType GetSectionType()
        {
            return SectionType.Editor;
        }
        protected override void BeginRender(ProjectSettingAsset Storage)
        {
            SettingAsset = Storage;
            projectSettings = new SerializedObject(Storage);
        }
        protected override void UpdateRender()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(20);
            if (GUILayout.Button("Reset", GUILayout.Width(80)))
            {
                SettingAsset.ResetLogColor();
            }
            EditorGUILayout.EndHorizontal();
            SettingAsset.Log_Color = EditorGUILayout.ColorField("Log", SettingAsset.Log_Color);
            SettingAsset.Warning_Color = EditorGUILayout.ColorField("Warning", SettingAsset.Warning_Color);
            SettingAsset.Error_Color = EditorGUILayout.ColorField("Error", SettingAsset.Error_Color);
            SettingAsset.Assert_Color = EditorGUILayout.ColorField("Assert", SettingAsset.Assert_Color);
            SettingAsset.Exception_Color = EditorGUILayout.ColorField("Exception", SettingAsset.Exception_Color);

            if (EditorGUI.EndChangeCheck())
            {
                if (projectSettings != null)
                {
                    //Undo.RecordObject(SettingAsset, "Modify Log Colors");
                    //EditorUtility.SetDirty(SettingAsset);
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