using System;
using System.Collections;
using UnityEngine;


namespace RealMethod
{
    [CreateAssetMenu(fileName = "ParticlePool", menuName = "RealMethod/Pool/ParticlePool", order = 1)]
    public sealed class ParticlePool : PoolAsset<ParticleSystem>
    {
        [Header("Setting")]
        [SerializeField]
        public ParticleSystem Particle;

        //Actions
        public Action<ParticleSystem> OnSpawn;

        // Private Variable
        private byte UseCashData = 0; //0:NoCashing 1:CashLocation 2:CashLocation&Rotation 3:Transform
        private Vector3 CashPosition = Vector3.zero;
        private Quaternion CashRotation = Quaternion.identity;
        private Vector3 CashScale = Vector3.one;


        // Functions
        public ParticleSystem Spawn(Vector3 location, Quaternion rotation, Vector3 scale)
        {
            UseCashData = 3;
            CashPosition = location;
            CashRotation = rotation;
            CashScale = scale;
            return Spawn();
        }
        public ParticleSystem Spawn(Vector3 location, Quaternion rotation)
        {
            UseCashData = 2;
            CashPosition = location;
            CashRotation = rotation;
            return Spawn();
        }
        public ParticleSystem Spawn(Vector3 location)
        {
            UseCashData = 1;
            CashPosition = location;
            return Spawn();
        }
        public ParticleSystem Spawn()
        {
            UseCashData = 0;
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
        protected override ParticleSystem CreateObject()
        {
            var NewObject = Instantiate(Particle, Vector3.zero, Quaternion.identity);
            return NewObject.GetComponent<ParticleSystem>();
        }
        protected override IEnumerator PostProcess(ParticleSystem Comp)
        {
            return PoolBack(Comp);
        }


        // IEnumerator
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