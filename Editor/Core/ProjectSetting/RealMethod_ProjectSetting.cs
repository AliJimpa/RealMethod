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
    public static class RealMethod_ProjectSetting
    {
        private static bool candraw = true;// Flag to determine if the UI can be drawn
        private static List<ProjectSettingSection> sections = new List<ProjectSettingSection>(3) {
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
                    if (!GetSettingStorage(out TargetStorage))
                    {
                        if (Directory.Exists(Path.GetDirectoryName(RM_CoreEditor.SetttingAssetPath)))
                        {
                            TargetStorage = CreateSettingStorage();
                        }
                        else
                        {
                            candraw = false;
                        }
                    }

                    // Initialize Main section with the loaded settings
                    ((ISectionSetting)sections[0]).FirstSelected(TargetStorage);
                    ((ISectionSetting)sections[1]).FirstSelected(TargetStorage);
                },


                // Called to draw the UI elements
                guiHandler = (searchContext) =>
                {
                    if (candraw)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Runtime", EditorStyles.whiteBoldLabel);
                        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                        // Render each section
                        foreach (var item in sections)
                        {
                            ISectionSetting ptovider = item;
                            if (ptovider.IsRuntime())
                            {
                                ptovider.Draw();
                                EditorGUILayout.Space(1);
                            }
                        }
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Editor", EditorStyles.whiteBoldLabel);
                        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                        foreach (var item in sections)
                        {
                            ISectionSetting ptovider = item;
                            if (!ptovider.IsRuntime())
                            {

                                ptovider.Draw();
                                EditorGUILayout.Space(1);

                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox($"The Address is not valid {Path.GetDirectoryName(RM_CoreEditor.SetttingAssetPath)}", MessageType.Error);
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
            settings = AssetDatabase.LoadAssetAtPath<ProjectSettingAsset>(RM_CoreEditor.SetttingAssetPath);
            AddExteraSections(settings);
            return settings != null;
        }
        private static ProjectSettingAsset CreateSettingStorage()
        {
            // Create a new settings asset at the specified path
            ProjectSettingAsset settings = ScriptableObject.CreateInstance<ProjectSettingAsset>();
            AssetDatabase.CreateAsset(settings, RM_CoreEditor.SetttingAssetPath);
            AssetDatabase.SaveAssets();
            AddExteraSections(settings);
            return settings;
        }
        private static void AddExteraSections(ProjectSettingAsset settings)
        {
            Type[] sectiontypes = settings.GetExteraSections();
            foreach (var item in sectiontypes)
            {
                if (item != null)
                {
                    if (typeof(ProjectSettingSection).IsAssignableFrom(item))
                    {
                        try
                        {
                            ProjectSettingSection TargetSection = (ProjectSettingSection)Activator.CreateInstance(item);
                            ((ISectionSetting)TargetSection).FirstSelected(settings);
                            sections.Add(TargetSection);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Failed to instantiate {item}: {ex.Message}. DefaultGameBridge Created");
                        }
                    }
                }
            }
        }
    }
}