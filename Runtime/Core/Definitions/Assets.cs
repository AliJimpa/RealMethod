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

    /// <summary>
    /// Base class for all custom asset types in the system.
    /// Provides shared functionality and common rules for assets derived from ScriptableObject,
    /// such as cloning, instancing, or direct usage depending on the derived asset type.
    /// </summary>
    public abstract class PrimitiveAsset : ScriptableObject, IAsset, ISpawn, ISpawnWithAuthor
    {
        public bool IsSpawned { get; private set; } = false;
        // Implement IAsset Interface
        PrimitiveAsset IAsset.GetAsset() => this;
        // Implement ISpawn interface
        void ISpawn.OnSpawn()
        {
            IsSpawned = true;
            EnsureAssetPermission();
            OnSpawn(null);
        }
        // Implement ISpawnWithAuthor interface
        void ISpawnWithAuthor.OnSpawn(Object author)
        {
            IsSpawned = true;
            EnsureAssetPermission();
            OnSpawn(author);
        }


        protected virtual void OnSpawn(Object spawner)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"[{name}] Spawned");
#endif
        }
        /// <summary>
        /// Ensures this asset can be used safely. Throws an error if it's not.
        /// </summary>
        protected virtual void EnsureAssetPermission()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogError($"[{name}] For this asset didnt write any permission.");
#endif
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
        /// <summary>
        /// Returns whether Reset() should be automatically called for the given
        /// PlayModeStateChange. If this method returns true, Unity's Reset() method
        /// on this ScriptableObject will be invoked for that state.
        /// </summary>
        /// <param name="state">The current play mode state change.</param>
        public virtual bool AutoReset(PlayModeStateChange state)
        {
            return false;
        }
#endif
    }


    /// <summary>
    /// A standard asset used to store data.
    /// Developers can create and use these assets directly in the project
    /// and access the functionality provided by PrimitiveAsset.
    /// </summary>
    public abstract class DataAsset : PrimitiveAsset
    {

    }
    /// <summary>
    /// Represents an asset used only as a clone.
    /// This asset cannot be used directly or instantiated normally.
    /// Its purpose is to generate clones that will be used instead of the original asset.
    /// </summary>
    public abstract class CloneAsset : PrimitiveAsset
    {
        protected sealed override void EnsureAssetPermission()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!HasCloneName())
            {
                if (!IsProjectAsset())
                {
                    Debug.LogError($"[{name}] CloneAsset cannot create new instance at runtime. Asset has been removed!");
                    DestroyImmediate(this);
                    return;
                }
            }
#endif
        }

    }
    /// <summary>
    /// Represents a unique shared asset in the project.
    /// Developers must use the asset directly and cannot clone it
    /// or create new instances from it.
    /// </summary>
    public abstract class UniqueAsset : PrimitiveAsset
    {
        protected override void EnsureAssetPermission()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (HasCloneName())
            {
                Debug.LogError($"[{name}] UniqueAsset cannot clone at runtime. NewAsset has been removed!");
                DestroyImmediate(this);
                return;
            }
            if (!IsProjectAsset())
            {
                Debug.LogError($"[{name}] UniqueAsset cannot create new instance at runtime. NewAsset has been removed!");
                DestroyImmediate(this);
                return;
            }
#endif
        }
    }
    /// <summary>
    /// A specialized UniqueAsset used for global configuration.
    /// The asset is intended to be read-only at runtime,
    /// and developers should not add mutable fields or methods
    /// that modify its configuration values.
    /// </summary>
    public abstract class ConfigAsset : UniqueAsset
    {

    }
    /// <summary>
    ///  A specialized UniqueAsset used for File system.
    /// The asset is uniqueAsset can instance at runtime.
    /// and developer can use fileasset for saving data in asset,
    /// or in runtime asset creation.
    /// The asset itself cannot be used cloned.
    /// Instead, the system creates new independent instances based on this asset's data.
    /// </summary>
    public abstract class FileAsset : UniqueAsset
    {
        protected override void EnsureAssetPermission()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            base.EnsureAssetPermission();
            if (IsSpawned)
            {
                if (IsProjectAsset())
                {
                    Debug.LogError($"[{name}] This asset is direct asset but Spawn Event called. Asset has been removed!");
                    DestroyImmediate(this);
                    return;
                }
            }
#endif
        }
    }
}