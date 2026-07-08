using System.IO;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public static class RM_Asset
    {
        public static string GetSelectedAssetDirectory()
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