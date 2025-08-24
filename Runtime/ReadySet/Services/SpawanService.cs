using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

namespace RealMethod
{
    public sealed class Spawn : Service
    {
        private static Spawn CacheInstance = null;
        public static Spawn instance
        {
            get
            {
                if (CacheInstance != null)
                {
                    return CacheInstance;
                }
                else
                {
                    if (Game.TryFindService(out Spawn CacheInstance))
                    {
                        return CacheInstance;
                    }
                    CacheInstance = Game.AddService<Spawn>(null);
                    return CacheInstance;
                }
            }
        }

        public Despawn GameDespawn;
        public AudioManager GameAudio;
        public UIManager GameUI;
        public ScreenManager GameScreen;
        public TaskManager GameTask;
        public HapticManager GameHaptic;
        public EnumeratorManager GameEnumerator;

        // Base Service
        protected override void OnStart(object Author)
        {
            GameDespawn = new Despawn();
        }
        protected override void OnNewWorld()
        {
        }
        protected override void OnEnd(object Author)
        {
            GameDespawn = null;
            GameAudio = null;
            GameUI = null;
            CacheInstance = null;
        }

        // Public Functions
        public void BringManager(IGameManager manager)
        {
            MonoBehaviour TargetManager = manager.GetManagerClass();
            switch (TargetManager) // **Order is very important**
            {
                case ScreenManager screenmanager: // Bring ScreenManager
                    if (GameScreen == null)
                    {
                        GameScreen = screenmanager;
                    }
                    else
                    {
                        Debug.LogError($"Spawn Service already have {typeof(ScreenManager)} Cant Enter this {screenmanager}");
                    }
                    break;
                case TaskManager taskmanager: // Bring TaskManager
                    if (GameTask == null)
                    {
                        GameTask = taskmanager;
                    }
                    else
                    {
                        Debug.LogError($"Spawn Service already have {typeof(TaskManager)} Cant Enter this {taskmanager}");
                    }
                    break;
                case EnumeratorManager enumeratormanager:
                    if (GameEnumerator == null)
                    {
                        GameEnumerator = enumeratormanager;
                    }
                    else
                    {
                        Debug.LogError($"Spawn Service already have {typeof(EnumeratorManager)} Cant Enter this {enumeratormanager}");
                    }
                    break;
                case HapticManager hapticmanager:
                    if (GameHaptic == null)
                    {
                        GameHaptic = hapticmanager;
                    }
                    else
                    {
                        Debug.LogError($"Spawn Service already have {typeof(HapticManager)} Cant Enter this {hapticmanager}");
                    }
                    break;
                case AudioManager audiomanager: // Brind AudioManager
                    if (GameAudio == null)
                    {
                        GameAudio = audiomanager;
                    }
                    else
                    {
                        Debug.LogError($"Spawn Service already have {typeof(AudioManager)} Cant Enter this {audiomanager}");
                    }
                    break;
                case UIManager uIManager: // Brind UIManager
                    if (GameUI == null)
                    {
                        GameUI = uIManager;
                    }
                    else
                    {
                        Debug.LogError($"Spawn Service already have {typeof(UIManager)} Cant Enter this {uIManager}");
                    }
                    break;
                default:
                    Debug.LogWarning($"{TargetManager.gameObject}:Manager not define in SpawnService");
                    break;
            }
        }


