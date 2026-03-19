using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class InitializerSetting_Section : ProjectSettingSection
    {
        private TypeSelector<Game> gameClass;
        private TypeSelector<GameBridge> gameBridge;
        private SerializedObject projectSettings;
        private ProjectSettingAsset SettingAsset;

        protected override string GetTitle()
        {
            return "Initializer";
        }
        protected override SectionType GetSectionType()
        {
            return SectionType.Runtime;
        }
        protected override void Initialized()
        {

        }
        protected override void BeginRender(ProjectSettingAsset Storage)
        {
            SettingAsset = Storage;
            projectSettings = new SerializedObject(Storage);
            gameClass = new TypeSelector<Game>(projectSettings.FindProperty("GameClass"), "Game Class");
            gameBridge = new TypeSelector<GameBridge>(projectSettings.FindProperty("GameBridge"), "Game Bridge");
        }
        protected override void UpdateRender()
        {
            if (projectSettings == null) return;

            projectSettings.Update();

            // GameInstanceClass
            gameClass.Draw();
            // GameBridgeClass
            gameBridge.Draw();
            // GameSettingAsset
            EditorGUILayout.PropertyField(projectSettings.FindProperty("GameConfig"), new GUIContent("Game Config"));
            //GameInitialPrefabs
            EditorGUILayout.PropertyField(projectSettings.FindProperty("GamePrefab_1"), new GUIContent("GameScope (Runtime)"));
            EditorGUILayout.PropertyField(projectSettings.FindProperty("GamePrefab_2"), new GUIContent("GameScope (Editor)"));
            EditorGUILayout.PropertyField(projectSettings.FindProperty("GamePrefab_3"), new GUIContent("Dedicated Server"));

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
            throw new System.NotImplementedException();
        }


    }
}