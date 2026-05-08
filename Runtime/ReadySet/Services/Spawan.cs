using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace RealMethod
{
    /// <summary>
    /// Spawn is a sealed static class used to instantiate gameplay objects such as
    /// prefabs, UI elements, audio sources, and other spawnable types.
    ///
    /// This class automatically locates the required manager instances from the
    /// World scope. When a new spawn request is made, Spawn will:
    ///     1) Look for a cached manager of the requested type.
    ///     2) If missing, it will attempt to find the manager inside World.
    ///     3) The found manager is cached internally for reuse to avoid repeated lookups.
    ///
    /// Developers may register their own managers manually if needed.
    /// This is useful when:
    ///     • A custom manager needs to be used before Spawn's automatic discovery.
    ///     • Multiple child managers inherit from an abstract base manager.
    ///       (Example: UIManager → ScreenManager / HUDManager)
    ///       Spawn will always pick the FIRST accessible base type (UIManager),
    ///       unless the developer registers a specific child manager earlier.
    ///
    /// Important notes:
    ///     • Spawn is a sealed class and cannot be inherited or modified.
    ///     • Spawn’s methods work only with the corresponding manager types
    ///       (UIManager, AudioManager, ScreenManager, TaskManager, HapticManager, EnumeratorManager & ..).
    ///     • When scenes change, World scope clears all managers. Spawn will also
    ///       lose cached references and re-discover managers on the next request.
    ///       Developers must ensure their managers are available in World before
    ///       calling Spawn.
    ///
    /// To manually register a manager early:
    ///     Game.World.SpawnService.AddManager(customManager);
    /// This guarantees Spawn will use the developer‑provided manager instead of
    /// auto-discovering one.
    ///
    /// Overall, Spawn provides a fast, centralized, and manager‑aware spawning
    /// system that intelligently reuses previously found managers and adapts to
    /// scene changes.
    /// </summary>
    public sealed class Spawn : IBridge, System.IDisposable, IInspectorInfo
    {
        private static Spawn Ins => CacheInstance.Value;
        private static System.Lazy<Spawn> CacheInstance = new System.Lazy<Spawn>(() => new Spawn());

        private Dictionary<System.Type, IGameManager> Managers;


        public Spawn()
        {
            Game.Bridge.Bind(this);
            Managers = new();
        }

        // Implement IBridge Interface
        void IBridge.OnWorldChanged(World world)
        {
            Managers.Clear();
        }
        // Implement IDisposable Interface
        void System.IDisposable.Dispose()
        {
            Game.Bridge.Unbind(this);
            Managers.Clear();
        }
#if UNITY_EDITOR
        // Implement IInspectorInfo Interface
        string IInspectorInfo.GetInfo()
        {
            return $"Managers ({Managers.Count})";
        }
#endif

        // Public Functions
        public void AddManager(IGameManager manager)
        {
            System.Type ManagerType = manager.Component.GetType();
            if (Managers.ContainsKey(ManagerType))
            {
                Managers.Add(ManagerType, manager);
            }
            else
            {
                Debug.LogWarning($"{Ins}: Manager of type '{ManagerType.Name}' already exists. Skipping add.");
            }
        }

        // Private Functions
        private static T GetManager<T>() where T : Component, IGameManager
        {
            System.Type type = typeof(T);
            if (Ins.Managers.ContainsKey(type))
            {
                if (Ins.Managers[type].Component is T manager)
                {
                    return manager;
                }
            }
            else
            {
                if (Game.World.TryFindGameManager(out T manager))
                {
                    Ins.Managers.Add(type, manager);
                    return manager;
                }
            }

            Debug.LogError($"{Ins}:Failed to spawn, Manager({typeof(T).Name}) is not available. [Note:Manger should be in WorldScope or add manualy]");
            return null;
        }

        // UI
        public static T Widget<T>(string Name, Object spawner = null) where T : MonoBehaviour
        {
            if (GetManager<UIManager>() != null)
            {
                return GetManager<UIManager>().CreateLayer<T>(Name, spawner);
            }
            else
            {
                Debug.LogWarning($" {Ins}: UIManager is not available.");
                return null;
            }
        }
        public static T Widget<T>(VisualTreeAsset UIAsset, string Name, Object spawner = null) where T : MonoBehaviour
        {
            if (GetManager<UIManager>() != null)
            {
                return GetManager<UIManager>().CreateLayer<T>(Name, UIAsset, spawner);
            }
            else
            {
                Debug.LogWarning($" {Ins}: UIManager is not available.");
                return null;
            }
        }
        public static GameObject Widget(UPrefab Prefab, string Name, Object spawner = null)
        {
            if (GetManager<UIManager>() != null)
            {
                //if(Prefab)
                return GetManager<UIManager>().AddLayer(Name, Prefab, spawner);
            }
            else
            {
                Debug.LogWarning($" {Ins}: UIManager is not available.");
                return null;
            }
        }
        public static T Widget<T>(UPrefab Prefab, string Name, Object spawner = null) where T : MonoBehaviour
        {
            if (GetManager<UIManager>() != null)
            {
                return GetManager<UIManager>().AddLayer<T>(Name, Prefab, spawner);
            }
            else
            {
                Debug.LogWarning($" {Ins}: UIManager is not available.");
                return null;
            }
        }
        public static UIDocument UIDoc(string Name, VisualTreeAsset UIAsset)
        {
            if (GetManager<UIManager>() != null)
            {
                return GetManager<UIManager>().CreateLayer(Name, UIAsset);
            }
            else
            {
                Debug.LogWarning($" {Ins}: UIManager is not available.");
                return null;
            }
        }

        // Screen
        public static void Message(string message)
        {
            if (GetManager<ScreenManager>() != null)
            {
                if (GetManager<ScreenManager>().Informer != null)
                {
                    GetManager<ScreenManager>().Informer.Popup(message);
                }
                else
                {
                    Debug.LogWarning($"ScreenMaanger need to set Informer");
                }
            }
            else
            {
                Debug.LogWarning($" {Ins}: ScreenManager is not available.");
            }
        }
        public static void Message(string message, float duration)
        {
            if (GetManager<ScreenManager>() != null)
            {
                if (GetManager<ScreenManager>().Informer != null)
                {
                    GetManager<ScreenManager>().Informer.Popup(message, duration);
                }
                else
                {
                    Debug.LogWarning($"ScreenMaanger need to set Informer");
                }
            }
            else
            {
                Debug.LogWarning($" {Ins}: ScreenManager is not available.");
            }
        }

        // Sound
        public static AudioSource Sound3D(AudioClip clip, Vector3 location, Transform parent = null, AudioMixerGroup group = null, float rolloffDistanceMin = 1f, bool loop = false, float pauseTime = 0, bool autoDestroy = true)
        {
            if (GetManager<AudioManager>() != null)
            {
                return GetManager<AudioManager>().PlaySound(clip, location, parent, group, rolloffDistanceMin, loop, pauseTime, autoDestroy);
            }
            else
            {
                GameObject AudioObject = new GameObject();
                AudioObject.name = "Audio_" + clip.name;
                AudioSource source = AudioObject.AddComponent<AudioSource>();
                source.clip = clip;
                source.outputAudioMixerGroup = group;
                source.spatialBlend = 0;
                source.minDistance = rolloffDistanceMin;
                source.transform.SetParent(parent != null ? parent : Game.World.transform);
                AudioObject.transform.localPosition = location;
                source.Play();
                if (autoDestroy)
                {
                    AudioObject.AddComponent<DestroyAfterDelay, float>(clip.length);
                }
                else
                {
                    Debug.LogWarning("for loop or puaseTime , you need to use AudioManager");
                }
                return source;
            }
        }
        public static AudioSource Sound3D(AudioClip clip, Vector3 location, Transform parent = null, bool autoDestroy = true)
        {
            return Sound3D(clip, location, parent, null, 1f, false, 0f, autoDestroy);
        }
        public static AudioSource Sound2D(AudioClip clip, AudioMixerGroup group = null, float rolloffDistanceMin = 1f, bool loop = false, float pauseTime = 0, bool autoDestroy = true)
        {
            if (GetManager<AudioManager>() != null)
            {
                return GetManager<AudioManager>().PlaySound2D(clip, group, 1, loop, pauseTime, autoDestroy);
            }
            else
            {
                GameObject AudioObject = new GameObject();
                AudioObject.name = "Audio_" + clip.name;
                AudioObject.transform.SetParent(Game.World.transform);
                AudioSource source = AudioObject.AddComponent<AudioSource>();
                source.clip = clip;
                source.outputAudioMixerGroup = group;
                source.spatialBlend = 0;
                source.minDistance = rolloffDistanceMin;
                source.Play();
                if (autoDestroy)
                {
                    AudioObject.AddComponent<DestroyAfterDelay, float>(clip.length);
                }
                else
                {
                    Debug.LogWarning("for loop or puaseTime , you need to use AudioManager");
                }
                return source;
            }
        }
        public static AudioSource Sound2D(AudioClip clip, bool autoDestroy = true)
        {
            return Sound2D(clip, null, 1f, false, 0f, autoDestroy);
        }

        // Clone 
        public static T Clone<T>(T original, Object spawner = null) where T : Object
        {
            if (spawner != null)
            {
                var target = Object.Instantiate(original);
                target.InvokeSpawnEvent(spawner);
                return target;
            }
            else
            {
                return Object.Instantiate(original);
            }
        }
        public static T Clone<T>(T original, Transform parent, Object spawner = null) where T : Object
        {
            if (spawner != null)
            {
                var target = Object.Instantiate(original, parent);
                target.InvokeSpawnEvent(spawner);
                return target;
            }
            else
            {
                return Object.Instantiate(original, parent);
            }
        }
        public static T Clone<T>(T original, Vector3 position, Quaternion rotation, Object spawner = null) where T : Object
        {
            if (spawner != null)
            {
                var target = Object.Instantiate(original, position, rotation);
                target.InvokeSpawnEvent(spawner);
                return target;
            }
            else
            {
                return Object.Instantiate(original, position, rotation);
            }
        }
        public static T Clone<T>(T original, Transform parent, bool worldPositionStays, Object spawner = null) where T : Object
        {
            if (spawner != null)
            {
                var target = Object.Instantiate(original, parent, worldPositionStays);
                target.InvokeSpawnEvent(spawner);
                return target;
            }
            else
            {
                return Object.Instantiate(original, parent, worldPositionStays);
            }
        }

        // Prefab
        public static GameObject Prefab(PrefabCore prefab, Object spawner = null)
        {
            if (spawner != null)
            {
                GameObject target = Object.Instantiate<GameObject>(prefab, Game.World.transform);
                target.InvokeSpawnEvent(spawner);
                return target;
            }
            else
            {
                return Object.Instantiate<GameObject>(prefab, Game.World.transform);
            }
        }
        public static GameObject Prefab(PrefabCore prefab, Transform parent, Object spawner = null)
        {
            if (spawner != null)
            {
                GameObject target = Object.Instantiate<GameObject>(prefab, parent);
                target.InvokeSpawnEvent(spawner);
                return target;
            }
            else
            {
                return Object.Instantiate<GameObject>(prefab, parent);
            }
        }
        public static GameObject Prefab(PrefabCore prefab, Transform parent, bool instantiateInWorldSpace, Object spawner = null)
        {
            if (spawner != null)
            {
                GameObject target = Object.Instantiate<GameObject>(prefab, parent, instantiateInWorldSpace);
                target.InvokeSpawnEvent(spawner);
                return target;
            }
            else
            {
                return Object.Instantiate<GameObject>(prefab, parent, instantiateInWorldSpace);
            }
        }
        public static GameObject Prefab(PrefabCore prefab, Vector3 position, Vector3 rotation, Object spawner = null)
        {
            if (spawner != null)
            {
                GameObject target = Object.Instantiate<GameObject>(prefab, position, Quaternion.Euler(rotation));
                target.InvokeSpawnEvent(spawner);
                return target;
            }
            else
            {
                return Object.Instantiate<GameObject>(prefab, position, Quaternion.Euler(rotation));
            }
        }
        public static GameObject Prefab(PrefabCore prefab, Vector3 position, Quaternion rotation, Object spawner = null)
        {
            if (spawner != null)
            {
                GameObject target = Object.Instantiate<GameObject>(prefab, position, rotation);
                target.InvokeSpawnEvent(spawner);
                return target;
            }
            else
            {
                return Object.Instantiate<GameObject>(prefab, position, rotation);
            }
        }
        public static GameObject Prefab(PrefabCore prefab, Vector3 position, Object spawner = null)
        {
            if (spawner != null)
            {
                GameObject target = Object.Instantiate<GameObject>(prefab, position, Quaternion.identity);
                target.InvokeSpawnEvent(spawner);
                return target;
            }
            else
            {
                return Object.Instantiate<GameObject>(prefab, position, Quaternion.identity);
            }
        }
        public static GameObject Prefab(PrefabCore prefab, Vector3 position, Vector3 rotation, Transform parent, Object spawner = null)
        {
            if (spawner != null)
            {
                GameObject target = Object.Instantiate<GameObject>(prefab, position, Quaternion.Euler(rotation), parent);
                target.InvokeSpawnEvent(spawner);
                return target;
            }
            else
            {
                return Object.Instantiate<GameObject>(prefab, position, Quaternion.Euler(rotation), parent);
            }
        }
        public static GameObject Prefab(PrefabCore prefab, UnityEngine.SceneManagement.Scene scene, Object spawner = null)
        {
            if (spawner != null)
            {
                GameObject target = Object.Instantiate(prefab, scene) as GameObject;
                target.InvokeSpawnEvent(spawner);
                return target;
            }
            else
            {
                return Object.Instantiate(prefab, scene) as GameObject;
            }

        }
        public static T Prefab<T>(PrefabCore<T> prefab, Object spawner = null) where T : Component
        {
            if (spawner != null)
            {
                T target = Object.Instantiate(prefab.GetMainComponent());
                target.InvokeSpawnEvent(spawner);
                return target;
            }
            else
            {
                return Object.Instantiate(prefab.GetMainComponent());
            }
        }
        public static T Prefab<T>(PrefabCore<T> prefab, Transform parent, Object spawner = null) where T : Component
        {
            if (spawner != null)
            {
                T target = Object.Instantiate(prefab.GetMainComponent(), parent);
                target.InvokeSpawnEvent(spawner);
                return target;
            }
            else
            {
                return Object.Instantiate(prefab.GetMainComponent(), parent);
            }
        }
        public static T Prefab<T>(PrefabCore<T> prefab, Vector3 position, Quaternion rotation, Object spawner = null) where T : Component
        {
            if (spawner != null)
            {
                T target = Object.Instantiate(prefab.GetMainComponent(), position, rotation);
                target.InvokeSpawnEvent(spawner);
                return target;
            }
            else
            {
                return Object.Instantiate(prefab.GetMainComponent(), position, rotation);
            }
        }
        public static T Prefab<T>(PrefabCore<T> prefab, Transform parent, bool worldPositionStays, Object spawner = null) where T : Component
        {
            if (spawner != null)
            {
                T target = Object.Instantiate(prefab.GetMainComponent(), parent, worldPositionStays);
                target.InvokeSpawnEvent(spawner);
                return target;
            }
            else
            {
                return Object.Instantiate(prefab.GetMainComponent(), parent, worldPositionStays);
            }
        }

        // Pool
        public static T Pool<T>(PoolAsset<T> asset) where T : Component
        {
            if (asset is IPoolSpawner<T> pooler)
            {
                return pooler.Spawn();
            }
            else
            {
                Debug.LogWarning("PoolAsset does not implement IPoolSpawner.");
                return null;
            }
        }
        public static T Pool<T>(PoolAsset<T> asset, Vector3 location) where T : Component
        {
            if (asset is IPoolSpawner<T> pooler)
            {
                return pooler.Spawn(location);
            }
            else
            {
                Debug.LogWarning("PoolAsset does not implement IPoolSpawner.");
                return null;
            }
        }
        public static T Pool<T>(PoolAsset<T> asset, Vector3 location, Quaternion rotation) where T : Component
        {
            if (asset is IPoolSpawner<T> pooler)
            {
                return pooler.Spawn(location, rotation);
            }
            else
            {
                Debug.LogWarning("PoolAsset does not implement IPoolSpawner.");
                return null;
            }
        }
        public static T Pool<T>(PoolAsset<T> asset, Vector3 location, Quaternion rotation, Vector3 scale) where T : Component
        {
            if (asset is IPoolSpawner<T> pooler)
            {
                return pooler.Spawn(location, rotation, scale);
            }
            else
            {
                Debug.LogWarning("PoolAsset does not implement IPoolSpawner.");
                return null;
            }
        }

        // Effect
        public static EffectPlayer Effect(EPrefab prefab, Object spawner = null)
        {
            return Prefab(prefab, spawner);
        }
        public static EffectPlayer Effect(EPrefab prefab, Vector3 location, Vector3 rotation, Object spawner = null)
        {
            return Prefab(prefab, location, Quaternion.Euler(rotation), spawner);
        }
        public static EffectPlayer Effect(EPrefab prefab, Transform parent, Object spawner = null)
        {
            return Prefab(prefab, parent, spawner);
        }

        // Haptic
        public static IHapticProvider Haptic(HapticConfig config)
        {
            if (GetManager<HapticManager>() != null)
            {
                return GetManager<HapticManager>().Produce(config);
            }
            else
            {
                Debug.LogWarning($" {Ins}: HapticManager is not available.");
                return null;
            }
        }

        // Particle
        public static ParticleSystem Particle(PSPrefab prefab, Vector3 location, Vector3 rotation, Object spawner = null)
        {
            return Prefab(prefab, location, Quaternion.Euler(rotation), spawner);
        }
        public static ParticleSystem Particle(PSPrefab prefab, Transform parent, Object spawner = null)
        {
            return Prefab(prefab, parent, spawner);
        }

        // Code
        public static T Component<T>(GameObject target, Object spawner = null) where T : MonoBehaviour
        {
            if (target)
            {
                var result = target.AddComponent<T>();
                if (spawner != null)
                {
                    result.InvokeSpawnEvent(spawner);
                }
                return target.AddComponent<T>();
            }
            else
            {
                Debug.LogWarning($" {Ins}: target is not valid.");
                return null;
            }

        }
        public static T Class<T>(System.Type classType)
        {
            if (classType == null)
            {
                Debug.LogWarning($" {Ins}: ClassType is not valid!");
                return default;
            }
            return (T)System.Activator.CreateInstance(classType);
        }

        // Realmethod
        public static T Command<T>(CPrefab prefab, MonoBehaviour owner) where T : Command
        {
            if (owner == null)
            {
                Debug.LogWarning($" {Ins}: Owner or Author is not available.");
                return null;
            }
            GameObject SpawnedObject = Object.Instantiate<GameObject>(prefab, owner.transform);
            T TargetCommand = SpawnedObject.GetComponent<T>();
            if (!TargetCommand.GetComponent<ICommand>().Initiate(owner))
            {
                Debug.LogWarning($"Spawn Command Breack: Initiation failed for command '{typeof(T).Name}' on '{prefab.NameID}'.");
            }
            return TargetCommand;
        }
        public static T Asset<T>(Object spawner = null) where T : PrimitiveAsset
        {
            var target = ScriptableObject.CreateInstance<T>();
            target.InvokeSpawnEvent(spawner);
            target.name = $"{typeof(T).Name}_Instance";

            return target;
        }
        public static PrimitiveAsset Asset(System.Type type, Object spawner = null)
        {
            if (type == null)
                throw new System.ArgumentNullException(nameof(type));

            if (!typeof(PrimitiveAsset).IsAssignableFrom(type))
                throw new System.ArgumentException(
                    $"Type {type.Name} must inherit from PrimitiveAsset.");

            PrimitiveAsset target = (PrimitiveAsset)ScriptableObject.CreateInstance(type);
            target.InvokeSpawnEvent(spawner);
            target.name = $"{type.Name}_Instance";

            return target;
        }
        // Task
        public static T Task<T, F>(F Task, bool AutoStart = false) where T : IHandle where F : class
        {
            TaskManager<T> manager = GetManager<TaskManager<T>>();
            if (manager == null)
                return default;
            return manager.Create(Task, AutoStart);
        }

        // Enumerator
        public static Coroutine Coroutine(IEnumerator routine)
        {
            if (GetManager<EnumeratorManager>() != null)
            {
                return GetManager<EnumeratorManager>().Run(routine);
            }
            else
            {
                Debug.LogWarning($" {Ins}: EnumeratorManager is not available.");
                return null;
            }
        }
        public static ICoroutineTask CoroutineTask(IEnumerator routine)
        {
            if (GetManager<EnumeratorManager>() != null)
            {
                return GetManager<EnumeratorManager>().StartTask(routine);
            }
            else
            {
                Debug.LogWarning($" {Ins}: EnumeratorManager is not available.");
                return null;
            }
        }

        // Other
        public static GameObject Empty(string name)
        {
            return new GameObject(name);
        }
        public static MeshRenderer Mesh(Mesh geometry)
        {
            GameObject emptyobject = new GameObject(geometry.name);
            emptyobject.AddComponent<MeshFilter>().mesh = geometry;
            return emptyobject.AddComponent<MeshRenderer>();
        }
        public static MeshRenderer Mesh(Mesh geometry, Vector3 location)
        {
            MeshRenderer result = Mesh(geometry);
            result.transform.position = location;
            return result;
        }
        public static AudioSource Audio(AudioClip clip)
        {
            GameObject emptyobject = new GameObject(clip.name);
            AudioSource source = emptyobject.AddComponent<AudioSource>();
            source.clip = clip;
            if (GetManager<AudioManager>() != null)
            {
                source.outputAudioMixerGroup = GetManager<AudioManager>().DefaultGroup;
                emptyobject.transform.SetParent(GetManager<AudioManager>().transform);
            }
            return source;
        }

    }
}

