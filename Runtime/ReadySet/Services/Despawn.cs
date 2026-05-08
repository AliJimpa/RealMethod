using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// Despawn is a sealed static class used to instantiate gameplay objects such as
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
    public sealed class Despawn : Context<Despawn>, IBridge, IInspectorInfo
    {
        private Dictionary<System.Type, IGameManager> GameManagers;
        private Dictionary<System.Type, IGameManager> WorldManagers;

        // Implement IBridge Interface
        void IBridge.OnWorldChanged(World world)
        {
            WorldManagers.Clear();
            Debug.Log("ChangeWorld Spawn");
        }

        protected override void OnBegin()
        {
            // Check if you game not initialized
            if (!Game.IsGameInitialized)
            {
                Debug.LogWarning($"Game doesn't initialized !");
                return;
            }
            Game.Bridge.Bind(this);
            GameManagers = new();
            WorldManagers = new();
            Debug.Log("Begin Spawn");
        }
        protected override void OnEnd()
        {
            Game.Bridge.Unbind(this);
            GameManagers = null;
            WorldManagers = null;
            Debug.Log("End Spawn");
        }

        private static T Get<T>(ScopeTarget context = ScopeTarget.Both) where T : Component, IGameManager
        {
            System.Type type = typeof(T);

            switch (context)
            {
                case ScopeTarget.World:
                    if (Instance.WorldManagers.ContainsKey(type))
                    {
                        return (T)Instance.WorldManagers[type].Component;
                    }
                    else
                    {
                        if (Game.World.TryFindGameManager(out T manager))
                        {
                            Instance.WorldManagers.Add(type, manager);
                            return manager;
                        }
                    }
                    break;
                case ScopeTarget.Game:
                    if (Instance.GameManagers.ContainsKey(type))
                    {
                        return (T)Instance.GameManagers[type].Component;
                    }
                    else
                    {
                        if (Game.Instance.TryFindGameManager(out T manager))
                        {
                            Instance.GameManagers.Add(type, manager);
                            return manager;
                        }
                    }
                    break;
                case ScopeTarget.Both:
                    if (Instance.WorldManagers.ContainsKey(type))
                    {
                        return (T)Instance.WorldManagers[type].Component;
                    }
                    else if (Instance.GameManagers.ContainsKey(type))
                    {
                        return (T)Instance.GameManagers[type].Component;
                    }
                    else if (Game.World.TryFindGameManager(out T manager1))
                    {
                        Instance.WorldManagers.Add(type, manager1);
                        return manager1;
                    }
                    else if (Game.Instance.TryFindGameManager(out T manager2))
                    {
                        Instance.GameManagers.Add(type, manager2);
                        return manager2;
                    }
                    break;
            }

            Debug.LogError($"{Instance}:Failed to spawn, Manager({typeof(T).Name}) is not available. [{context} Context]");
            return null;
        }


#if UNITY_EDITOR
        string IInspectorInfo.GetInfo()
        {
            return $"GameManagers ({GameManagers.Count}) , WorldManagers ({WorldManagers.Count})";
        }
#endif


        // UI
        public static bool Widget(string Name, Object despawner = null, bool debug = true)
        {
            if (Get<UIManager>() == null)
            {
                if (debug)
                    Debug.LogWarning("Despawn UIManager is not available.");
                return false;
            }

            return Get<UIManager>().RemoveLayer(Name, despawner);
        }
        public static bool Widget(MonoBehaviour Comp, Object despawner = null, bool debug = true)
        {
            if (Get<UIManager>() == null)
            {
                if (debug)
                    Debug.LogWarning("Despawn UIManager is not available.");
                return false;
            }

            return Get<UIManager>().RemoveLayer(Comp, despawner);
        }

        // Prefab
        public static bool GameObject(GameObject target, Object despawner = null, bool debug = true)
        {
            if (target != null)
            {
                if (despawner != null)
                {
                    target.InvokeDespawnEvent(despawner);
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

        // Realmethod
        public static bool Asset<T>(T target, Object despawner = null, bool debug = true) where T : PrimitiveAsset
        {
            if (target != null)
            {
                Object.Destroy(target);
                target.InvokeDespawnEvent(despawner);
                return true;
            }
            else
            {
                if (debug)
                    Debug.LogWarning($"Asset is not Valid!");
                return false;
            }
        }


        // Task
        public static bool Task<T>(T task) where T : IHandle
        {
            TaskManager<T> manager = Get<TaskManager<T>>();
            if (manager == null)
                return false;
            return manager.Destroy(task);
        }

        // Enumerator
        public static bool Coroutine(Coroutine coroutine, bool debug = true)
        {
            if (Get<EnumeratorManager>() == null)
            {
                if (debug)
                    Debug.LogWarning("TaskManager is not available.");
                return false;
            }

            Get<EnumeratorManager>().Stop(coroutine);
            return true;
        }

        // Haptic
        public static bool Haptic(IHapticProvider provider, bool debug = true)
        {
            if (Get<HapticManager>() == null)
            {
                if (debug)
                    Debug.LogWarning("HapticManager is not available.");
                return false;
            }
            return Get<HapticManager>().Demolish(provider);
        }


    }
}