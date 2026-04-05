using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RealMethod
{
    public interface IAsset
    {
        PrimitiveAsset GetAsset();
    }
    // PrimitiveAsset: is a ScriptableObject with some functions & IAsset interface
    public abstract class PrimitiveAsset : ScriptableObject, IAsset, ISpawn, ISpawnWithAuthor
    {
        // Implement IAsset Interface
        PrimitiveAsset IAsset.GetAsset()
        {
            return this;
        }
        // Implement ISpawn interface
        void ISpawn.OnSpawn()
        {
            OnSpawn(null);
        }
        // Implement ISpawnWithAuthor interface
        void ISpawnWithAuthor.OnSpawn(Object author)
        {
            OnSpawn(author);
        }


        protected virtual void OnSpawn(Object spawner)
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
        [InitializeOnEnterPlayMode] // Runs when entering Play Mode in Editor
        private static void EditorPlayModeInit()
        {
            var assets = Resources.FindObjectsOfTypeAll<PrimitiveAsset>();
            foreach (var asset in assets)
            {
                if (asset.IsProjectAsset())
                {
                    asset.OnEditorPlay();
                }
            }
        }

        public virtual void OnEditorPlay()
        {
            Debug.Log($"[{GetType()}]  -> {name} OnEditorPlay called.");
        }
#endif
    }


    // DataAsset: is just a PrimitiveAsset
    public abstract class DataAsset : PrimitiveAsset
    {
    }
    // TemplateAsset: is a PrimitiveAsset that you can't create new at runtime & Should Use With Clone
    public abstract class TemplateAsset : PrimitiveAsset
    {
        protected virtual void OnEnable()
        {
            if (!HasCloneName())
            {
                if (!IsProjectAsset())
                {
                    Debug.LogError($"TemplateAsset Can't Create New Instance at Runtime, NewFile Removed!");
                    Destroy(this);
                    return;
                }
            }
        }

#if UNITY_EDITOR
        public override void OnEditorPlay()
        {

        }
#endif
    }
    // FileAsset: is a PrimitiveAsset that you can't clone at runtime
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
    // UniqueAsset: is a PrimitiveAsset that you can't clone or create new at runtime
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

#if UNITY_EDITOR
        public override void OnEditorPlay()
        {

        }
#endif
    }
    // ConfigAsset: is a UniqueAsset that you can't decelar modifier variable or method , all of things should be readonly 
    public abstract class ConfigAsset : UniqueAsset
    {

    }

}