
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class FreeCameraSection : ProjectSettingSection
    {
        private ProjectSettingAsset SettingAsset;
        private SerializedObject projectSettings;
        protected override void Initialized()
        {

        }
        protected override SectionType GetSectionType()
        {
            return SectionType.Development;
        }
        protected override string GetTitle()
        {
            return "FreeCamera";
        }
        protected override void BeginRender(ProjectSettingAsset Storage)
        {
            SettingAsset = Storage;
            projectSettings = new SerializedObject(Storage);
        }
        protected override void UpdateRender()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(projectSettings.FindProperty("FreeCameraPrefab"), new GUIContent("FreeCameraPrefab"));

            SettingAsset.FreeCameraMoveSpeed = EditorGUILayout.FloatField("FreeCameraMoveSpeed", SettingAsset.FreeCameraMoveSpeed);
            SettingAsset.FreeCameraSprintSpeed = EditorGUILayout.FloatField("FreeCameraSprintSpeed", SettingAsset.FreeCameraSprintSpeed);
            SettingAsset.FreeCameraLookSpeed = EditorGUILayout.FloatField("FreeCameraMoveSpeed", SettingAsset.FreeCameraLookSpeed);

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