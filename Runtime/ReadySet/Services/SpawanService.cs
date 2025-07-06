using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

namespace RealMethod
{
    public sealed class Spawn : Service
    {
        private static Spawn instance;
        public static Spawn Service
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }
                else
                {
                    Debug.LogError("For Spawning You need Add SpawnService in Game. [Game Class > GameServiceCreated Method]");
                    return null;
                }
            }
            private set { }
        }

        private Despawn AntiSpawn;
        private AudioManager audioBox;
        private UIManager uIBox;

        public override void Start(object Author)
        {
            if (instance == null)
            {
                instance = this;
                AntiSpawn = new Despawn(uIBox, audioBox);
            }
            else
            {
                Debug.LogError($"SpawnService Is Created befor, You Can't Create Twice! [Author:{Author}]");
                return;
            }
        }
        public override void WorldUpdated()
        {
        }
        public override void End(object Author)
        {
            AntiSpawn = null;
            instance = null;
        }

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
            if (Service.uIBox != null)
            {
                return Service.uIBox.CreateLayer<T>(Name, Owner);
            }
            else
            {
                Debug.LogWarning("Spawn Service: UIManager is not available. Cannot create widget. You Need Mater UI");
                return null;
            }
        }
        public static T Widget<T>(string Name) where T : MonoBehaviour
        {
            if (Service.uIBox != null)
            {
                return Service.uIBox.CreateLayer<T>(Name);
            }
            else
            {
                Debug.LogWarning("Spawn Service: UIManager is not available. Cannot create widget. You Need Mater UI");
                return null;
            }
        }
        public static UIDocument Widget(string Name, VisualTreeAsset UIAsset)
        {
            if (Service.uIBox != null)
            {
                return Service.uIBox.CreateLayer(Name, UIAsset);
            }
            else
            {
                Debug.LogWarning("Spawn Service: UIManager is not available. Cannot create widget. You Need Mater UI");
                return null;
            }
        }
        public static T Widget<T>(string Name, VisualTreeAsset UIAsset, MonoBehaviour Owner) where T : MonoBehaviour
        {
            if (Service.uIBox != null)
            {
                return Service.uIBox.CreateLayer<T>(Name, UIAsset, Owner);
            }
            else
            {
                Debug.LogWarning("Spawn Service: UIManager is not available. Cannot create widget. You Need Mater UI");
                return null;
            }
        }
        public static T Widget<T>(string Name, VisualTreeAsset UIAsset) where T : MonoBehaviour
        {
            if (Service.uIBox != null)
            {
                return Service.uIBox.CreateLayer<T>(Name, UIAsset);
            }
            else
            {
                Debug.LogWarning("Spawn Service: UIManager is not available. Cannot create widget. You Need Mater UI");
                return null;
            }
        }
        public static GameObject Widget(string Name, GameObject Prefab, MonoBehaviour Owner)
        {
            if (Service.uIBox != null)
            {
                return Service.uIBox.AddLayer(Name, Prefab, Owner);
            }
            else
            {
                Debug.LogWarning("Spawn Service: UIManager is not available. Cannot create widget. You Need Mater UI");
                return null;
            }
        }
        public static GameObject Widget(string Name, GameObject Prefab)
        {
            if (Service.uIBox != null)
            {
                return Service.uIBox.AddLayer(Name, Prefab);
            }
            else
            {
                Debug.LogWarning("Spawn Service: UIManager is not available. Cannot create widget. You Need Mater UI");
                return null;
            }
        }
        public static T Widget<T>(string Name, GameObject Prefab, MonoBehaviour Owner) where T : MonoBehaviour
        {
            if (Service.uIBox != null)
            {
                return Service.uIBox.AddLayer<T>(Name, Prefab, Owner);
            }
            else
            {
                Debug.LogWarning("Spawn Service: UIManager is not available. Cannot create widget. You Need Mater UI");
                return null;
            }
        }
        public static T Widget<T>(string Name, GameObject Prefab) where T : MonoBehaviour
        {
            if (Service.uIBox != null)
            {
                return Service.uIBox.AddLayer<T>(Name, Prefab);
            }
            else
            {
                Debug.LogWarning("Spawn Service: UIManager is not available. Cannot create widget. You Need Mater UI");
                return null;
            }
        }

        // Sound
        public static AudioSource Sound2D(AudioClip clip, bool autoDestroy = true)
        {
            if (Service.audioBox != null)
            {
                return Service.audioBox.PlaySound2D(clip, autoDestroy);
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
                    DestroyAfterDelay timedSelfDestroyer = AudioObject.AddComponent<DestroyAfterDelay, float>(clip.length);
                }
                return source;
            }
        }
        public static AudioSource Sound2D(AudioClip clip, AudioMixerGroup group, bool autoDestroy = true)
        {
            if (Service.audioBox != null)
            {
                return Service.audioBox.PlaySound2D(clip, group, autoDestroy);
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
                    DestroyAfterDelay timedSelfDestroyer = AudioObject.AddComponent<DestroyAfterDelay, float>(clip.length);
                }
                return source;
            }
        }
        public static AudioSource Sound3D(AudioClip clip, Vector3 location, bool autoDestroy = true)
        {
            if (Service.audioBox != null)
            {
                return Service.audioBox.PlaySound(clip, location, autoDestroy);
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
                    DestroyAfterDelay timedSelfDestroyer = AudioObject.AddComponent<DestroyAfterDelay, float>(clip.length);
                }
                return source;
            }
        }
        public static AudioSource Sound3D(AudioClip clip, AudioMixerGroup group, Vector3 location, bool autoDestroy = true)
        {
            if (Service.audioBox != null)
            {
                return Service.audioBox.PlaySound(clip, group, location, autoDestroy);
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
                    DestroyAfterDelay timedSelfDestroyer = AudioObject.AddComponent<DestroyAfterDelay, float>(clip.length);
                }
                return source;
            }
        }

        // Particle
        public static ParticleSystem Particle(GameObject prefab, Vector3 location)
        {
            ParticleSystem ParticleSmple = prefab.GetComponent<ParticleSystem>();
            if (ParticleSmple != null)
            {
                return Spawn.Prefab(prefab, location).GetComponent<ParticleSystem>();
            }
            else
            {
                Debug.LogError("Spawn Service: The provided prefab does not have a ParticleSystem component.");
                return null;
            }
        }

        // Memory
        public static GameObject GameObject(string name)
        {
            return new GameObject(name);
        }
        public static GameObject Prefab(GameObject prefab)
        {
            return Game.World.AddObject(prefab);
        }
        public static GameObject Prefab(GameObject prefab, Transform parent)
        {
            return Object.Instantiate(prefab, parent);
        }
        public static GameObject Prefab(GameObject prefab, Transform parent, bool instantiateInWorldSpace)
        {
            return Object.Instantiate(prefab, parent, instantiateInWorldSpace);
        }
        public static GameObject Prefab(GameObject prefab, Vector3 location, Vector3 rotation)
        {
            return Object.Instantiate(prefab, location, Quaternion.Euler(rotation));
        }
        public static GameObject Prefab(GameObject prefab, Vector3 location)
        {
            return Object.Instantiate(prefab, location, Quaternion.identity);
        }
        public static GameObject Prefab(GameObject prefab, Vector3 location, Vector3 rotation, Transform parent)
        {
            return Object.Instantiate(prefab, location, Quaternion.Euler(rotation), parent);
        }
        public static GameObject Prefab(GameObject prefab, UnityEngine.SceneManagement.Scene scene)
        {
            return Object.Instantiate(prefab, scene) as GameObject;
        }
        public static T Component<T>(string name)
        {
            GameObject emptyobjec = new GameObject(name);
            return emptyobjec.GetComponent<T>();
        }
        public static T Class<T>() where T : Object, new()
        {
            return new T();
        }
        public static MeshRenderer Mesh(Mesh geometry)
        {
            GameObject emptyobject = new GameObject(geometry.name);
            emptyobject.AddComponent<MeshFilter>().mesh = geometry;
            return emptyobject.AddComponent<MeshRenderer>();
        }
        public static T Command<T>(GameObject prefab, MonoBehaviour owner, Object author) where T : Command
        {
            T command = prefab.GetComponent<T>();
            if (command == null)
            {
                Debug.LogError($"Spawn Command Failed: The GameObject '{prefab.name}' does not have a command of type '{typeof(T).Name}'.");
                return null;
            }

            if (owner == null || author == null)
            {
                Debug.LogError($"Spawn Command Failed: Owner or Author Not valid.");
                return null;
            }
            GameObject SpawnedObject = Object.Instantiate(prefab, owner.transform);
            T TargetCommand = SpawnedObject.GetComponent<T>();
            if (!TargetCommand.GetComponent<ICommandInitiator>().Initiate(author, owner))
            {
                Debug.LogWarning($"Spawn Command Breack: Initiation failed for command '{typeof(T).Name}' on '{prefab.name}'.");
            }
            return TargetCommand;
        }
        public static T DataAsset<T>(T asset) where T : DataAsset
        {
            return ScriptableObject.CreateInstance<T>();
        }
    }


    public sealed class Despawn
    {
        private static Despawn Instance;
        private UIManager uIBox;
        private AudioManager audioBox;

        public Despawn(UIManager ui, AudioManager audio)
        {
            if (Instance == null)
            {
                Instance = this;
                uIBox = ui;
                audioBox = audio;
            }
        }


        public static bool Widget(string Name)
        {
            if (Instance != null)
            {
                return Instance.uIBox.RemoveLayer(Name);
            }
            else
            {
                return false;
            }
        }
        public static bool Widget(MonoBehaviour Comp)
        {
            if (Instance != null)
            {
                return Instance.uIBox.RemoveLayer(Comp);
            }
            else
            {
                return false;
            }

        }
    }


}