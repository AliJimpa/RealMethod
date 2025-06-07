using System.Security.Cryptography;
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
        private AudioManager audioBox;
        private UIManager uIBox;

        public override void Start(object Author)
        {
            if (instance == null)
            {
                instance = this;
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
            instance = null;
        }

        public void BringManager(IGameManager manager)
        {
            Debug.Log(manager);

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

        public static IWidget Widget<T>(string name, VisualTreeAsset uIAsset, MonoBehaviour owner) where T : MonoBehaviour
        {
            if (Service.uIBox != null)
            {
                return Service.uIBox.CreateWidget<T>(name, uIAsset, owner);
            }
            else
            {
                Debug.LogWarning("Spawn Service: UIManager is not available. Cannot create widget. You Need Mater UI");
                return null;
            }
        }
        public static IWidget Widget<T>(string name, VisualTreeAsset uIAsset) where T : MonoBehaviour
        {
            if (Service.uIBox != null)
            {
                return Service.uIBox.CreateWidget<T>(name, uIAsset);
            }
            else
            {
                Debug.LogWarning("Spawn Service: UIManager is not available. Cannot create widget. You Need Mater UI");
                return null;
            }
        }
        public static IWidget Widget(string name, GameObject prefab, MonoBehaviour owner)
        {
            if (Service.uIBox != null)
            {
                return Service.uIBox.AddWidget(name, prefab, owner);
            }
            else
            {
                Debug.LogWarning("Spawn Service: UIManager is not available. Cannot create widget. You Need Mater UI");
                return null;
            }
        }
        public static IWidget Widget(string name, GameObject prefab)
        {
            if (Service.uIBox != null)
            {
                return Service.uIBox.AddWidget(name, prefab);
            }
            else
            {
                Debug.LogWarning("Spawn Service: UIManager is not available. Cannot create widget. You Need Mater UI");
                return null;
            }
        }
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
        public static T Class<T>(string name) where T : MonoBehaviour
        {
            GameObject emptyobject = new GameObject(name);
            return emptyobject.AddComponent<T>();
        }
        public static GameObject Prefab(GameObject prefab)
        {
            return Game.World.AddObject(prefab);
        }
        public static GameObject Prefab(GameObject prefab, Vector3 location)
        {
            return Game.World.AddObject(prefab, location);
        }
        public static GameObject Prefab(GameObject prefab, Vector3 location, Vector3 rotation)
        {
            return Game.World.AddObject(prefab, location, rotation);
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
            TargetCommand.GetComponent<ICommandInitiator>().Initiate(author, owner);
            return TargetCommand;
        }
    }
}