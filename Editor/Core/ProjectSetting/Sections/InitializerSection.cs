using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class InitializerSection : ProjectSettingSection
    {
        private ClassType<Game> gameClass;
        private ClassType<GameService> gameService;
        private SerializedObject projectSettings;
        private ProjectSettingAsset SettingAsset;

        protected override void Initialized()
        {
            gameClass = new ClassType<Game>();
            gameService = new ClassType<GameService>();
        }
        protected override void FirstSelected(ProjectSettingAsset Storage)
        {
            SettingAsset = Storage;
            projectSettings = new SerializedObject(Storage);
        }
        protected override void Draw()
        {
            if (projectSettings == null) return;

            projectSettings.Update();

            // GameInstanceClass
            gameClass.Draw(projectSettings, "GameClass", "Game Class");

            // GameServiceClass
            gameService.Draw(projectSettings, "GameService", "Game Service");

            // GameSettingAsset
            EditorGUILayout.PropertyField(projectSettings.FindProperty("GameConfig"), new GUIContent("Game Config"));

            //GameInitialPrefabs
            EditorGUILayout.PropertyField(projectSettings.FindProperty("GamePrefab_1"), new GUIContent("Game Prefab (1)"));
            EditorGUILayout.PropertyField(projectSettings.FindProperty("GamePrefab_2"), new GUIContent("Game Prefab (2)"));
            EditorGUILayout.PropertyField(projectSettings.FindProperty("GamePrefab_3"), new GUIContent("Game Prefab (3)"));

            if (GUI.changed)
            {
                projectSettings.ApplyModifiedProperties();
                EditorUtility.SetDirty(SettingAsset); // Mark ScriptableObject dirty
                AssetDatabase.SaveAssets();     // Optional: saves to disk immediately
                AssetDatabase.Refresh();
            }

            projectSettings.ApplyModifiedProperties();
        }
        protected override string GetTitle()
        {
            return "Initializer";
        }
        protected override void Fix(int Id)
        {
            throw new System.NotImplementedException();
        }




    }
}