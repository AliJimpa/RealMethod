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
        private byte UseCashData = 0; //0:NoCashing 1:CashLocation 2:CashLocation&Rotation 3:Transform 4:CashLocation&Duration 5:CashL&R&D
        private Vector3 CashPosition = Vector3.zero;
        private Quaternion CashRotation = Quaternion.identity;
        private Vector3 CashScale = Vector3.one;


        // Functions
        public Transform Spawn(Vector3 location, Quaternion rotation, float duration)
        {
            UseCashData = 5;
            if (UseDynamicDuration)
            {
                CashPosition = location;
                CashRotation = rotation;
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
            UseCashData = 3;
            if (!UseDynamicDuration)
            {
                CashPosition = location;
                CashRotation = rotation;
                CashScale = scale;
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
            UseCashData = 2;
            if (!UseDynamicDuration)
            {
                CashPosition = location;
                CashRotation = rotation;
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
            UseCashData = 4;
            if (UseDynamicDuration)
            {
                CashPosition = location;
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
            UseCashData = 1;
            if (!UseDynamicDuration)
            {
                CashPosition = location;
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
            UseCashData = 0;
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
                case 4:
                    Comp.transform.position = CashPosition;
                    break;
                case 5:
                    Comp.transform.position = CashPosition;
                    Comp.transform.rotation = CashRotation;
                    break;
                default:
                    Debug.LogWarning($"For this CashStage ({UseCashData}) is Not implemented any Preprocessing");
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