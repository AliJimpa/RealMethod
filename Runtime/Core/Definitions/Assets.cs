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
        protected virtual void OnEnable()
        {
            if (!HasCloneName())
            {
                if (!IsProjectAsset())
                {
                    Debug.LogError($"CloneAsset Can't Create New Instance at Runtime, NewFile Removed!");
                    Destroy(this);
                    return;
                }
            }
        }
    }
    /// <summary>
    /// Represents an asset that defines how to create new instances.
    /// The asset itself cannot be used directly or cloned.
    /// Instead, the system creates new independent instances based on this asset's data.
    /// </summary>
    public abstract class InstanceAsset : PrimitiveAsset
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
    /// <summary>
    /// Represents a unique shared asset in the project.
    /// Developers must use the asset directly and cannot clone it
    /// or create new instances from it.
    /// </summary>
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
    /// <summary>
    /// A specialized UniqueAsset used for global configuration.
    /// The asset is intended to be read-only at runtime,
    /// and developers should not add mutable fields or methods
    /// that modify its configuration values.
    /// </summary>
    public abstract class ConfigAsset : UniqueAsset
    {

    }

}