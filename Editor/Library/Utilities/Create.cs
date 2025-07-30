using UnityEditor;
using UnityEngine;
using System.IO;

namespace RealMethod.Editor
{
    public static class RM_Create
    {
        public static string Script(string templateFileName, string defaultName, bool UseProject = false)
        {
            string templatePath = string.Empty;
            if (UseProject)
            {
                ProjectSettingAsset ProjectSetting = AssetDatabase.LoadAssetAtPath<ProjectSettingAsset>(RM_CoreEditor.SetttingAssetPath);
                templatePath = Path.Combine(ProjectSetting.FindAddres(ProjectSettingAsset.IdentityAsset.ScriptTemplate).Path, templateFileName);
            }
            else
            {
                templatePath = Path.Combine(RM_CoreEditor.ScriptTemplatesPath, templateFileName);
            }


            if (!File.Exists(templatePath))
            {
                Debug.LogError($"Template file not found: {templatePath}");
                return string.Empty;
            }

            string selectedPath = RM_Assets.GetSelectedAssetPath();
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
        public static GameObject Prefab(string prefabName, bool UseProject = false)
        {

            string prefabPath = string.Empty;
            if (UseProject)
            {
                ProjectSettingAsset ProjectSetting = AssetDatabase.LoadAssetAtPath<ProjectSettingAsset>(RM_CoreEditor.SetttingAssetPath);
                prefabPath = Path.Combine(ProjectSetting.FindAddres(ProjectSettingAsset.IdentityAsset.PrefabTemplate).Path, prefabName);
            }
            else
            {
                prefabPath = Path.Combine(RM_CoreEditor.PrefabTemplatePath, prefabName);
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
        public static void GameObjectInScene<T>(string Name = "GameObject") where T : Component
        {
            GameObject instance = new GameObject(Name);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(instance, "Create " + instance.name);
            instance.AddComponent<T>();
            // Select the newly created instance
            Selection.activeObject = instance;
        }
        public static T ScriptableObj<T>(string path) where T : ScriptableObject
        {
            // Create an instance of the ScriptableObject
            T asset = ScriptableObject.CreateInstance<T>();

            // Ensure the directory exists
            string directory = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            // Save the asset
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            // Focus on the newly created asset in the Project window
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            Debug.Log($"ScriptableObject of type {typeof(T).Name} created and saved at: {path}");

            // Return the created asset
            return asset;
        }
    }
}

