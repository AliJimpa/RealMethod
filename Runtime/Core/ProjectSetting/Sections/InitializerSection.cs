using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RealMethod
{
    public class InitializerSection : ProjectSettingSection
    {
        private SerializedObject settings;
        private List<Type> componentTypes;
        private string[] typeNames;
        private int SelctedIndex = 0;
        private int NewIndex;

        protected override void Initialized()
        {
            // Get all available MonoBehaviour types **only once**
            componentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(Game).IsAssignableFrom(type) && !type.IsAbstract)
                .ToList();

            typeNames = componentTypes.Select(t => t.FullName).ToArray();
        }
        protected override void FirstSelected(ProjectSettingAsset Storage)
        {
            settings = new SerializedObject(Storage);
        }
        protected override void Draw()
        {
            if (settings == null) return;

            // GameInstanceClass
            SelctedIndex = System.Array.IndexOf(typeNames, settings.FindProperty("GameClass").stringValue);
            NewIndex = EditorGUILayout.Popup("GameInstanceClass", SelctedIndex, typeNames);
            if (NewIndex >= 0 && NewIndex < typeNames.Length)
            {
                settings.FindProperty("GameClass").stringValue = typeNames[NewIndex];
            }

            // GameSettingAsset
            EditorGUILayout.PropertyField(settings.FindProperty("GameSetting"), new GUIContent("GameSettingAsset"));

            //GameInitialPrefabs
            EditorGUILayout.PropertyField(settings.FindProperty("GamePrefab_1"), new GUIContent("GameInitialPrefab 1"));
            EditorGUILayout.PropertyField(settings.FindProperty("GamePrefab_2"), new GUIContent("GameInitialPrefab 2"));
            EditorGUILayout.PropertyField(settings.FindProperty("GamePrefab_3"), new GUIContent("GameInitialPrefab 3"));

            settings.ApplyModifiedProperties();
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