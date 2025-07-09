using UnityEngine;
using UnityEngine.Audio;

namespace RealMethod
{
    public class OpticalEffect : Effect
    {
        [Header("Resource")]
        [SerializeField]
        private ParticleSystem Particle;
        [SerializeField]
        private AudioClip Clip;
        [SerializeField]
        private AudioMixerGroup Group;
        [Header("Setting")]
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
        protected override void OnProduce()
        {
            part = Object.Instantiate(Particle, pose.position, pose.rotation);
            EffectObject = part.gameObject;
            audi = EffectObject.AddComponent<AudioSource>();
            audi.clip = Clip;
            audi.outputAudioMixerGroup = Group;
            audi.spatialBlend = spatialBlend;
            audi.minDistance = rolloffDistanceMin;
            audi.maxDistance = rolloffDistanceMax;
            if (hasParent)
            {
                EffectObject.transform.SetParent(parent);
            }
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
            Object.Destroy(EffectObject);
        }
        protected override void OnReset()
        {

        }


    }
}