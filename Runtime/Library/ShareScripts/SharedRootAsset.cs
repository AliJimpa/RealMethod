using UnityEngine;
using RealMethod;

public abstract class SharedRootAsset : DataAsset
{
    private Transform _GameObjectRoot;
    protected Transform GameObjectRoot
    {
        get
        {
            if (_GameObjectRoot == null)
            {
                _GameObjectRoot = new GameObject("Shared_" + name).transform;
                OnRootInitiate();
            }
            return _GameObjectRoot;
        }
    }

    protected abstract void OnRootInitiate();
    
    public Transform GetRoot()
    {
        return GameObjectRoot;
    }

    protected bool IsInitiate()
    {
        return _GameObjectRoot;
    }


}
