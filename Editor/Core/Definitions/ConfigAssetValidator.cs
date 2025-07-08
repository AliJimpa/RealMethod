#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

namespace RealMethod.Editor
{
    [InitializeOnLoad]
    public static class ConfigAssetValidator
    {
        static ConfigAssetValidator()
        {
            EditorApplication.delayCall += ValidateAllConfigs;
        }

        static void ValidateAllConfigs()
        {
            var configTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(ConfigAsset)));

            foreach (var type in configTypes)
            {
                var badFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                    .Where(f => !f.IsInitOnly && !f.IsLiteral && !f.IsDefined(typeof(SerializeField), false))
                    .ToList();

                foreach (var f in badFields)
                {
                    Debug.LogError($"❌ '{type.Name}' has non-readonly field '{f.Name}' — only readonly fields allowed in Config-derived classes.");
                }
            }
        }
    }
#endif

}