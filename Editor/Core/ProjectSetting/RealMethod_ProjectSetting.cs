using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    // Interface for call base method in ProjectSettingSection by RealMethodSetting
    interface ISectionSetting
    {
        void FirstSelected(ProjectSettingAsset storage);
        void Draw();
    }

    // Abstract base class for a settings section
    public abstract class ProjectSettingSection : ISectionSetting
    {
        protected class ClassType<T>
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
        private bool isReady = true;// Indicates whether the section is ready to render
        private string message = string.Empty;// Error message to display if the section is not ready
        private int errorid = 0;// Error ID to identify the type of error

        public ProjectSettingSection()
        {
            Initialized();
        }

        // Implement ISectionSetting Interface
        void ISectionSetting.FirstSelected(ProjectSettingAsset storage)
        {
            BeginRender(storage);
        }
        void ISectionSetting.Draw()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(GetTitle(), EditorStyles.boldLabel); // Section Title

            if (isReady)
            {
                // Render the section's content
                UpdateRender();
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

        // Abstract Method
        protected abstract void Initialized();
        protected abstract void BeginRender(ProjectSettingAsset Storage);
        protected abstract void UpdateRender();
        protected abstract string GetTitle();
        protected abstract void Fix(int Id);

        // Protected Function
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

    // Project Setting
    public static class RealMethod_ProjectSetting
    {
        private const string settingsPath = "Assets/Resources/RealMethod/RealMethodSetting.asset";
        private static bool candraw = true;// Flag to determine if the UI can be drawn
        private static ProjectSettingSection[] sections = new ProjectSettingSection[2] {
        // Array of sections to be rendered in the settings UI
        new InitializerSetting_Section(),
        new FolderStructure_Section()
        };


        [SettingsProvider]
        // Create a SettingsProvider for Unity's Project Settings
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Real Method", SettingsScope.Project)
            {
                label = "Real Method",

                // Called when the settings tab is first selected
                activateHandler = (searchContext, rootElement) =>
                {
                    ProjectSettingAsset TargetStorage = null;

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
                        ISectionSetting ptovider = item;
                        if (TargetStorage != null)
                            ptovider.FirstSelected(TargetStorage);
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
                            ISectionSetting ptovider = item;
                            ptovider.Draw();
                            // Add a separator line
                            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
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


                            ProjectSettingAsset TargetStorage = CreateSettingStorage();
                            foreach (var item in sections)
                            {
                                ISectionSetting ptovider = item;
                                if (TargetStorage != null)
                                    ptovider.FirstSelected(TargetStorage);
                            }

                            candraw = true;
                        }
                    }
                }
            };

            return provider;
        }


        public static bool GetSettingStorage(out ProjectSettingAsset settings)
        {
            // Attempt to load the settings asset from the specified path
            settings = AssetDatabase.LoadAssetAtPath<ProjectSettingAsset>(settingsPath);
            return settings != null;
        }
        private static ProjectSettingAsset CreateSettingStorage()
        {
            // Create a new settings asset at the specified path
            ProjectSettingAsset settings = ScriptableObject.CreateInstance<ProjectSettingAsset>();
            AssetDatabase.CreateAsset(settings, settingsPath);
            AssetDatabase.SaveAssets();
            return settings;
        }
    }
}