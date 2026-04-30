using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealMethod
{
    public abstract class Scope : MonoBehaviour
    {
        /// <summary>
        /// Cached array of managers that were instantiated from configured game prefabs or World gameobject.
        /// </summary>
        private IGameManager[] Managers = null;
        /// <summary>
        /// Cached array of managers that were instantiated from configured game prefabs or World gameobject.
        /// </summary>
        // public IReadOnlyList<IGameManager> manager => Managers;


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
            result = null;

            if (Managers == null)
                return false;

            foreach (var manager in Managers)
            {
                if (manager?.Component is T found)
                {
                    result = found;
                    return true;
                }
            }

            return false;
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


            if (Managers != null)
            {
                Debug.LogError("CollectManagers was called more than once. This is not allowed.");
                return;
            }

            HashSet<IGameManager> managerCache = new HashSet<IGameManager>();
            for (int i = 0; i < Objects.Length; i++)
            {
                GameObject Obj = Objects[i];
                if (Obj == null)
                    continue;

                var found = Obj.GetComponents<IGameManager>();

                foreach (var manager in found)
                {
                    if (managerCache.Add(manager))
                    {
                        manager.InitiateManager(this);
                    }
                    else
                    {
                        Debug.LogWarning($"Duplicate manager detected: {manager.GetType().Name} in {manager.Component.gameObject.name}");
                        continue;
                    }
                }
            }
            Managers = managerCache.ToArray();
        }



    }



}