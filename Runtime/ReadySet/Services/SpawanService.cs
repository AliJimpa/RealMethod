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
            private set { }
        }

        private Despawn AntiSpawn;
        private AudioManager audioBox;
        private UIManager uIBox;

        // Base Service
        protected override void OnStart(object Author)
        {
            AntiSpawn = new Despawn(uIBox, audioBox);
        }
        protected override void OnNewWorld()
        {
        }
        protected override void OnEnd(object Author)
        {
            AntiSpawn = null;
            audioBox = null;
            uIBox = null;
            CacheInstance = null;
        }

        // Public Functions
        public void BringManager(IGameManager manager)
        {
            // Brind AudioManager
            if (manager.GetManagerClass() is AudioManager audiomanager)
            {
                if (audioBox == null)
                {
                    audioBox = audiomanager;
                }
                else
                {
                    Debug.LogError($"Spawn Service already have AudioManager Cant Enter this {audiomanager}");
                }
            }

            // Brind UIManagerF
            if (manager.GetManagerClass() is UIManager uIManager)
            {
                if (uIBox == null)
                {
                    if (uIManager.IsMaster())
                    {
                        uIBox = uIManager;
                    }
                }
                else
                {
                    Debug.LogError($"Spawn Service already have AudioManager Cant Enter this {uIManager}");
                }
            }
        }


        // UI
        public static T Widget<T>(string Name, MonoBehaviour Owner) where T : MonoBehaviour
        {
            if (instance.uIBox != null)
            {
                return instance.uIBox.CreateLayer<T>(Name, Owner);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }
        public static T Widget<T>(string Name) where T : MonoBehaviour
        {
            if (instance.uIBox != null)
            {
                return instance.uIBox.CreateLayer<T>(Name);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }
        public static UIDocument Widget(string Name, VisualTreeAsset UIAsset)
        {
            if (instance.uIBox != null)
            {
                return instance.uIBox.CreateLayer(Name, UIAsset);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }
        public static T Widget<T>(string Name, VisualTreeAsset UIAsset, MonoBehaviour Owner) where T : MonoBehaviour
        {
            if (instance.uIBox != null)
            {
                return instance.uIBox.CreateLayer<T>(Name, UIAsset, Owner);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }
        public static T Widget<T>(string Name, VisualTreeAsset UIAsset) where T : MonoBehaviour
        {
            if (instance.uIBox != null)
            {
                return instance.uIBox.CreateLayer<T>(Name, UIAsset);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }
        public static GameObject Widget(string Name, WPrefab Prefab, MonoBehaviour Owner)
        {
            if (instance.uIBox != null)
            {
                return instance.uIBox.AddLayer(Name, Prefab, Owner);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }
        public static GameObject Widget(string Name, WPrefab Prefab)
        {
            if (instance.uIBox != null)
            {
                return instance.uIBox.AddLayer(Name, Prefab);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }
        public static T Widget<T>(string Name, WPrefab Prefab, MonoBehaviour Owner) where T : MonoBehaviour
        {
            if (instance.uIBox != null)
            {
                return instance.uIBox.AddLayer<T>(Name, Prefab, Owner);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }
        public static T Widget<T>(string Name, WPrefab Prefab) where T : MonoBehaviour
        {
            if (instance.uIBox != null)
            {
                return instance.uIBox.AddLayer<T>(Name, Prefab);
            }
            else
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return null;
            }
        }

        // Sound
        public static AudioSource Sound2D(AudioClip clip, bool autoDestroy = true)
        {
            if (instance.audioBox != null)
            {
                return instance.audioBox.PlaySound2D(clip, autoDestroy);
            }
            else
            {
                GameObject AudioObject = new GameObject();
                AudioObject.name = "Audio_" + clip.name;
                AudioObject.transform.SetParent(Game.World.transform);
                AudioSource source = AudioObject.AddComponent<AudioSource>();
                source.clip = clip;
                source.spatialBlend = 0;
                source.Play();
                if (autoDestroy)
                {
                    AudioObject.AddComponent<DestroyAfterDelay, float>(clip.length);
                }
                return source;
            }
        }
        public static AudioSource Sound2D(AudioClip clip, AudioMixerGroup group, bool autoDestroy = true)
        {
            if (instance.audioBox != null)
            {
                return instance.audioBox.PlaySound2D(clip, group, autoDestroy);
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
                source.Play();
                if (autoDestroy)
                {
                    AudioObject.AddComponent<DestroyAfterDelay, float>(clip.length);
                }
                return source;
            }
        }
        public static AudioSource Sound3D(AudioClip clip, Vector3 location, bool autoDestroy = true)
        {
            if (instance.audioBox != null)
            {
                return instance.audioBox.PlaySound(clip, location, autoDestroy);
            }
            else
            {
                GameObject AudioObject = new GameObject();
                AudioObject.name = "Audio_" + clip.name;
                AudioObject.transform.position = location;
                AudioSource source = AudioObject.AddComponent<AudioSource>();
                source.clip = clip;
                source.spatialBlend = 0;
                source.Play();
                if (autoDestroy)
                {
                    AudioObject.AddComponent<DestroyAfterDelay, float>(clip.length);
                }
                return source;
            }
        }
        public static AudioSource Sound3D(AudioClip clip, AudioMixerGroup group, Vector3 location, bool autoDestroy = true)
        {
            if (instance.audioBox != null)
            {
                return instance.audioBox.PlaySound(clip, group, location, autoDestroy);
            }
            else
            {
                GameObject AudioObject = new GameObject();
                AudioObject.name = "Audio_" + clip.name;
                AudioObject.transform.position = location;
                AudioSource source = AudioObject.AddComponent<AudioSource>();
                source.clip = clip;
                source.outputAudioMixerGroup = group;
                source.spatialBlend = 0;
                source.Play();
                if (autoDestroy)
                {
                    AudioObject.AddComponent<DestroyAfterDelay, float>(clip.length);
                }
                return source;
            }
        }

        // Generic 
        public static T Obj<T>(T original) where T : Object
        {
            return Object.Instantiate(original);
        }
        public static T Obj<T>(T original, Transform parent) where T : Object
        {
            return Object.Instantiate(original, parent);
        }
        public static T Obj<T>(T original, Vector3 position, Quaternion rotation) where T : Object
        {
            return Object.Instantiate(original, position, rotation);
        }
        public static T Obj<T>(T original, Transform parent, bool worldPositionStays) where T : Object
        {
            return Object.Instantiate(original, parent, worldPositionStays);
        }

        // Prefab
        public static GameObject Prefab(UPrefab prefab)
        {
            return Object.Instantiate(prefab.asset, Game.World.transform);
        }
        public static GameObject Prefab(UPrefab prefab, Transform parent)
        {
            return Object.Instantiate(prefab.asset, parent);
        }
        public static GameObject Prefab(UPrefab prefab, Transform parent, bool instantiateInWorldSpace)
        {
            return Object.Instantiate(prefab.asset, parent, instantiateInWorldSpace);
        }
        public static GameObject Prefab(UPrefab prefab, Vector3 location, Vector3 rotation)
        {
            return Object.Instantiate(prefab.asset, location, Quaternion.Euler(rotation));
        }
        public static GameObject Prefab(UPrefab prefab, Vector3 location, Quaternion rotation)
        {
            return Object.Instantiate(prefab.asset, location, rotation);
        }
        public static GameObject Prefab(UPrefab prefab, Vector3 location)
        {
            return Object.Instantiate(prefab.asset, location, Quaternion.identity);
        }
        public static GameObject Prefab(UPrefab prefab, Vector3 location, Vector3 rotation, Transform parent)
        {
            return Object.Instantiate(prefab.asset, location, Quaternion.Euler(rotation), parent);
        }
        public static GameObject Prefab(UPrefab prefab, UnityEngine.SceneManagement.Scene scene)
        {
            return Object.Instantiate(prefab.asset, scene) as GameObject;
        }
        public static T Prefab<T>(Prefab<T> original) where T : Component
        {
            return Object.Instantiate(original.GetSoftClassTarget());
        }
        public static T Prefab<T>(Prefab<T> original, Transform parent) where T : Component
        {
            return Object.Instantiate(original.GetSoftClassTarget(), parent);
        }
        public static T Prefab<T>(Prefab<T> original, Vector3 position, Quaternion rotation) where T : Component
        {
            return Object.Instantiate(original.GetSoftClassTarget(), position, rotation);
        }
        public static T Prefab<T>(Prefab<T> original, Transform parent, bool worldPositionStays) where T : Component
        {
            return Object.Instantiate(original.GetSoftClassTarget(), parent, worldPositionStays);
        }

        // Particle
        public static ParticleSystem Particle(PPrefab prefab, Vector3 location, Vector3 rotation)
        {
            return Prefab(prefab, location, Quaternion.Euler(rotation));
        }
        public static ParticleSystem Particle(PPrefab prefab, Transform parent)
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
            if (!TargetCommand.GetComponent<ICommandInitiator>().Initiate(author, owner))
            {
                Debug.LogWarning($"Spawn Command Breack: Initiation failed for command '{typeof(T).Name}' on '{prefab.Name}'.");
            }
            return TargetCommand;
        }
        public static T DataAsset<T>() where T : DataAsset
        {
            return ScriptableObject.CreateInstance<T>();
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
            if (instance.audioBox != null)
            {
                source.outputAudioMixerGroup = instance.audioBox.defaultGroup;
                emptyobject.transform.SetParent(instance.audioBox.transform);
            }
            return source;
        }

    }

    public sealed class Despawn
    {
        private static Despawn instance;
        private UIManager uIBox;
        private AudioManager audioBox;

        public Despawn(UIManager ui, AudioManager audio)
        {
            if (instance == null)
            {
                instance = this;
                uIBox = ui;
                audioBox = audio;
            }
        }

        // UI
        public static bool Widget(string Name)
        {
            if (instance == null)
            {
                Debug.LogWarning("Something Wrong!");
                return false;
            }
            if (instance.uIBox == null)
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return false;
            }

            return instance.uIBox.RemoveLayer(Name);
        }
        public static bool Widget(MonoBehaviour Comp)
        {
            if (instance == null)
            {
                Debug.LogWarning("Something Wrong!");
                return false;
            }
            if (instance.uIBox == null)
            {
                Debug.LogWarning($" {instance}: UIManager is not available.");
                return false;
            }

            return instance.uIBox.RemoveLayer(Comp);
        }

        // Prefab
        public static bool GameObject(GameObject target)
        {
            if (target != null)
            {
                Object.Destroy(target);
                return true;
            }
            else
            {
                Debug.LogWarning($" {instance}: target is not available.");
                return false;
            }
        }


    }

}