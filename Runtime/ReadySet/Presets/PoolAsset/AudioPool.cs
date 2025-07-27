using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "AudioPool", menuName = "RealMethod/Pool/AudioPool", order = 1)]
    public sealed class AudioPool : PoolAsset<AudioSource> , IPoolSpawner<AudioSource>
    {
        [Header("Setting")]
        [SerializeField]
        private AudioClip[] Clips = new AudioClip[1];
        [SerializeField]
        private AudioMixerGroup Group;
        [SerializeField, Range(0, 1)]
        private float spatialBlend = 1;
        [SerializeField]
        private float rolloffDistanceMin = 1f;
        [SerializeField]
        private float rolloffDistanceMax = 100f;

        //Actions
        public Action<AudioSource> OnSpawn;


        // Private Variable
        private byte UseCacheData = 0; //0,1:NoCashing 2,3:CachePosition 4,5:CachePosition&Rotation 6,7:CacheTransform 
        private Vector3 CachePosition = Vector3.zero;
        private Quaternion CacheRotation = Quaternion.identity;
        private Vector3 CacheScale = Vector3.one;
        private int CacheIndex = 0;


        // Functions
        public AudioSource Spawn(Vector3 location, Quaternion rotation, Vector3 scale, int index)
        {
            UseCacheData = 7;
            CacheIndex = index;
            CachePosition = location;
            CacheRotation = rotation;
            CacheScale = scale;
            return Spawn();
        }
        public AudioSource Spawn(Vector3 location, Quaternion rotation, Vector3 scale)
        {
            UseCacheData = 6;
            CachePosition = location;
            CacheRotation = rotation;
            CacheScale = scale;
            return Spawn();
        }
        public AudioSource Spawn(Vector3 location, Quaternion rotation, int index)
        {
            UseCacheData = 5;
            CacheIndex = index;
            CachePosition = location;
            CacheRotation = rotation;
            return Spawn();
        }
        public AudioSource Spawn(Vector3 location, Quaternion rotation)
        {
            UseCacheData = 4;
            CachePosition = location;
            CacheRotation = rotation;
            return Spawn();
        }
        public AudioSource Spawn(Vector3 location, int index)
        {
            UseCacheData = 3;
            CacheIndex = index;
            CachePosition = location;
            return Spawn();
        }
        public AudioSource Spawn(Vector3 location)
        {
            UseCacheData = 2;
            CachePosition = location;
            return Spawn();
        }
        public AudioSource Spawn(int index)
        {
            UseCacheData = 1;
            CacheIndex = index;
            return Spawn();
        }
        public AudioSource Spawn()
        {
            AudioSource result = Request();
            OnSpawn?.Invoke(result);
            return result;
        }

        // Private Functions
        private AudioClip GetRandomClip()
        {
            int clipLength = Clips.Length;
            if (clipLength > 1)
            {
                int randomIndex = UnityEngine.Random.Range(0, clipLength - 1);
                return Clips[randomIndex];
            }
            else
            {
                return Clips[0];
            }
        }

        // Base PoolAsset Methods
        protected override void OnRootInitiate(Transform Root)
        {
            AudioManager audiomanager = Game.World.GetManager<AudioManager>();
            if (audiomanager != null)
            {
                Root.SetParent(audiomanager.transform);
            }
            else
            {
                Root.SetParent(Game.World.transform);
            }
        }
        protected override void PreProcess(AudioSource Comp)
        {
            switch (UseCacheData)
            {
                case 0:
                    Comp.clip = GetRandomClip();
                    break;
                case 1:
                    Comp.clip = Clips[CacheIndex];
                    break;
                case 2:
                    Comp.clip = GetRandomClip();
                    Comp.transform.position = CachePosition;
                    break;
                case 3:
                    Comp.clip = Clips[CacheIndex];
                    Comp.transform.position = CachePosition;
                    break;
                case 4:
                    Comp.clip = GetRandomClip();
                    Comp.transform.position = CachePosition;
                    Comp.transform.rotation = CacheRotation;
                    break;
                case 5:
                    Comp.clip = Clips[CacheIndex];
                    Comp.transform.position = CachePosition;
                    Comp.transform.rotation = CacheRotation;
                    break;
                case 6:
                    Comp.clip = GetRandomClip();
                    Comp.transform.position = CachePosition;
                    Comp.transform.rotation = CacheRotation;
                    if (CacheScale != Vector3.one)
                    {
                        Comp.transform.localScale = CacheScale;
                    }
                    break;
                case 7:
                    Comp.clip = Clips[CacheIndex];
                    Comp.transform.position = CachePosition;
                    Comp.transform.rotation = CacheRotation;
                    if (CacheScale != Vector3.one)
                    {
                        Comp.transform.localScale = CacheScale;
                    }
                    break;
                default:
                    Debug.LogWarning($"For this CacheStage ({UseCacheData}) is Not implemented any Preprocessing");
                    break;
            }
        }
        protected override AudioSource CreateObject()
        {
            int randomIndex = 0;
            if (Clips.Length > 1)
            {
                randomIndex = UnityEngine.Random.Range(0, Clips.Length - 1);
            }

            string ObjName = Clips.Length > 1 ? "Multi" : Clips[0].name;
            GameObject AudioObject = new GameObject("Audio_" + ObjName);
            AudioSource source = AudioObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = Group;
            source.spatialBlend = spatialBlend;
            source.minDistance = rolloffDistanceMin;
            source.maxDistance = rolloffDistanceMax;
            return source;
        }
        protected override IEnumerator PostProcess(AudioSource Comp)
        {
            return PoolBack(Comp);
        }

#if UNITY_EDITOR
        // Base DataAsset Methods
        public override void OnEditorPlay()
        {
            base.OnEditorPlay();
            UseCacheData = 0;
        }
#endif

        // IEnumerator
        private IEnumerator PoolBack(AudioSource source)
        {
            source.Play();
            yield return new WaitForSeconds(source.clip.length);
            source.Stop();
            Return(source);
        }


    }
}