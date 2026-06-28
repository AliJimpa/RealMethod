
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

            EditorGUILayout.LabelField("Log", EditorStyles.boldLabel); // Section Title
            EditorGUILayout.BeginHorizontal();
            SettingAsset.Log_Duration = EditorGUILayout.FloatField("Duration", SettingAsset.Log_Duration);
            SettingAsset.Log_Color = EditorGUILayout.ColorField("Color", SettingAsset.Log_Color);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Warning", EditorStyles.boldLabel); // Section Title
            EditorGUILayout.BeginHorizontal();
            SettingAsset.Warning_Duration = EditorGUILayout.FloatField("Duration", SettingAsset.Warning_Duration);
            SettingAsset.Warning_Color = EditorGUILayout.ColorField("Color", SettingAsset.Warning_Color);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Error", EditorStyles.boldLabel); // Section Title
            EditorGUILayout.BeginHorizontal();
            SettingAsset.Error_Duration = EditorGUILayout.FloatField("Duration", SettingAsset.Error_Duration);
            SettingAsset.Error_Color = EditorGUILayout.ColorField("Color", SettingAsset.Error_Color);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Assert", EditorStyles.boldLabel); // Section Title
            EditorGUILayout.BeginHorizontal();
            SettingAsset.Assert_Duration = EditorGUILayout.FloatField("Duration", SettingAsset.Assert_Duration);
            SettingAsset.Assert_Color = EditorGUILayout.ColorField("Color", SettingAsset.Assert_Color);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Exception", EditorStyles.boldLabel); // Section Title
            EditorGUILayout.BeginHorizontal();
            SettingAsset.Exception_Duration = EditorGUILayout.FloatField("Duration", SettingAsset.Exception_Duration);
            SettingAsset.Exception_Color = EditorGUILayout.ColorField("Color", SettingAsset.Exception_Color);
            EditorGUILayout.EndHorizontal();


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