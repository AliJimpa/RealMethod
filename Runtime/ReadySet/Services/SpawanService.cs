using UnityEngine;
using UnityEngine.Audio;

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
                    Debug.LogError("For Spawning You need Add SpawnService in Game. [Game Class > InstanceCreated Method]");
                    return null;
                }
            }
            private set { }
        }
        private AudioManager audioBox;

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
        }


        public static AudioSource PlaySound2D(AudioClip clip, bool autoDestroy = true)
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
        public static AudioSource PlaySound2D(AudioClip clip, AudioMixerGroup group, bool autoDestroy = true)
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
        public static AudioSource PlaySound(AudioClip clip, Vector3 location, bool autoDestroy = true)
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
        public static AudioSource PlaySound(AudioClip clip, AudioMixerGroup group, Vector3 location, bool autoDestroy = true)
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
        public static T Class<T>(string Name) where T : MonoBehaviour
        {
            GameObject emptyobject = new GameObject(Name);
            return emptyobject.AddComponent<T>();
        }
        public static GameObject Prefab(GameObject Prefab)
        {
            return Game.World.AddObject(Prefab);
        }
        public static GameObject AddObject(GameObject Prefab, Vector3 location)
        {
            return Game.World.AddObject(Prefab, location);
        }
        public static GameObject AddObject(GameObject Prefab, Vector3 location, Vector3 Rotation)
        {
            return Game.World.AddObject(Prefab, location, Rotation);
        }
        public static MeshRenderer Mesh(Mesh Geometry)
        {
            GameObject emptyobject = new GameObject(Geometry.name);
            emptyobject.AddComponent<MeshFilter>().mesh = Geometry;
            return emptyobject.AddComponent<MeshRenderer>();
        }
        

    }
}