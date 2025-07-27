using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public class PoolComponent : MonoBehaviour
    {

    }

    public abstract class PoolAsset<T> : SharedRootAsset, IPool<T> where T : Component
    {
        public int Count => Available.Count;
        public IPool<T> Provider => this;
        protected Stack<T> Available = new Stack<T>();
        private bool Prewarmed = false;


#if UNITY_EDITOR
        // Base DataAsset Methods
        public override void OnEditorPlay()
        {
            //base.OnEditorPlay();
            Prewarmed = false;
        }
#endif

        // IPool Interface Implement
        void IPool<T>.Prewarm(int amount)
        {
            if (Prewarmed)
            {
                Debug.LogWarning($"Pool {name} has already been prewarmed.");
                return;
            }

            for (int i = 0; i < amount; i++)
            {
                var NewObject = CreateObject();
                NewObject.transform.SetParent(GetRoot());
                NewObject.gameObject.SetActive(false);
                Available.Push(NewObject);
            }
            Prewarmed = true;
        }
        IEnumerable<T> IPool<T>.Request(int amount)
        {
            T[] members = new T[amount];
            for (int i = 0; i < amount; i++)
            {
                members[i] = Request();
            }
            return members;
        }
        void IPool<T>.Return(int amount)
        {
            T[] Chield = GetRoot().GetComponentsInChildren<T>();
            for (int i = 0; i < amount; i++)
            {
                if (i < Chield.Length)
                {
                    Return(Chield[i]);
                }
            }
        }
        void IPool<T>.Remove(int amount, bool Force)
        {
            for (int i = 0; i < amount; i++)
            {
                if (Available.Count > 0)
                {
                    T item = Available.Pop();
                    Destroy(item.gameObject);
                }
                else
                {
                    if (Force)
                    {
                        T ValidComponent = GetRoot().GetComponent<T>();
                        if (ValidComponent)
                        {
                            Destroy(ValidComponent.gameObject);
                        }
                    }
                }
            }

        }
        void IPool<T>.Clean()
        {
            if (IsInitiateRoot())
            {
#if UNITY_EDITOR
                DestroyImmediate(GetRoot().gameObject);
#else
    Destroy(GetRoot().gameObject);
#endif
            }
            Clear();
        }

        // Protected Functions
        protected T Request()
        {
            if (!IsInitiateRoot())
            {
                Clear();
            }

            if (Available.Count > 0)
            {
                T item = Available.Pop();
                PreProcess(item);
                item.gameObject.SetActive(true);
                IEnumerator Poolback = PostProcess(item);
                if (Poolback != null)
                    CallPoolBackEvent(item, Poolback);
                return item;
            }
            else
            {
                T newMember = CreateObject();
                PreProcess(newMember);
                newMember.transform.SetParent(GetRoot().transform);
                IEnumerator Poolback = PostProcess(newMember);
                if (Poolback != null)
                    CallPoolBackEvent(newMember, Poolback);
                return newMember;
            }
        }
        protected void Return(T Value)
        {
            Value.gameObject.SetActive(false);
            Available.Push(Value);
        }
        protected virtual void Clear()
        {
            Available.Clear();
            Prewarmed = false;
        }

        // Private Functions
        private void CallPoolBackEvent(T Target, IEnumerator Corotine)
        {
            MonoBehaviour MonoComp = Target.GetComponent<MonoBehaviour>();
            if (MonoComp)
            {
                MonoComp.StartCoroutine(Corotine);
            }
            else
            {
                Target.gameObject.AddComponent<PoolComponent>().StartCoroutine(Corotine);
            }
        }

        // Abstract Methods
        protected abstract void PreProcess(T Comp);
        protected abstract T CreateObject();
        protected abstract IEnumerator PostProcess(T Comp);

    }

    public interface IPool<T>
    {
        void Prewarm(int amount = 1);
        IEnumerable<T> Request(int amount = 1);
        void Return(int amount = 1);
        void Remove(int amount = 1, bool Force = false);
        void Clean();
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



