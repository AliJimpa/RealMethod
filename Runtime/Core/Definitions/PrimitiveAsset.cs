using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RealMethod
{
    // All Scriptable Object in RealMethod should use this
    public abstract class PrimitiveAsset : ScriptableObject
    {

    }



    // this Asset is basic asset in method can reset on editor begin
    public abstract class DataAsset : PrimitiveAsset
    {
        public static bool IsRuntimeClone(PrimitiveAsset so)
        {
            return so.hideFlags == HideFlags.DontSave || so.name.EndsWith("(Clone)");
        }


#if UNITY_EDITOR
        /// This method runs once at game start, calls OnEditorPlay
        [InitializeOnEnterPlayMode]
        private static void EditorReset()
        {
            var assets = Resources.FindObjectsOfTypeAll<DataAsset>();

            foreach (var asset in assets)
            {
                if (IsRuntimeClone(asset))
                    continue;

                asset.OnEditorPlay();
            }
        }

        /// <summary>
        /// Custom reset method you override in children
        /// (Editor only)
        /// </summary>
        public virtual void OnEditorPlay()
        {
            Debug.Log($"{name} OnEditorPlay called.");
        }
#endif
    }
    // This Asset Can Created in game but You can't Clone 
    public abstract class FileAsset : PrimitiveAsset
    {
        private static bool _creationInProgress = false;

        /// <summary>
        /// Use this factory method instead of CreateInstance directly.
        /// </summary>
        public static T Create<T>() where T : FileAsset
        {
            _creationInProgress = true;
            var instance = CreateInstance<T>();
            _creationInProgress = false;
            return instance;
        }

        protected virtual void OnEnable()
        {
            // Allow if being created via factory
            if (_creationInProgress)
                return;

            // Detect if this was a clone of an existing SO
            if (IsCloneInstance())
            {
                Debug.LogError($"Cloning of {name} is not allowed!");
                //Destroy(this);
            }
        }

        private bool IsCloneInstance()
        {
#if UNITY_EDITOR
            // In editor: check if this SO is an unsaved instance (not an asset)
            return !AssetDatabase.Contains(this);
#else
        // In runtime: assume it's a clone if not created through the factory
        return true;
#endif
        }
    }
    // This Asset Can't Create or Clone Just can created in editor
    public abstract class UniqueAsset : PrimitiveAsset
    {
        private static bool _creationInProgress = false;

        
        protected virtual void OnEnable()
        {
            // Allow if being created via factory
            if (_creationInProgress)
                return;

            // Detect if this was a clone of an existing SO
            if (IsCloneInstance())
            {
                Debug.LogError($"Cloning of {name} is not allowed!");
                //Destroy(this);
            }
        }

        private bool IsCloneInstance()
        {
#if UNITY_EDITOR
            // In editor: check if this SO is an unsaved instance (not an asset)
            return !AssetDatabase.Contains(this);
#else
        // In runtime: assume it's a clone if not created through the factory
        return true;
#endif
        }
    }
    // This Asset is a data container that all data is readonly just for use not change
    public abstract class ConfigAsset : PrimitiveAsset
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