using System.IO;
using UnityEditor;
using UnityEngine;

namespace RealMethod
{
    public static class RealMethodSetting
    {
        private const string settingsPath = "Assets/Resources/RealMethod/RealMethodSetting.asset";
        private static bool candraw = true;// Flag to determine if the UI can be drawn
        private static SettingSection[] sections = new SettingSection[2] {
        // Array of sections to be rendered in the settings UI
        new GameSetting(),
        new ContentSetting()
    };


        [SettingsProvider]// Create a SettingsProvider for Unity's Project Settings
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Real Method", SettingsScope.Project)
            {
                label = "Real Method",

                // Called when the settings tab is first selected
                activateHandler = (searchContext, rootElement) =>
                {
                    RealMethodSettingAsset TargetStorage = null;

                    // Attempt to load the settings asset
                    if (!GetSettingStorage(out TargetStorage))
                    {
                        if (Directory.Exists(Path.GetDirectoryName(settingsPath)))
                        {
                            TargetStorage = CreateSettingStorage();
                        }
                        else
                        {
                            candraw = false;
                        }
                    }

                    // Initialize each section with the loaded settings
                    foreach (var item in sections)
                    {
                        if (TargetStorage != null)
                            item.BeginRender(TargetStorage);
                    }
                },

                // Called to draw the UI elements
                guiHandler = (searchContext) =>
                {
                    if (candraw)
                    {
                        // Render each section
                        foreach (var item in sections)
                        {
                            item.UpdateRender();
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox($"The Address is not valid {Path.GetDirectoryName(settingsPath)}", MessageType.Error);
                        if (GUILayout.Button("Fix"))
                        {
                            if (!Directory.Exists("Assets/Resources"))
                                AssetDatabase.CreateFolder("Assets", "Resources");

                            if (!Directory.Exists("Assets/Resources/RealMethod"))
                                AssetDatabase.CreateFolder("Assets/Resources", "RealMethod");


                            RealMethodSettingAsset TargetStorage = CreateSettingStorage();
                            foreach (var item in sections)
                            {
                                if (TargetStorage != null)
                                    item.BeginRender(TargetStorage);
                            }

                            candraw = true;
                        }
                    }
                }
            };

            return provider;
        }


        private static bool GetSettingStorage(out RealMethodSettingAsset settings)
        {
            // Attempt to load the settings asset from the specified path
            settings = AssetDatabase.LoadAssetAtPath<RealMethodSettingAsset>(settingsPath);
            return settings != null;
        }
        private static RealMethodSettingAsset CreateSettingStorage()
        {
            // Create a new settings asset at the specified path
            RealMethodSettingAsset settings = ScriptableObject.CreateInstance<RealMethodSettingAsset>();
            AssetDatabase.CreateAsset(settings, settingsPath);
            AssetDatabase.SaveAssets();
            return settings;
        }
    }


    // Abstract base class for a settings section
    public abstract class SettingSection
    {

        private bool isReady = true;// Indicates whether the section is ready to render
        private string message = string.Empty;// Error message to display if the section is not ready
        private int errorid = 0;// Error ID to identify the type of error


        public SettingSection()
        {
            Initialized();
        }

        public void BeginRender(RealMethodSettingAsset storage)
        {
            FirstSelected(storage);
        }
        public void UpdateRender()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(GetTitle(), EditorStyles.boldLabel); // Section Title

            if (isReady)
            {
                // Render the section's content
                Draw();
            }
            else
            {
                // Display an error message if the section is not ready
                EditorGUILayout.HelpBox(message, MessageType.Error);

                // Provide a "Fix" button to resolve the error
                if (GUILayout.Button("Fix"))
                {
                    Fix(errorid);
                }
            }
        }


        protected abstract void Initialized();
        protected abstract void FirstSelected(RealMethodSettingAsset Storage);
        protected abstract void Draw();
        protected abstract string GetTitle();
        protected abstract void Fix(int Id);


        protected void Error(string Message, int Id = 0)
        {
            if (isReady)
            {
                isReady = false;
            }
            message = Message;
            errorid = Id;
        }
        protected void ClearError()
        {
            message = string.Empty;
            errorid = 0;
            if (!isReady)
            {
                isReady = true;
            }
            UpdateRender();
        }

    }
}