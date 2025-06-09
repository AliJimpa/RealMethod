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
        private GameObject Prefab;
        [SerializeField]
        private bool AutoDespown = true;
        [SerializeField, ConditionalHide("AutoDespown", true, false)]
        private bool UseDynamicDuration = true;
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
            if (UseDynamicDuration)
            {
                CachePosition = location;
                CacheRotation = rotation;
                Duration = duration;
                return Spawn();
            }
            else
            {
                Debug.LogError("The PoolObject Should be DynamicDuration");
                return null;
            }
        }
        public Transform Spawn(Vector3 location, Quaternion rotation, Vector3 scale)
        {
            UseCacheData = 3;
            if (!UseDynamicDuration)
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
            if (!UseDynamicDuration)
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
            if (UseDynamicDuration)
            {
                CachePosition = location;
                Duration = duration;
                return Spawn();
            }
            else
            {
                Debug.LogError("The PoolObject Should be DynamicDuration");
                return null;
            }
        }
        public Transform Spawn(Vector3 location)
        {
            UseCacheData = 1;
            if (!UseDynamicDuration)
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
            UseCacheData = 0;
            Transform result = Request();
            OnSpawn?.Invoke(result);
            return result;
        }
        public void Despawn()
        {
            if (!AutoDespown)
            {
                Return();
            }
            else
            {
                Debug.LogError("The Pool Object has AutoDespawn");
            }
        }
        public void Despawn(Transform target)
        {
            if (!AutoDespown)
            {
                Return(target);
            }
            else
            {
                Debug.LogError("The Pool Object has AutoDespawn");
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
                    Debug.LogWarning($"For this CashStage ({UseCacheData}) is Not implemented any Preprocessing");
                    break;
            }
        }
        protected override Transform CreateObject()
        {
            GameObject Target = Instantiate(Prefab);
            return Target.transform;
        }
        protected override IEnumerator PostProcess(Transform Comp)
        {
            return AutoDespown ? PoolBack(Comp) : null;
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