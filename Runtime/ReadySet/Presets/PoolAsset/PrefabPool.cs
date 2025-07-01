using System;
using System.Collections;
using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "PrefabPool", menuName = "RealMethod/Pool/PrefabPool", order = 1)]
    public sealed class PrefabPool : PoolAsset<Transform>
    {
        [Header("Setting")]
        [SerializeField]
        private GameObject prefab;
        [SerializeField]
        private bool autoDespawn = true;
        [SerializeField, ConditionalHide("AutoDespown", true, false)]
        private bool overrideDuration = true;
        [SerializeField, ConditionalHide("UseDynamicDuration", true, true)]
        private float Duration = 2;

        //Actions
        public Action<Transform> OnSpawn;


        // Private Variable
        private byte UseCacheData = 0; //0:NoCashing 1:CashLocation 2:CashLocation&Rotation 3:Transform 4:CashLocation&Duration 5:CashL&R&D
        private Vector3 CachePosition = Vector3.zero;
        private Quaternion CacheRotation = Quaternion.identity;
        private Vector3 CacheScale = Vector3.one;


        // Functions
        public Transform Spawn(Vector3 location, Quaternion rotation, float duration)
        {
            UseCacheData = 5;
            if (overrideDuration)
            {
                CachePosition = location;
                CacheRotation = rotation;
                Duration = duration;
                return Spawn();
            }
            else
            {
                Debug.LogError("OverrideDuration should enable in your PoolAsset");
                return null;
            }
        }
        public Transform Spawn(Vector3 location, Quaternion rotation, Vector3 scale)
        {
            UseCacheData = 3;
            if (!overrideDuration)
            {
                CachePosition = location;
                CacheRotation = rotation;
                CacheScale = scale;
                return Spawn();
            }
            else
            {
                Debug.LogError("In DynamicDuration You sould Spawn Object Without Time");
                return null;
            }
        }
        public Transform Spawn(Vector3 location, Quaternion rotation)
        {
            UseCacheData = 2;
            if (!overrideDuration)
            {
                CachePosition = location;
                CacheRotation = rotation;
                return Spawn();
            }
            else
            {
                Debug.LogError("In DynamicDuration You sould Spawn Object Without Time");
                return null;
            }
        }
        public Transform Spawn(Vector3 location, float duration)
        {
            UseCacheData = 4;
            if (overrideDuration)
            {
                CachePosition = location;
                Duration = duration;
                return Spawn();
            }
            else
            {
                Debug.LogError("OverrideDuration should enable in your PoolAsset");
                return null;
            }
        }
        public Transform Spawn(Vector3 location)
        {
            UseCacheData = 1;
            if (!overrideDuration)
            {
                CachePosition = location;
                return Spawn();
            }
            else
            {
                Debug.LogError("In DynamicDuration You sould Spawn Object With Time");
                return null;
            }
        }
        public Transform Spawn()
        {
            Transform result = Request();
            OnSpawn?.Invoke(result);
            return result;
        }
        public void Despawn()
        {
            if (!autoDespawn)
            {
                Return();
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
                case 4:
                    Comp.transform.position = CachePosition;
                    break;
                case 5:
                    Comp.transform.position = CachePosition;
                    Comp.transform.rotation = CacheRotation;
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


        // IEnumerator
        private IEnumerator PoolBack(Transform Transf)
        {
            // Befor Timer
            yield return new WaitForSeconds(Duration);
            // After Time
            Return(Transf);
            yield return null;
        }

    }
}