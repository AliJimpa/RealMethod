using System.Collections;
using UnityEngine;


namespace RealMethod
{
    [CreateAssetMenu(fileName = "ParticlePool", menuName = "RealMethod/Pool/ParticlePool", order = 1)]
    public sealed class ParticlePool : Pool<ParticleSystem>
    {
        public ParticleSystem Particle;
        private Vector3 ParticleLocation = Vector3.zero;
        private Quaternion ParticleRotation = Quaternion.identity;
        private Vector3 Particlescale = Vector3.one;


        protected override void OnRootInitiate(Transform Root)
        {
            Root.SetParent(Game.World.transform);
        }
        protected override void PreProcess(ParticleSystem Comp)
        {
            Comp.transform.position = ParticleLocation;
            Comp.transform.rotation = ParticleRotation;
            Comp.transform.localScale = Particlescale;
        }
        protected override ParticleSystem CreateObject()
        {
            var NewObject = Instantiate(Particle, Vector3.zero, Quaternion.identity);
            return NewObject.GetComponent<ParticleSystem>();
        }
        protected override IEnumerator PostProcess(ParticleSystem Comp)
        {
            return PoolBack(Comp);
        }


        public void Spawn(Vector3 location)
        {
            ParticleLocation = location;
            Request();
        }
        public void Spawn(Vector3 location, Quaternion rotation, Vector3 scale)
        {
            ParticleLocation = location;
            ParticleRotation = rotation;
            Particlescale = scale;
            Request();
        }

        private IEnumerator PoolBack(ParticleSystem particle)
        {
            particle.Play();
            yield return new WaitForSeconds(particle.main.duration);
            particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            yield return new WaitUntil(() => particle.particleCount == 0);
            this.Return(particle);
            yield return null;
        }


    }
}