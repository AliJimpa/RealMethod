using System.IO;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class RM_Assets
    {
        public static string GetSelectedAssetPath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path)) return "Assets";
            if (Directory.Exists(path)) return path;
            return Path.GetDirectoryName(path);
        }
        public static bool IsPrefab(GameObject obj)
        {
            return PrefabUtility.GetPrefabAssetType(obj) != PrefabAssetType.NotAPrefab;
        }
    }
}