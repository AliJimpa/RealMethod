using UnityEditor;
using UnityEngine;
using System.IO;

namespace RealMethod.Editor
{
    public static class ScriptCreator
    {
        private const string settingsPath = "Assets/Resources/RealMethod/RealMethodSetting.asset";
        private static string TemplateFolder => GetPackagePath("com.mustard.realmethod") + "/Source/ScriptTemplates";

        public static string Create(string templateFileName, string defaultName, bool UseProject = false)
        {
            string templatePath = string.Empty;
            if (UseProject)
            {
                ProjectSettingAsset ProjectSetting = AssetDatabase.LoadAssetAtPath<ProjectSettingAsset>(settingsPath);
                templatePath = Path.Combine(ProjectSetting.FindAddres(ProjectSettingAsset.IdentityAsset.ScriptTemplate).Path, templateFileName);
            }
            else
            {
                templatePath = Path.Combine(TemplateFolder, templateFileName);
            }


            if (!File.Exists(templatePath))
            {
                Debug.LogError($"Template file not found: {templatePath}");
                return string.Empty;
            }

            string selectedPath = GetSelectedPath();
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

        private static string GetSelectedPath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path)) return "Assets";
            if (Directory.Exists(path)) return path;
            return Path.GetDirectoryName(path);
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
    }
}

