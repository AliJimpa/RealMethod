using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// An abstract base class that extends <see cref="RealKernel"/> to provide a structured
    /// container for managing game systems and features within a specific context.
    /// 
    /// Scope instances are responsible for collecting, initializing, and providing access
    /// to various <see cref="IGameManager"/> implementations.
    /// It acts as a localized service locator for its immediate context and potentially
    /// delegates to parent scopes for unresolved dependencies.
    /// 
    /// This class defines the core manager lifecycle within a given boundary, such as
    /// global application lifetime or a scene-specific context.
    /// </summary>
    /// <remarks>
    /// Developers will typically implement or inherit from concrete Scope types like
    /// <see cref="Game"/>Scope (for global systems) or <see cref="World"/>Scope (for scene-bound systems).
    /// </remarks>
    public abstract class Scope : Kernel
    {
        /// <summary>
        /// Attempts to find a manager whose <c>Component</c> matches the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The component type to search for.</typeparam>
        /// <param name="result">
        /// When this method returns, contains the found component of type <typeparamref name="T"/> if successful;
        /// otherwise <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if a matching manager component was found; otherwise <c>false</c>.
        /// </returns>
        public bool TryFindManager<T>(out T result) where T : Component, IGameManager
        {
            return Registry.TryGet(out result);
        }
        /// <summary>
        /// Collects all <see cref="IGameManager"/> components from the provided GameObjects,
        /// initializes them, and stores them internally.
        ///
        /// This method can only be executed once. If it is called again after managers
        /// have already been collected, an error will be logged and the method will exit.
        ///
        /// Duplicate manager instances are ignored and logged as warnings.
        /// </summary>
        /// <param name="Objects">
        /// Array of GameObjects to search for <see cref="IGameManager"/> components.
        /// Null objects in the array are skipped.
        /// </param>
        protected void CollectManagers(GameObject[] Objects)
        {
            if (Objects == null)
            {
                Debug.LogError("CollectManagers received a null GameObjects array.");
                return;
            }

            for (int i = 0; i < Objects.Length; i++)
            {
                GameObject Obj = Objects[i];
                if (Obj == null)
                    continue;

                var found = Obj.GetComponents<IGameManager>();

                foreach (var manager in found)
                {
                    Component comp = manager.Component;
                    Registry.Register(comp, comp.GetType());
                    manager.InitiateManager(this);
                }
            }
        }

    }



}