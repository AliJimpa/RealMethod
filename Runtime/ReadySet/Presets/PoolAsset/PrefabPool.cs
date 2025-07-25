using System;
using System.Collections;
using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "PrefabPool", menuName = "RealMethod/Pool/PrefabPool", order = 1)]
    public sealed class PrefabPool : PoolAsset<Transform>, IPoolSpawner<Transform>, IPoolDespawner<Transform>
    {
        [Header("Setting")]
        [SerializeField]
        private GameObject prefab;
        [SerializeField]
        private bool autoDespawn = true;
        [SerializeField, ConditionalHide("autoDespawn", true, false)]
        private float duration = 2;

        //Actions
        public Action<Transform> OnSpawn;


        // Private Variable
        private byte UseCacheData = 0; //0:NoCashing 1:CachePosition 2:CachePosition&Rotation 3:CacheTransform 
        private Vector3 CachePosition = Vector3.zero;
        private Quaternion CacheRotation = Quaternion.identity;
        private Vector3 CacheScale = Vector3.one;
        private float CacheDuration = 0;


        // Functions
        public Transform Spawn(Vector3 location, Quaternion rotation, Vector3 scale, float overrideDuration)
        {
            UseCacheData = 3;
            CachePosition = location;
            CacheRotation = rotation;
            CacheScale = scale;
            return Spawn(overrideDuration);
        }
        public Transform Spawn(Vector3 location, Quaternion rotation, Vector3 scale)
        {
            UseCacheData = 3;
            CachePosition = location;
            CacheRotation = rotation;
            CacheScale = scale;
            return Spawn();
        }
        public Transform Spawn(Vector3 location, Quaternion rotation, float overrideDuration)
        {
            UseCacheData = 2;
            CachePosition = location;
            CacheRotation = rotation;
            return Spawn(overrideDuration);
        }
        public Transform Spawn(Vector3 location, Quaternion rotation)
        {
            UseCacheData = 2;
            CachePosition = location;
            CacheRotation = rotation;
            return Spawn();
        }
        public Transform Spawn(Vector3 location, float overrideDuration)
        {
            UseCacheData = 1;
            CachePosition = location;
            return Spawn(overrideDuration);
        }
        public Transform Spawn(Vector3 location)
        {
            UseCacheData = 1;
            CachePosition = location;
            return Spawn();
        }
        public Transform Spawn(float overrideDuration)
        {
            CacheDuration = overrideDuration;
            Transform result = Request();
            OnSpawn?.Invoke(result);
            return result;
        }
        public Transform Spawn()
        {
            CacheDuration = duration;
            Transform result = Request();
            OnSpawn?.Invoke(result);
            return result;
        }
        public void Despawn()
        {
            if (!autoDespawn)
            {
                Provider.Return();
            }
            else
            {
                Debug.LogError("Can't Despawn, AutoDespawn in your PoolAsset is Enable");
            }
        }
        public void Despawn(Transform target)
        {
            if (!autoDespawn)
            {
                Return(target);
            }
            else
            {
                Debug.LogError("Can't Despawn, AutoDespawn in your PoolAsset is Enable");
            }
        }


        // Base PoolAsset Methods
        protected override void OnRootInitiate(Transform Root)
        {
            Root.SetParent(Game.World.transform);
        }
        protected override void PreProcess(Transform Comp)
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
        protected override Transform CreateObject()
        {
            GameObject Target = Instantiate(prefab);
            return Target.transform;
        }
        protected override IEnumerator PostProcess(Transform Comp)
        {
            return autoDespawn ? PoolBack(Comp) : null;
        }

#if UNITY_EDITOR
        // Base DataAsset Methods
        public override void OnEditorPlay()
        {
            base.OnEditorPlay();
            UseCacheData = 0;
            CacheDuration = 0;
        }
#endif
        // IEnumerator
        private IEnumerator PoolBack(Transform Transf)
        {
            // Befor Timer
            yield return new WaitForSeconds(CacheDuration);
            // After Time
            Return(Transf);
            yield return null;
        }


    }
}