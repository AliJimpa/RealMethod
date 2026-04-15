using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

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
        protected abstract void OnValidateAsset();
#endif
    }


    /// <summary>
    /// A standard asset used to store data.
    /// Developers can create and use these assets directly in the project
    /// and access the functionality provided by PrimitiveAsset.
    /// </summary>
    public abstract class DataAsset : PrimitiveAsset
    {
#if UNITY_EDITOR
        protected override sealed void OnValidateAsset()
        {

        }
#endif
    }
    /// <summary>
    /// Represents an asset used only as a clone.
    /// This asset cannot be used directly or instantiated normally.
    /// Its purpose is to generate clones that will be used instead of the original asset.
    /// </summary>
    public abstract class CloneAsset : PrimitiveAsset
    {
        [SerializeField, HideInInspector]
        private bool _isRuntimeClone = false;

#if UNITY_EDITOR
        protected override sealed void OnValidateAsset()
        {
            if (!HasCloneName())
            {
                if (!IsProjectAsset())
                {
                    Debug.LogError($"[{name}] CloneAsset cannot create new instance at runtime. Asset has been removed!");
                    DestroyImmediate(this);
                    return;
                }
            }
        }
#endif

        /// <summary>
        /// Creates a runtime clone of this asset and marks it as safe for usage.
        /// Direct project assets cannot be used; only cloned copies are valid.
        /// </summary>
        public CloneAsset Clone()
        {
            var clone = Instantiate(this);
            clone._isRuntimeClone = true;
            //clone.name = $"{name}_Clone";
            return clone;
        }

        /// <summary>
        /// Ensures this asset can be used safely. Throws an error if it's a direct project asset.
        /// </summary>
        protected void EnsureClonedUsage()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!_isRuntimeClone)
            {
                Debug.LogError($"[{name}] You cannot use a direct asset of type {GetType().Name}. Use Clone() instead.");
            }
#endif
        }
    }
    /// <summary>
    /// Represents an asset that defines how to create new instances.
    /// The asset itself cannot be used directly or cloned.
    /// Instead, the system creates new independent instances based on this asset's data.
    /// </summary>
    public abstract class InstanceAsset : PrimitiveAsset
    {
        [SerializeField, HideInInspector]
        private bool _isRuntimeInstance = false;

#if UNITY_EDITOR
        protected override sealed void OnValidateAsset()
        {
            if (HasCloneName())
            {
                Debug.LogError($"[{name}] InstanceAsset cannot be cloned directly. The cloned asset has been removed!");
                DestroyImmediate(this);
                return;
            }
        }
#endif

        /// <summary>
        /// Creates a valid runtime instance of this InstanceAsset.
        /// This is the only allowed way to use InstanceAsset at runtime.
        /// </summary>
        public static T Create<T>(string name = "") where T : InstanceAsset
        {
            T inst = CreateInstance<T>();
            inst._isRuntimeInstance = true;
            inst.name = $"{name}_Instance";
            return inst;
        }

        /// <summary>
        /// Ensures this asset is being used correctly.
        /// Only runtime-created instances are allowed at runtime.
        /// </summary>
        protected void EnsureInstanceUsage()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!_isRuntimeInstance)
            {
                Debug.LogError(
                    $"[{name}] InstanceAsset must be instantiated using Create<T>(). " +
                    "Direct use of project asset is not allowed.");
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
#if UNITY_EDITOR
        protected override sealed void OnValidateAsset()
        {
            if (HasCloneName())
            {
                Debug.LogError($"[{name}] UniqueAsset cannot clone at runtime. NewAsset has been removed!");
                Destroy(this);
                return;
            }
            if (!IsProjectAsset())
            {
                Debug.LogError($"[{name}] UniqueAsset cannot create new instance at runtime. NewAsset has been removed!");
                Destroy(this);
                return;
            }
        }
#endif
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