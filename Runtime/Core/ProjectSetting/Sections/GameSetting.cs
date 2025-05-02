using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealMethod
{
    public class GameSetting : SettingSection
    {
        private const string settingsPath = "Assets/Resources/RealMethod/RealMethodSetting.asset"; // Store in Resources for runtime access
        private SerializedObject settings;
        private List<Type> componentTypes;
        private string[] typeNames;



        // Implement Abstaction Methods
        protected override void Initialized()
        {
            // Get all available MonoBehaviour types **only once**
            componentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(Game).IsAssignableFrom(type) && !type.IsAbstract)
                .ToList();

            typeNames = componentTypes.Select(t => t.FullName).ToArray();
        }
        protected override void FirstSelected(RealMethodSettingAsset Storage)
        {
            settings = new SerializedObject(Storage);
        }
        protected override void Draw()
        {
            if (settings == null) return;

            EditorGUILayout.PropertyField(settings.FindProperty("GeneralGameData"), new GUIContent("GameData"));

            int selectedIndex = Array.IndexOf(typeNames, settings.FindProperty("GameClass").stringValue);
            int newIndex = EditorGUILayout.Popup("GameClass", selectedIndex, typeNames);

            if (newIndex >= 0 && newIndex < typeNames.Length)
            {
                settings.FindProperty("GameClass").stringValue = typeNames[newIndex];
            }

            settings.ApplyModifiedProperties();
        }
        protected override string GetTitle()
        {
            return "Base";
        }
        protected override void Fix(int Id)
        {

        }


    }
}
