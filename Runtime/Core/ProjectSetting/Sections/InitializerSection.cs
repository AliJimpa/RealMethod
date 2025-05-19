using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RealMethod
{
    public class InitializerSection : ProjectSettingSection
    {
        private class ClassType<T>
        {
            private List<Type> TypeList;
            private string[] TypeName;
            private int selctedIndex = 0;
            private int newIndex;

            public ClassType()
            {
                // Get all available T types **only once**
                TypeList = AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(assembly => assembly.GetTypes())
               .Where(type => typeof(T).IsAssignableFrom(type) && !type.IsAbstract)
               .ToList();

                TypeName = TypeList.Select(t => t.FullName).ToArray();
            }

            public void Draw(SerializedObject projectSettings, string PropertyName, string DisplayName)
            {
                selctedIndex = System.Array.IndexOf(TypeName, projectSettings.FindProperty(PropertyName).stringValue);
                newIndex = EditorGUILayout.Popup(DisplayName, selctedIndex, TypeName);
                if (newIndex >= 0 && newIndex < TypeName.Length)
                {
                    projectSettings.FindProperty(PropertyName).stringValue = TypeName[newIndex];
                }
            }
        }


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
            EditorGUILayout.PropertyField(projectSettings.FindProperty("GameSetting"), new GUIContent("Game Setting"));

            //GameInitialPrefabs
            EditorGUILayout.PropertyField(projectSettings.FindProperty("GamePrefab_1"), new GUIContent("Game Prefab (1)"));
            EditorGUILayout.PropertyField(projectSettings.FindProperty("GamePrefab_2"), new GUIContent("Game Prefab (2)"));
            EditorGUILayout.PropertyField(projectSettings.FindProperty("GamePrefab_3"), new GUIContent("Game Prefab (3)"));

            if (GUI.changed)
            {
                EditorUtility.SetDirty(SettingAsset); // Mark ScriptableObject dirty
                AssetDatabase.SaveAssets();     // Optional: saves to disk immediately
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