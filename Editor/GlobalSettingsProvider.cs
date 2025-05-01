using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class GlobalSettingsProvider
{
    private const string settingsPath = "Assets/Resources/Mustard/GameSettingObj.asset"; // Store in Resources for runtime access
    private static SerializedObject settings;
    private static string[] typeNames;
    private static List<Type> componentTypes;



    [SettingsProvider]
    public static SettingsProvider CreateSettingsProvider()
    {
        var provider = new SettingsProvider("Project/GlobalSettings", SettingsScope.Project)
        {
            label = "Global Settings",

            // Called ONLY when the tab is first selected
            activateHandler = (searchContext, rootElement) =>
            {
                //Debug.Log("Settings Tab Selected!");
                settings = new SerializedObject(GetOrCreateSettings());

                // Get all available MonoBehaviour types **only once**
                componentTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => typeof(Game).IsAssignableFrom(type) && !type.IsAbstract)
                    .ToList();

                typeNames = componentTypes.Select(t => t.FullName).ToArray();
            },

            // Draws UI elements
            guiHandler = (searchContext) =>
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
        };

        return provider;
    }

    public static GameSettingObj GetOrCreateSettings()
    {
        GameSettingObj settings = AssetDatabase.LoadAssetAtPath<GameSettingObj>(settingsPath);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<GameSettingObj>();
            AssetDatabase.CreateAsset(settings, settingsPath);
            AssetDatabase.SaveAssets();
        }
        return settings;
    }
}



// Alternative: Use Addressables (Better for Large Projects)
// public static class GlobalSettingsLoader
// {
//     private static GlobalSettings cachedSettings;

//     public static void LoadSettings(System.Action<GlobalSettings> onComplete)
//     {
//         if (cachedSettings != null)
//         {
//             onComplete?.Invoke(cachedSettings);
//             return;
//         }

//         Addressables.LoadAssetAsync<GlobalSettings>("GlobalSettings").Completed += handle =>
//         {
//             cachedSettings = handle.Result;
//             onComplete?.Invoke(cachedSettings);
//         };
//     }
// }

