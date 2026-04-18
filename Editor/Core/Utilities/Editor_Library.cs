using System.IO;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public enum RealMethodLayer
    {
        Core,
        Library,
        Pattern,
        ReadySet,
        Toolkit
    }

    public static class RM_Editor
    {
        public static string SetttingAssetPath = ProjectSettingAsset.Path;
        public static string ScriptTemplatesPath => GetPackagePath("com.mustard.realmethod") + "/Reservoir/ScriptTemplates";
        public static string PrefabTemplatePath => GetPackagePath("com.mustard.realmethod") + "/Reservoir/Prefabs";
        public const string GameObjectMenuItemPath = "GameObject/RealMethod/";
        public const string ScriptMenuItemPath = "Assets/Create/Scripting/RealMethod/";
        public static string Documentation => GetPackagePath("com.mustard.realmethod") + "/Documentation/Information";
        public static bool IsPlayMode
        {
            get
            {
                return EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode == false;
            }
        }

        public static bool TryGetSettingAsset(out ProjectSettingAsset settings)
        {
            // Attempt to load the settings asset from the specified path
            settings = AssetDatabase.LoadAssetAtPath<ProjectSettingAsset>(SetttingAssetPath);
            return settings != null;
        }
        private static string GetPackagePath(string packageName)
        {
            string[] guids = AssetDatabase.FindAssets("package", new[] { "Packages/" + packageName });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains(packageName))
                {
                    string packagePath = Path.GetDirectoryName(path);
                    while (!string.IsNullOrEmpty(packagePath))
                    {
                        if (File.Exists(Path.Combine(packagePath, "package.json")))
                            return packagePath;

                        packagePath = Path.GetDirectoryName(packagePath);
                    }
                }
            }

            Debug.LogError($"Could not find package path for: {packageName}");
            return null;
        }
        public static void CreateScriptTemplate(string TemplateName, RealMethodLayer layer)
        {
            string NewFileName = $"My{TemplateName}.cs";
            string TemplateFileName = $"{layer}\\{TemplateName}Template.txt";
            string NewFilePath = CreateScriptTemplate(TemplateFileName, NewFileName);
        }
        public static GameObject CreatePrefabTemplate(string prefabName, bool UseProject = false)
        {

            string prefabPath = string.Empty;
            if (UseProject)
            {
                ProjectSettingAsset ProjectSetting = AssetDatabase.LoadAssetAtPath<ProjectSettingAsset>(SetttingAssetPath);
                prefabPath = Path.Combine(ProjectSetting.GetFolderPathByType(ProjectSettingAsset.AssetFormat.Prefab), prefabName);
            }
            else
            {
                prefabPath = Path.Combine(PrefabTemplatePath, prefabName);
            }

            // Load the prefab from the specified path
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab != null)
            {
                // Create an instance of the prefab in the scene
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

                // Register the creation in the undo system
                Undo.RegisterCreatedObjectUndo(instance, "Create " + instance.name);

                // Select the newly created instance
                Selection.activeObject = instance;
                return instance;
            }
            else
            {
                Debug.LogError("Prefab not found at path: " + prefabPath);
                return null;
            }
        }
        public static string GetScenePathByName(string name)
        {
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.path.Contains(name))
                    return scene.path;
            }
            return null;
        }





        private static string CreateScriptTemplate(string templateFileName, string defaultName)
        {
            string templatePath = string.Empty;
            if (Directory.Exists("Assets/RealDev/_General"))
            {
                Debug.LogWarning("When This Message show RealMetod using hardcode addres becuase find [Assets/RealDev/_General]");
                templatePath = Path.Combine("Assets\\RealMethod\\Reservoir\\ScriptTemplates\\", templateFileName);
            }
            else
            {
                templatePath = Path.Combine(ScriptTemplatesPath, templateFileName);
            }


            if (!File.Exists(templatePath))
            {
                Debug.LogError($"Template file not found: {templatePath}");
                return string.Empty;
            }

            string selectedPath = RM_Asset.GetSelectedAssetDirectory();
            string newScriptPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(selectedPath, defaultName));

            // Prompt user for script name before creating the file
            string inputName = EditorUtility.SaveFilePanel(
                "Create Script",
                selectedPath,
                Path.GetFileNameWithoutExtension(defaultName),
                "cs"
            );

            if (string.IsNullOrEmpty(inputName))
                return string.Empty;

            // Ensure the path is relative to the Assets folder
            if (inputName.StartsWith(Application.dataPath))
                newScriptPath = "Assets" + inputName.Substring(Application.dataPath.Length);
            else
                newScriptPath = inputName;

            string template = File.ReadAllText(templatePath);
            template = template.Replace("#SCRIPTNAME#", Path.GetFileNameWithoutExtension(newScriptPath));
            string projectName = Application.productName;
            template = template.Replace("#PROJECTNAME#", projectName);

            File.WriteAllText(newScriptPath, template);
            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<MonoScript>(newScriptPath);
            return newScriptPath;
        }

    }
}