        // UI
        public static T Widget<T>(string Name, MonoBehaviour Owner) where T : MonoBehaviour
        {
            if (instance.GameUI != null)
            {
                return instance.GameUI.CreateLayer<T>(Name, Owner);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }
        public static T Widget<T>(string Name) where T : MonoBehaviour
        {
            if (instance.GameUI != null)
            {
                return instance.GameUI.CreateLayer<T>(Name);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }
        public static UIDocument Widget(string Name, VisualTreeAsset UIAsset)
        {
            if (instance.GameUI != null)
            {
                return instance.GameUI.CreateLayer(Name, UIAsset);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }
        public static T Widget<T>(string Name, VisualTreeAsset UIAsset, MonoBehaviour Owner) where T : MonoBehaviour
        {
            if (instance.GameUI != null)
            {
                return instance.GameUI.CreateLayer<T>(Name, UIAsset, Owner);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }
        public static T Widget<T>(string Name, VisualTreeAsset UIAsset) where T : MonoBehaviour
        {
            if (instance.GameUI != null)
            {
                return instance.GameUI.CreateLayer<T>(Name, UIAsset);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }
        public static GameObject Widget(string Name, UPrefab Prefab, MonoBehaviour Owner)
        {
            if (instance.GameUI != null)
            {
                return instance.GameUI.AddLayer(Name, Prefab, Owner);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }
        public static GameObject Widget(string Name, UPrefab Prefab)
        {
            if (instance.GameUI != null)
            {
                return instance.GameUI.AddLayer(Name, Prefab);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }
        public static T Widget<T>(string Name, UPrefab Prefab, MonoBehaviour Owner) where T : MonoBehaviour
        {
            if (instance.GameUI != null)
            {
                return instance.GameUI.AddLayer<T>(Name, Prefab, Owner);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }
        public static T Widget<T>(string Name, UPrefab Prefab) where T : MonoBehaviour
        {
            if (instance.GameUI != null)
            {
                return instance.GameUI.AddLayer<T>(Name, Prefab);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }

        // Screen
        public static void Message(string message)
        {
            if (instance.GameScreen != null)
            {
                if (instance.GameScreen.Informer != null)
                {
                    instance.GameScreen.Informer.Popup(message);
                }
                else
                {
                    Debug.LogWarning($"ScreenMaanger need to set Informer");
                }
            }
            else
            {
                Debug.LogWarning($" {instance}: ScreenManager is not available.");
            }
        }
        public static void Message(string message, float duration)
        {
            if (instance.GameScreen != null)
            {
                if (instance.GameScreen.Informer != null)
                {
                    instance.GameScreen.Informer.Popup(message, duration);
                }
                else
                {
                    Debug.LogWarning($"ScreenMaanger need to set Informer");
                }
            }
            else
            {
                Debug.LogWarning($" {instance}: ScreenManager is not available.");
            }
        }

        // Sound
        public static AudioSource Sound3D(AudioClip clip, Vector3 location, Transform parent = null, AudioMixerGroup group = null, float rolloffDistanceMin = 1f, bool loop = false, float pauseTime = 0, bool autoDestroy = true)
        {
            if (instance.GameAudio != null)
            {
                return instance.GameAudio.PlaySound(clip, location, parent, group, rolloffDistanceMin, loop, pauseTime, autoDestroy);
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
            if (instance.GameAudio != null)
            {
                return instance.GameAudio.PlaySound2D(clip, group, 1, loop, pauseTime, autoDestroy);
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

        // Cloning 
        public static T Clone<T>(T original) where T : Object
        {
            return Object.Instantiate(original);
        }
        public static T Clone<T>(T original, Transform parent) where T : Object
        {
            return Object.Instantiate(original, parent);
        }
        public static T Clone<T>(T original, Vector3 position, Quaternion rotation) where T : Object
        {
            return Object.Instantiate(original, position, rotation);
        }
        public static T Clone<T>(T original, Transform parent, bool worldPositionStays) where T : Object
        {
            return Object.Instantiate(original, parent, worldPositionStays);
        }

        // Prefab
        public static GameObject Prefab(PrefabCore prefab)
        {
            return Object.Instantiate(prefab.asset, Game.World.transform);
        }
        public static GameObject Prefab(PrefabCore prefab, Transform parent)
        {
            return Object.Instantiate(prefab.asset, parent);
        }
        public static GameObject Prefab(PrefabCore prefab, Transform parent, bool instantiateInWorldSpace)
        {
            return Object.Instantiate(prefab.asset, parent, instantiateInWorldSpace);
        }
        public static GameObject Prefab(PrefabCore prefab, Vector3 position, Vector3 rotation)
        {
            return Object.Instantiate(prefab.asset, position, Quaternion.Euler(rotation));
        }
        public static GameObject Prefab(PrefabCore prefab, Vector3 position, Quaternion rotation)
        {
            return Object.Instantiate(prefab.asset, position, rotation);
        }
        public static GameObject Prefab(PrefabCore prefab, Vector3 position)
        {
            return Object.Instantiate(prefab.asset, position, Quaternion.identity);
        }
        public static GameObject Prefab(PrefabCore prefab, Vector3 position, Vector3 rotation, Transform parent)
        {
            return Object.Instantiate(prefab.asset, position, Quaternion.Euler(rotation), parent);
        }
        public static GameObject Prefab(PrefabCore prefab, UnityEngine.SceneManagement.Scene scene)
        {
            return Object.Instantiate(prefab.asset, scene) as GameObject;
        }
        public static T Prefab<T>(PrefabCore<T> prefab) where T : Component
        {
            return Object.Instantiate(prefab.GetSoftClassTarget());
        }
        public static T Prefab<T>(PrefabCore<T> prefab, Transform parent) where T : Component
        {
            return Object.Instantiate(prefab.GetSoftClassTarget(), parent);
        }
        public static T Prefab<T>(PrefabCore<T> prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            return Object.Instantiate(prefab.GetSoftClassTarget(), position, rotation);
        }
        public static T Prefab<T>(PrefabCore<T> prefab, Transform parent, bool worldPositionStays) where T : Component
        {
            return Object.Instantiate(prefab.GetSoftClassTarget(), parent, worldPositionStays);
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
        public static EffectPlayer Effect(EPrefab prefab)
        {
            return Prefab(prefab);
        }
        public static EffectPlayer Effect(EPrefab prefab, Vector3 location, Vector3 rotation)
        {
            return Prefab(prefab, location, Quaternion.Euler(rotation));
        }
        public static EffectPlayer Effect(EPrefab prefab, Transform parent)
        {
            return Prefab(prefab, parent);
        }

        // Haptic
        public static IHapticProvider Haptic(HapticConfig config)
        {
            if (instance.GameHaptic != null)
            {
                return instance.GameHaptic.Produce(config);
            }
            else
            {
                Debug.LogWarning($" {instance}: HapticManager is not available.");
                return null;
            }
        }

        // Particle
        public static ParticleSystem Particle(PSPrefab prefab, Vector3 location, Vector3 rotation)
        {
            return Prefab(prefab, location, Quaternion.Euler(rotation));
        }
        public static ParticleSystem Particle(PSPrefab prefab, Transform parent)
        {
            return Prefab(prefab, parent);
        }

        // Code
        public static T Component<T>(GameObject target) where T : MonoBehaviour
        {
            if (target)
            {
                return target.AddComponent<T>();
            }
            else
            {
                Debug.LogWarning($" {instance}: target is not valid.");
                return null;
            }

        }
        public static T Class<T>() where T : Object, new()
        {
            return new T();
        }

        // Realmethod
        public static T Command<T>(CPrefab prefab, MonoBehaviour owner, Object author) where T : Command
        {
            if (owner == null || author == null)
            {
                Debug.LogWarning($" {instance}: Owner or Author is not available.");
                return null;
            }
            GameObject SpawnedObject = Object.Instantiate(prefab.asset, owner.transform);
            T TargetCommand = SpawnedObject.GetComponent<T>();
            if (!TargetCommand.GetComponent<ICommand>().Initiate(author, owner))
            {
                Debug.LogWarning($"Spawn Command Breack: Initiation failed for command '{typeof(T).Name}' on '{prefab.Name}'.");
            }
            return TargetCommand;
        }
        public static T Data<T>() where T : DataAsset
        {
            return ScriptableObject.CreateInstance<T>();
        }
        public static T File<T>() where T : FileAsset
        {
            return FileAsset.Create<T>();
        }

        // Task
        public static bool Task(object TaskObj, Object author)
        {
            if (instance.GameTask != null)
            {
                if (TaskObj is ITask task)
                {
                    if (!instance.GameTask.IsValid(task))
                    {
                        instance.GameTask.Add(task, author);
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning($"Task already is running");
                        return false;
                    }
                }
                else
                {
                    Debug.LogWarning($"Your Object should have {typeof(ITask)} Interface");
                    return false;
                }
            }
            else
            {
                Debug.LogWarning($" {instance}: TaskManager is not available.");
                return false;
            }
        }

        // Enumerator
        public static Coroutine Coroutine(IEnumerator routine)
        {
            if (instance.GameEnumerator != null)
            {
                return instance.GameEnumerator.Run(routine);
            }
            else
            {
                Debug.LogWarning($" {instance}: EnumeratorManager is not available.");
                return null;
            }
        }
        public static ICoroutineTask CoroutineTask(IEnumerator routine)
        {
            if (instance.GameEnumerator != null)
            {
                return instance.GameEnumerator.StartTask(routine);
            }
            else
            {
                Debug.LogWarning($" {instance}: EnumeratorManager is not available.");
                return null;
            }
        }

        // Other
        public static GameObject New(string name)
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
            if (instance.GameAudio != null)
            {
                source.outputAudioMixerGroup = instance.GameAudio.defaultGroup;
                emptyobject.transform.SetParent(instance.GameAudio.transform);
            }
            return source;
        }

    }

    public sealed class Despawn
    {
        // UI
        public static bool Widget(string Name, bool debug = true)
        {
            if (Spawn.instance.GameUI == null)
            {
                if (debug)
                    Debug.LogWarning("Despawn UIManager is not available.");
                return false;
            }

            return Spawn.instance.GameUI.RemoveLayer(Name);
        }
        public static bool Widget(MonoBehaviour Comp, bool debug = true)
        {
            if (Spawn.instance.GameUI == null)
            {
                if (debug)
                    Debug.LogWarning("Despawn UIManager is not available.");
                return false;
            }

            return Spawn.instance.GameUI.RemoveLayer(Comp);
        }

        // Prefab
        public static bool GameObject(GameObject target, bool debug = true)
        {
            if (target != null)
            {
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
            if (Spawn.instance.GameTask == null)
            {
                if (debug)
                    Debug.LogWarning("TaskManager is not available.");
                return false;
            }

            if (TaskObj is ITask task)
            {
                if (Spawn.instance.GameTask.IsValid(task))
                {
                    Spawn.instance.GameTask.Remove(task, author);
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
            if (Spawn.instance.GameEnumerator == null)
            {
                if (debug)
                    Debug.LogWarning("TaskManager is not available.");
                return false;
            }

            Spawn.instance.GameEnumerator.Stop(coroutine);
            return true;
        }

        // Haptic
        public static bool Haptic(IHapticProvider provider, bool debug = true)
        {
            if (Spawn.instance.GameHaptic == null)
            {
                if (debug)
                    Debug.LogWarning("HapticManager is not available.");
                return false;
            }
            return Spawn.instance.GameHaptic.Demolish(provider);
        }
    }
}

