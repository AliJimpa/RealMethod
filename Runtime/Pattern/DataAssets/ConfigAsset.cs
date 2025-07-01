using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RealMethod
{
    public abstract class ConfigAsset : DataAsset
    {
#if UNITY_EDITOR
        private void ValidateImmutableFields()
        {
            var type = GetType();

            // Check public fields
            var publicFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in publicFields)
            {
                Debug.LogError($"❌ [Config Validation] Public field '{field.Name}' in {type.Name} should be private.");
                EditorApplication.isPlaying = false;
            }

            // Check public properties with public setters
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var prop in props)
            {
                if (prop.CanWrite && prop.SetMethod != null && prop.SetMethod.IsPublic)
                {
                    Debug.LogError($"❌ [Config Validation] Property '{prop.Name}' in {type.Name} has a public setter. Make it read-only.");
                    EditorApplication.isPlaying = false;
                }
            }
        }
#endif

    }
}