using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "AudioPool", menuName = "RealMethod/Pool/AudioPool", order = 1)]
    public sealed class AudioPool : PoolAsset<AudioSource>
    {
        [Header("ّFirstSetup")]
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



        private bool IsNeedLocation = false;
        private Vector3 AudioLocation;

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
            if (IsNeedLocation)
            {
                Comp.transform.position = AudioLocation;
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


        public void PlaySound(Vector3 Location)
        {
            IsNeedLocation = true;
            AudioLocation = Location;
            Request();
        }
        public void PlaySound2D()
        {
            IsNeedLocation = false;
            Request();
        }

        private void RandomizeAudioSource(AudioSource source)
        {
            int clipLength = TargetClip.Length;
            if (clipLength > 1)
            {
                int randomIndex = UnityEngine.Random.Range(0, clipLength - 1);
                source.clip = TargetClip[randomIndex];
            }
        }

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