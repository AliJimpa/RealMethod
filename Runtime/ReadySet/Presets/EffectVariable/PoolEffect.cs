using UnityEngine;

namespace RealMethod
{
    public class PoolEffect : Effect
    {
        [Header("Resource")]
        [SerializeField]
        private ParticlePool Particle;
        [SerializeField]
        private AudioPool Audio;
        [SerializeField]
        private bool RandomClip;


        // Cache Source
        private ParticleSystem part = null;
        private AudioSource audi = null;

        // Effect Methods
        protected override void OnProduce()
        {
            SpawnPool(pose.position, pose.rotation);
            if (hasParent)
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

        private void SpawnPool(Vector3 location, Quaternion rotation)
        {
            if (Particle)
                part = Particle.Spawn(location, rotation);
            if (Audio)
            {
                if (RandomClip)
                {
                    audi = Audio.Spawn(location, rotation);
                }
                else
                {
                    audi = Audio.Spawn(location, rotation, 0);
                }
            }

        }

    }
}