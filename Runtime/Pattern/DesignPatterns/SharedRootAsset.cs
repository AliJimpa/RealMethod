using UnityEngine;

namespace RealMethod
{
    public abstract class SharedRootAsset : DataAsset
    {
        private Transform RootObject;

        // Public Functions
        public bool IsInitiateRoot()
        {
            return RootObject;
        }
        public Transform GetRoot()
        {
            if (RootObject == null)
            {
                RootObject = new GameObject("Shared_" + name).transform;
                OnRootInitiate(RootObject);
            }
            return RootObject;
        }

        
        //Abstract Methods
        protected abstract void OnRootInitiate(Transform Root);
    }

}
