using System.IO;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public static class RM_CoreEditor
    {
        public const string SetttingAssetPath = "Assets/Resources/RealMethod/RealMethodSetting.asset";
        public static string ScriptTemplatesPath => GetPackagePath("com.mustard.realmethod") + "/Reservoir/ScriptTemplates";
        public static string PrefabTemplatePath => GetPackagePath("com.mustard.realmethod") + "/Reservoir/Prefabs";

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