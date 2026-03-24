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
        bool IsRuntime();
    }
    // Abstract base class for a settings section
    public abstract class ProjectSettingSection : ISectionSetting
    {
        protected enum SectionType
        {
            Runtime,
            Editor,
        }
        protected class TypeSelector<T>
        {
            private SerializedProperty myProperty;
            private string fieldName;
            private List<Type> typeList;
            private string[] displayNames;
            private string[] assemblyNames;
            private int selectedIndex = 0;
            private int newIndex;


            public TypeSelector(SerializedProperty TargetStringProperty, string displayName)
            {
                if (TargetStringProperty != null && displayName != string.Empty)
                {
                    myProperty = TargetStringProperty;
                    fieldName = displayName;
                }
                else
                {
                    Debug.LogWarning("TypeSelector Can't Create");
                    return;
                }


                typeList = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract)
                    .ToList();

                displayNames = typeList.Select(t => $"{t.Namespace}.{t.Name}").ToArray(); // shown in UI
                assemblyNames = typeList.Select(t => t.AssemblyQualifiedName).ToArray(); // stored
            }

            public void Draw()
            {
                selectedIndex = Array.IndexOf(assemblyNames, myProperty.stringValue);

                newIndex = EditorGUILayout.Popup(fieldName, selectedIndex, displayNames);

                if (newIndex >= 0 && newIndex < assemblyNames.Length)
                {
                    myProperty.stringValue = assemblyNames[newIndex];
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
            EditorGUILayout.LabelField($"{GetTitle()} ({GetSectionType()})", EditorStyles.boldLabel); // Section Title

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
        bool ISectionSetting.IsRuntime()
        {
            return GetSectionType() == SectionType.Runtime;
        }

        // Abstract Method
        protected abstract void Initialized();
        protected abstract void BeginRender(ProjectSettingAsset Storage);
        protected abstract void UpdateRender();
        protected abstract string GetTitle();
        protected abstract SectionType GetSectionType();
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
    public static class ProjectSettingProvider
    {
        private static bool candraw = true;// Flag to determine if the UI can be drawn
        private static List<ProjectSettingSection> sections = new List<ProjectSettingSection>(2) {
        // Array of sections to be rendered in the settings UI
        new InitializerSetting_Section(),
        new FolderStructure_Section()
        };


        [SettingsProvider]
        // Create a SettingsProvider for Unity's Project Settings 
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider("Project/RealMethod", SettingsScope.Project)
            {
                label = "RealMethod",

                // Called when the settings tab is first selected
                activateHandler = (searchContext, rootElement) =>
                {
                    ProjectSettingAsset TargetStorage = null;

                    // Attempt to load the settings asset
                    if (!RealMethod.GetSettingStorage(out TargetStorage))
                    {
                        if (Directory.Exists(Path.GetDirectoryName(RealMethod.SetttingAssetPath)))
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
                        EditorGUILayout.HelpBox($"The Address is not valid {Path.GetDirectoryName(RealMethod.SetttingAssetPath)}", MessageType.Error);
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



        private static ProjectSettingAsset CreateSettingStorage()
        {
            // Create a new settings asset at the specified path
            ProjectSettingAsset settings = ScriptableObject.CreateInstance<ProjectSettingAsset>();
            AssetDatabase.CreateAsset(settings, RealMethod.SetttingAssetPath);
            AssetDatabase.SaveAssets();
            return settings;
        }
    }
}