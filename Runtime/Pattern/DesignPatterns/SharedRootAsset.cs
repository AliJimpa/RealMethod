using UnityEngine;

namespace RealMethod
{
    public abstract class SharedRootAsset : DataAsset
    {
        private Transform AlternativeRootObject;
        private Transform RootObject
        {
            get
            {
                if (AlternativeRootObject == null)
                {
                    AlternativeRootObject = new GameObject("Shared_" + name).transform;
                    OnRootInitiate(AlternativeRootObject);
                }
                return AlternativeRootObject;
            }
        }

        protected abstract void OnRootInitiate(Transform Root);

        public Transform GetRoot()
        {
            return RootObject;
        }
        protected bool IsInitiate()
        {
            return AlternativeRootObject;
        }


    }

}
