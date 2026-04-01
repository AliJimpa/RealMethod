using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace RealMethod
{
    public interface IPool
    {
        void Prewarm(int amount = 1);
        IEnumerator PrewarmEachFrame(int amount = 1);
        void Request();
        void Return(int amount = 1);
        void Remove(int amount = 1, bool Force = false);
        void Clean();
    }
    public interface IPool<T> : IPool
    {
        IEnumerable<T> Request(int amount = 1);
    }
    public interface IPoolSpawner<J> where J : Component
    {
        J Spawn(Vector3 location, Quaternion rotation, Vector3 scale);
        J Spawn(Vector3 location, Quaternion rotation);
        J Spawn(Vector3 location);
        J Spawn();
    }
    public interface IPoolDespawner<J> where J : Component
    {
        void Despawn();
        void Despawn(J target);
    }

}