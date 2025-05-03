using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "AudioPool", menuName = "Mustard/Pool/AudioPool", order = 1)]
    public class AudioPool : Pool<AudioSource>
    {
        [Header("ّFirstSetup")]
        [SerializeField]
        private AudioClip[] TargetClip = new AudioClip[0];
        [SerializeField, Range(0, 1)]
        private float spatialBlend = 1;
        [SerializeField]
        private float rolloffDistanceMin = 1f;
        [SerializeField]
        private float rolloffDistanceMax = 100f;
        [SerializeField]
        private AudioMixerGroup MixedGroup;
        private Vector3 AudioLocation;

        protected override void OnRootInitiate()
        {
            GameObjectRoot.SetParent(Game.World.transform);
        }
        protected override void PreProcess(AudioSource Comp)
        {
            Comp.transform.position = AudioLocation;
        }
        protected override AudioSource CreateObject()
        {
            int randomIndex = UnityEngine.Random.Range(0, TargetClip.Length - 1);

            GameObject impactSfxInstance = new GameObject("Audio_" + TargetClip[randomIndex].name);
            AudioSource source = impactSfxInstance.AddComponent<AudioSource>();
            source.clip = TargetClip[randomIndex];
            source.spatialBlend = spatialBlend;
            source.minDistance = rolloffDistanceMin;
            source.maxDistance = rolloffDistanceMax;
            source.outputAudioMixerGroup = MixedGroup;
            return source;
        }
        protected override IEnumerator PostProcess(AudioSource Comp)
        {
            return PoolBack(Comp);
        }


        public void Spawn(Vector3 Location)
        {
            AudioLocation = Location;
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