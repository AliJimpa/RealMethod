using System.Collections;
using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "PrefabPool", menuName = "RealMethod/Pool/PrefabPool", order = 1)]
    public sealed class PrefabPool : Pool<Transform>
    {
        [SerializeField]
        private GameObject Prefab;
        [SerializeField]
        private bool AutoDespown = true;
        [SerializeField, ConditionalHide("AutoDespown", true, false)]
        private bool UseDynamicDuration = true;
        [SerializeField, ConditionalHide("UseDynamicDuration", true, true)]
        private float Duration = 2;


        private Vector3 ObjectPosition = Vector3.zero;
        private Quaternion ObjectRotation = Quaternion.identity;
        private Vector3 ObjectScale = Vector3.one;
        protected override void OnRootInitiate(Transform Root)
        {
            Root.SetParent(Game.World.transform);
        }
        protected override void PreProcess(Transform Comp)
        {
            Comp.transform.position = ObjectPosition;
            Comp.transform.rotation = ObjectRotation;
            if (ObjectScale != Vector3.one)
            {
                Comp.transform.localScale = ObjectScale;
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


        public void Spawn(Vector3 location)
        {
            if (!UseDynamicDuration)
            {
                ObjectPosition = location;
                Request();
            }
            else
            {
                Debug.LogError("In DynamicDuration You sould Spawn Object With Time");
            }
        }
        public void Spawn(Vector3 location, Quaternion rotation)
        {
            if (!UseDynamicDuration)
            {
                ObjectPosition = location;
                ObjectRotation = rotation;
                Request();
            }
            else
            {
                Debug.LogError("In DynamicDuration You sould Spawn Object Without Time");
            }
        }
        public void Spawn(Vector3 location, Quaternion rotation, Vector3 scale)
        {
            if (!UseDynamicDuration)
            {
                ObjectPosition = location;
                ObjectRotation = rotation;
                ObjectScale = scale;
                Request();
            }
            else
            {
                Debug.LogError("In DynamicDuration You sould Spawn Object Without Time");
            }
        }
        public void Spawn(Vector3 location, float duration)
        {
            if (UseDynamicDuration)
            {
                ObjectPosition = location;
                Duration = duration;
                Request();
            }
            else
            {
                Debug.LogError("The PoolObject Should be DynamicDuration");
            }
        }
        public void Spawn(Vector3 location, Quaternion rotation, float duration)
        {
            if (UseDynamicDuration)
            {
                ObjectPosition = location;
                ObjectRotation = rotation;
                Duration = duration;
                Request();
            }
            else
            {
                Debug.LogError("The PoolObject Should be DynamicDuration");
            }
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

        private IEnumerator PoolBack(Transform Transf)
        {
            yield return new WaitForSeconds(Duration);
            this.Return(Transf);
            yield return null;
        }

    }
}