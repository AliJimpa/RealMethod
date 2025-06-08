using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "AudioPool", menuName = "RealMethod/Pool/AudioPool", order = 1)]
    public sealed class AudioPool : PoolAsset<AudioSource>
    {
        [Header("Setting")]
        [SerializeField]
        private AudioClip[] TargetClip = new AudioClip[1];
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
        private byte UseCashData = 0; //0:NoCashing 1:CashLocation 2:CashLocation&Rotation 3:Transform
        private Vector3 CashPosition = Vector3.zero;
        private Quaternion CashRotation = Quaternion.identity;
        private Vector3 CashScale = Vector3.one;


        // Functions
        public AudioSource Spawn(Vector3 location, Quaternion rotation, Vector3 scale)
        {
            UseCashData = 3;
            CashPosition = location;
            CashRotation = rotation;
            CashScale = scale;
            return Spawn();
        }
        public AudioSource Spawn(Vector3 location, Quaternion rotation)
        {
            UseCashData = 2;
            CashPosition = location;
            CashRotation = rotation;
            return Spawn();
        }
        public AudioSource Spawn(Vector3 location)
        {
            UseCashData = 1;
            CashPosition = location;
            return Spawn();
        }
        public AudioSource Spawn()
        {
            UseCashData = 0;
            AudioSource result = Request();
            OnSpawn?.Invoke(result);
            return result;
        }

        // Private Functions
        private void RandomizeAudioSource(AudioSource source)
        {
            int clipLength = TargetClip.Length;
            if (clipLength > 1)
            {
                int randomIndex = UnityEngine.Random.Range(0, clipLength - 1);
                source.clip = TargetClip[randomIndex];
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
            switch (UseCashData)
            {
                case 1:
                    Comp.transform.position = CashPosition;
                    break;
                case 2:
                    Comp.transform.position = CashPosition;
                    Comp.transform.rotation = CashRotation;
                    break;
                case 3:
                    Comp.transform.position = CashPosition;
                    Comp.transform.rotation = CashRotation;
                    if (CashScale != Vector3.one)
                    {
                        Comp.transform.localScale = CashScale;
                    }
                    break;
                default:
                    Debug.LogWarning($"For this CashStage ({UseCashData}) is Not implemented any Preprocessing");
                    break;
            }
        }
        protected override AudioSource CreateObject()
        {
            int randomIndex = 0;
            if (TargetClip.Length > 1)
            {
                randomIndex = UnityEngine.Random.Range(0, TargetClip.Length - 1);
            }

            GameObject AudioObject = new GameObject("Audio_" + TargetClip[randomIndex].name);
            AudioSource source = AudioObject.AddComponent<AudioSource>();
            source.clip = TargetClip[randomIndex];
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


        // IEnumerator
        private IEnumerator PoolBack(AudioSource source)
        {
            RandomizeAudioSource(source);
            source.Play();
            yield return new WaitForSeconds(source.clip.length);
            source.Stop();
            this.Return(source);
        }


    }
}