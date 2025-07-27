using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class RM_Project
    {
        public static class RM_ObjectUtils
        {
            public static bool IsPrefab(GameObject obj)
            {
                return PrefabUtility.GetPrefabAssetType(obj) != PrefabAssetType.NotAPrefab;
            }
        }
    }
}