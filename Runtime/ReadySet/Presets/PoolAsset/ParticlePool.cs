using System;
using System.Collections;
using UnityEngine;


namespace RealMethod
{
    [CreateAssetMenu(fileName = "ParticlePool", menuName = "RealMethod/Pool/ParticlePool", order = 1)]
    public sealed class ParticlePool : PoolAsset<ParticleSystem> , IPoolSpawner<ParticleSystem>
    {
        [Header("Setting")]
        [SerializeField]
        private ParticleSystem Particle;


        //Actions
        public Action<ParticleSystem> OnSpawn;

        // Private Variable
        private byte UseCacheData = 0; //0:NoCashing 1:CachePosition 2:CachePosition&Rotation 3:CacheTransform 
        private Vector3 CachePosition = Vector3.zero;
        private Quaternion CacheRotation = Quaternion.identity;
        private Vector3 CacheScale = Vector3.one;


        // Functions
        public ParticleSystem Spawn(Vector3 location, Quaternion rotation, Vector3 scale)
        {
            UseCacheData = 3;
            CachePosition = location;
            CacheRotation = rotation;
            CacheScale = scale;
            return Spawn();
        }
        public ParticleSystem Spawn(Vector3 location, Quaternion rotation)
        {
            UseCacheData = 2;
            CachePosition = location;
            CacheRotation = rotation;
            return Spawn();
        }
        public ParticleSystem Spawn(Vector3 location)
        {
            UseCacheData = 1;
            CachePosition = location;
            return Spawn();
        }
        public ParticleSystem Spawn()
        {
            ParticleSystem result = Request();
            OnSpawn?.Invoke(result);
            return result;
        }


        // Base PoolAsset Methods
        protected override void OnRootInitiate(Transform Root)
        {
            Root.SetParent(Game.World.transform);
        }
        protected override void PreProcess(ParticleSystem Comp)
        {
            switch (UseCacheData)
            {
                case 0:
                    break;
                case 1:
                    Comp.transform.position = CachePosition;
                    break;
                case 2:
                    Comp.transform.position = CachePosition;
                    Comp.transform.rotation = CacheRotation;
                    break;
                case 3:
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
        protected override ParticleSystem CreateObject()
        {
            return Instantiate(Particle, Vector3.zero, Quaternion.identity);
        }
        protected override IEnumerator PostProcess(ParticleSystem Comp)
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
        private IEnumerator PoolBack(ParticleSystem particle)
        {
            particle.Play();
            if (particle.main.loop)
            {
                yield return new WaitForSeconds(particle.main.duration);
                particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
            yield return new WaitUntil(() => particle.particleCount == 0);
            this.Return(particle);
            yield return null;
        }


    }
}