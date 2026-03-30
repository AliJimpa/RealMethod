using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

namespace RealMethod.Editor
{
    [InitializeOnLoad]
    public static class ConfigAssetInitializer
    {
        static ConfigAssetInitializer()
        {
            EditorApplication.delayCall += ValidateAllConfigs;
        }

        public static void ValidateAllConfigs()
        {
            var configTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(ConfigAsset)));

            foreach (var type in configTypes)
            {
                // 🔹 Check fields
                var badFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                    .Where(f => !f.IsInitOnly && !f.IsLiteral && !f.IsDefined(typeof(SerializeField), false))
                    .ToList();

                foreach (var f in badFields)
                {
                    Debug.LogError($"❌ '{type.Name}' has non-readonly field '{f.Name}' — only readonly fields allowed in Config-derived classes.");
                }

                // 🔹 Check methods
                var badMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName && !m.IsConstructor)
                .Where(m => ViolatesPureMethodRules(m))
                .ToList();

                foreach (var m in badMethods)
                {
                    Debug.LogError($"❌ '{type.Name}' public method '{m.Name}' modifies state — only pure getter/return methods are allowed in Config-derived classes.");
                }
            }
        }
        private static bool ViolatesPureMethodRules(MethodInfo method)
        {
            try
            {
                var body = method.GetMethodBody();
                if (body == null)
                    return false;

                var il = body.GetILAsByteArray();
                if (il == null)
                    return false;

                // Look for IL opcodes that store data or call setters
                for (int i = 0; i < il.Length - 1; i++)
                {
                    byte b = il[i];
                    byte next = il[i + 1];

                    // Store field (stfld or stsfld)
                    if (b == 0x7D || b == 0x80)
                        return true;

                    // Callvirt / call that might be setter
                    if (b == 0x28)
                    {
                        var tokens = method.Module.ResolveMethod(BitConverter.ToInt32(il, i + 1));
                        if (tokens.Name.StartsWith("set_", StringComparison.Ordinal))
                            return true;
                    }
                }
            }
            catch
            {
                // Ignore methods that can't be inspected
            }

            return false;
        }
    }
}