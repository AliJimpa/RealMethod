using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// Despawn is a sealed static service used to instantiate gameplay objects such as
    /// prefabs, UI elements, audio sources, and other spawnable types.
    ///
    /// This class automatically locates the required manager instances from the
    /// World scope. When a new spawn request is made, Despawn will:
    ///     1) Look for a cached manager of the requested type.
    ///     2) If missing, it will attempt to find the manager inside World.
    ///     3) The found manager is cached internally for reuse to avoid repeated lookups.
    ///
    /// Developers may register their own managers manually if needed.
    /// This is useful when:
    ///     • A custom manager needs to be used before Despawn's automatic discovery.
    ///     • Multiple child managers inherit from an abstract base manager.
    ///       (Example: UIManager → ScreenManager / HUDManager)
    ///       Despawn will always pick the FIRST accessible base type (UIManager),
    ///       unless the developer registers a specific child manager earlier.
    ///
    /// Important notes:
    ///     • Despawn is a sealed class and cannot be inherited or modified.
    ///     • Spawn’s methods work only with the corresponding manager types
    ///       (UIManager, AudioManager, ScreenManager, TaskManager, HapticManager, EnumeratorManager & ..).
    ///     • When scenes change, World scope clears all managers. Despawn will also
    ///       lose cached references and re-discover managers on the next request.
    ///       Developers must ensure their managers are available in World before
    ///       calling Despawn.
    ///
    /// To manually register a manager early:
    ///     Game.World.SpawnService.AddManager(customManager);
    /// This guarantees Despawn will use the developer‑provided manager instead of
    /// auto-discovering one.
    ///
    /// Overall, Despawn provides a fast, centralized, and manager‑aware spawning
    /// system that intelligently reuses previously found managers and adapts to
    /// scene changes.
    /// </summary>
    public sealed class Despawn : IService
    {
        private static Despawn Ins
        {
            get
            {
                var CacheInstance = Game.GetService<Despawn>(false);
                if (CacheInstance == null)
                {
                    CacheInstance = new Despawn();
                    Game.RegisterService(CacheInstance, null);
                }
                return CacheInstance;
            }
        }
        private Dictionary<System.Type, IGameManager> Managers;




        public Despawn()
        {
            Managers = new();
        }
        public Despawn(Dictionary<System.Type, IGameManager> DefaultManager)
        {
            Managers = DefaultManager;
        }


        // Implement IService Interface
        public object GetServiceClass() => this;
        void IService.Created(object Author)
        {
        }
        void IService.ChangingWorld(World NewWorld)
        {
            Managers.Clear();
        }
        void IService.Deleted(object Author)
        {
            Managers.Clear();
        }

        // Public Functions
        public void AddManager(IGameManager manager)
        {
            System.Type ManagerType = manager.GetManagerClass().GetType();
            if (Managers.ContainsKey(ManagerType))
            {
                Managers.Add(ManagerType, manager);
            }
            else
            {
                Debug.LogWarning($"{Ins}: Manager of type '{ManagerType.Name}' already exists. Skipping add.");
            }
        }


        // Public Functions
        private static T GetManager<T>() where T : MonoBehaviour
        {
            System.Type type = typeof(T);
            if (Ins.Managers.ContainsKey(type))
            {
                if (Ins.Managers[type].GetManagerClass() is T manager)
                {
                    return manager;
                }
            }
            else
            {
                var manager = Game.World.FindManager<T>();
                if (manager is IGameManager provider)
                {
                    Ins.Managers.Add(type, provider);
                    return manager;
                }
            }

            Debug.LogError($"{Ins}:Failed to spawn, Manager({typeof(T).Name}) is not available.");
            return null;
        }



        // UI
        public static bool Widget(string Name, Object spawner = null, bool debug = true)
        {
            if (GetManager<UIManager>() == null)
            {
                if (debug)
                    Debug.LogWarning("Despawn UIManager is not available.");
                return false;
            }

            return GetManager<UIManager>().RemoveLayer(Name, spawner);
        }
        public static bool Widget(MonoBehaviour Comp, Object spawner = null, bool debug = true)
        {
            if (GetManager<UIManager>() == null)
            {
                if (debug)
                    Debug.LogWarning("Despawn UIManager is not available.");
                return false;
            }

            return GetManager<UIManager>().RemoveLayer(Comp, spawner);
        }

        // Prefab
        public static bool GameObject(GameObject target, Object spawner = null, bool debug = true)
        {
            if (target != null)
            {
                if (spawner != null)
                {
                    target.InvokeDespawnEvent(spawner);
                }
                Object.Destroy(target);
                return true;
            }
            else
            {
                if (debug)
                    Debug.LogWarning("Despawn target is not available.");
                return false;
            }
        }

        // Pool
        public static bool Pool<T>(PoolAsset<T> asset, bool debug = true) where T : Component
        {
            if (asset is IPoolDespawner<T> pooler)
            {
                pooler.Despawn();
                return true;
            }
            else
            {
                if (debug)
                    Debug.LogWarning("PoolAsset does not implement IPoolDespawner.");
                return false;
            }
        }
        public static bool Pool<T>(PoolAsset<T> asset, T target, bool debug = true) where T : Component
        {
            if (asset is IPoolDespawner<T> pooler)
            {
                pooler.Despawn(target);
                return true;
            }
            else
            {
                if (debug)
                    Debug.LogWarning("PoolAsset does not implement IPoolDespawner.");
                return false;
            }
        }

        // Task
        public static bool Task(object TaskObj, Object author, bool debug = true)
        {
            if (GetManager<TaskManager>() == null)
            {
                if (debug)
                    Debug.LogWarning("TaskManager is not available.");
                return false;
            }

            if (TaskObj is ITask task)
            {
                if (GetManager<TaskManager>().IsValid(task))
                {
                    GetManager<TaskManager>().Remove(task, author);
                    return true;
                }
                else
                {
                    if (debug)
                        Debug.LogWarning($"Task Not Found!");
                    return false;
                }
            }
            else
            {
                if (debug)
                    Debug.LogWarning($"Your Object should have {typeof(ITask)} Interfave");
                return false;
            }
        }

        // Enumerator
        public static bool Coroutine(Coroutine coroutine, bool debug = true)
        {
            if (GetManager<EnumeratorManager>() == null)
            {
                if (debug)
                    Debug.LogWarning("TaskManager is not available.");
                return false;
            }

            GetManager<EnumeratorManager>().Stop(coroutine);
            return true;
        }

        // Haptic
        public static bool Haptic(IHapticProvider provider, bool debug = true)
        {
            if (GetManager<HapticManager>() == null)
            {
                if (debug)
                    Debug.LogWarning("HapticManager is not available.");
                return false;
            }
            return GetManager<HapticManager>().Demolish(provider);
        }

        // Debug
        public static bool Printer(IPrint printer, bool debug = true)
        {
            if (GetManager<PrintManager>() == null)
            {
                if (debug)
                    Debug.LogWarning("Despawn PrintManager is not available.");
                return false;
            }
            return GetManager<PrintManager>().RemoveStatic(printer);
        }
        public static bool Button(IButton button, bool debug = true)
        {
            if (GetManager<DebugManager>() == null)
            {
                if (debug)
                    Debug.LogWarning("Despawn DebugManager is not available.");
                return false;
            }
            return GetManager<DebugManager>().Remove(button);
        }


    }
}