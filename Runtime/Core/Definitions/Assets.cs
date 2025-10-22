using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RealMethod
{
    public interface IAsset
    {
        PrimitiveAsset GetAsset();
        void OnSpawned(Object spawner);
    }
    // All Scriptable Object in RealMethod should use this
    public abstract class PrimitiveAsset : ScriptableObject, IAsset
    {
        // Implement IAsset Interface
        PrimitiveAsset IAsset.GetAsset()
        {
            return this;
        }
        public virtual void OnSpawned(Object spawner)
        {

        }

        public bool HasCloneName()
        {
            // Best-effort clone detection: relies on name
            return name.EndsWith("(Clone)");
        }
        public bool IsCreatedAtRuntime()
        {
            // Runtime clones usually have DontSave set
            return (hideFlags & HideFlags.DontSave) != 0;
        }
        public bool IsProjectAsset()
        {
#if UNITY_EDITOR
            // In editor: check if this SO is an unsaved instance (not an asset)
            return AssetDatabase.Contains(this);
#else
        // Not a runtime object and not a clone → project asset
            return !HasCloneName() && !IsCreatedAtRuntime();
#endif
        }

#if UNITY_EDITOR
        public virtual void OnEditorPlay()
        {
            Debug.Log($"[{GetType()}]  -> {name} OnEditorPlay called.");
        }
#endif
    }


    // this Asset is basic asset in method can reset on editor begin
    public abstract class DataAsset : PrimitiveAsset
    {
    }
    // This Asset Can Created in game but You can't Clone 
    public abstract class FileAsset : PrimitiveAsset
    {
        protected virtual void OnEnable()
        {
            if (HasCloneName())
            {
                Debug.LogError($"FileAsset Can't Clone at Runtime, NewFile Removed!");
                Destroy(this);
                return;
            }
        }
    }
    // This Asset Can't Create or Clone Just can created in editor
    public abstract class UniqueAsset : PrimitiveAsset
    {
        protected virtual void OnEnable()
        {
            if (HasCloneName())
            {
                Debug.LogError($"UniqueAsset Can't Clone at Runtime, NewFile Removed!");
                Destroy(this);
                return;
            }
            if (!IsProjectAsset())
            {
                Debug.LogError($"UniqueAsset Can't Create New Instance at Runtime, NewFile Removed!");
                Destroy(this);
                return;
            }
        }
    }
    // This Asset is a data container that all data is readonly just for use not change
    public abstract class ConfigAsset : UniqueAsset
    {

    }

}