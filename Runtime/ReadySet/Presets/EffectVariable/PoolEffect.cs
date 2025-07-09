using System;
using UnityEngine;

namespace RealMethod
{
    [Serializable]
    public sealed class PoolEffect : Effect
    {
        [Header("Resource")]
        [SerializeField]
        private ParticlePool Particle;
        [SerializeField]
        private HapticConfig haptic;
        [SerializeField]
        private AudioPool Audio;
        [SerializeField]
        private bool RandomClip;


        // Cache Source
        private ParticleSystem part = null;
        private AudioSource audi = null;

        // Effect Methods
        protected override void OnProduce(Pose pose, Transform parent, float size)
        {
            SpawnPool(pose.position, pose.rotation, size * Vector3.one);
            if (parent != null)
            {
                Debug.LogWarning("PoolEffect Cant set parent");
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
                if (part && part.gameObject.activeSelf)
                    part.Play();
                if (audi && part.gameObject.activeSelf)
                    audi.Play();
            }
        }
        protected override void OnClear()
        {
            part = null;
            audi = null;
        }
        protected override void OnReset()
        {

        }

        private void SpawnPool(Vector3 location, Quaternion rotation, Vector3 scale)
        {
            if (Particle)
                part = Particle.Spawn(location, rotation, scale);
            if (Audio)
            {
                if (RandomClip)
                {
                    audi = Audio.Spawn(location, rotation, scale);
                }
                else
                {
                    audi = Audio.Spawn(location, rotation, scale, 0);
                }
            }

        }

    }
}