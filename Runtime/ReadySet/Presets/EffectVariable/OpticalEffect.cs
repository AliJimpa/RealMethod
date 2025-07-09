using System;
using Codice.Client.BaseCommands.WkStatus.Printers;
using UnityEngine;
using UnityEngine.Audio;

namespace RealMethod
{
    [Serializable]
    public sealed class OpticalEffect : Effect
    {
        [Header("Resource")]
        [SerializeField]
        private ParticleSystem particle;
        [SerializeField]
        private HapticConfig haptic;
        [SerializeField]
        private AudioClip audioClip;
        
        [Header("AudioSetting")]
        [SerializeField]
        private AudioMixerGroup group;
        [SerializeField, Range(0, 1)]
        private float spatialBlend = 1;
        [SerializeField]
        private float rolloffDistanceMin = 1f;
        [SerializeField]
        private float rolloffDistanceMax = 100f;


        // Cache Source
        GameObject EffectObject = null;
        private ParticleSystem part = null;
        private AudioSource audi = null;


        // Effect Methods   
        protected override void OnProduce(Pose pose, Transform parent, float size)
        {
            if (haptic != null)
                haptic.Play(this);

            if (parent != null)
            {
                part = UnityEngine.Object.Instantiate(particle, pose.position, pose.rotation, parent);
            }
            else
            {
                part = UnityEngine.Object.Instantiate(particle, pose.position, pose.rotation);
            }
            part.transform.localScale = Vector3.one * size;
            EffectObject = part.gameObject;

            audi = EffectObject.AddComponent<AudioSource>();
            audi.clip = audioClip;

            if (group != null)
                audi.outputAudioMixerGroup = group;

            audi.spatialBlend = spatialBlend;
            audi.minDistance = rolloffDistanceMin;
            audi.maxDistance = rolloffDistanceMax;

            audi.Play();            

        }
        protected override void OnHold(bool enable)
        {
            if (enable)
            {
                if (part)
                    part.Pause();
                if (audi)
                    audi.Pause();
            }
            else
            {
                if (part)
                    part.Play();
                if (audi)
                    audi.Play();
            }
        }
        protected override void OnClear()
        {
            UnityEngine.Object.Destroy(EffectObject);
        }
        protected override void OnReset()
        {
        }


    }
}