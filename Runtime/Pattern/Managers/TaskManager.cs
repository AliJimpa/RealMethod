using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public interface ITask<T> : ITask, ITick where T : IHandle
    {
        string ID => this.GetType().ToString();
        T Controller { get; }
    }

    public abstract class TaskManager : MonoBehaviour, IGameManager
    {
        protected readonly List<object> Tasks = new List<object>();
        public int Count => Tasks != null ? Tasks.Count : 0;

        // Implement IGameManager Interface
        void IGameManager.InitiateManager(bool AlwaysLoaded)
        {
            InitiateManager(AlwaysLoaded);
        }
        void IGameManager.ResolveService(Service service, bool active)
        {
            ResolveService(service);
        }

        // Abstract Methods
        protected abstract void InitiateManager(bool alwaysLoaded);
        protected abstract void ResolveService(Service service);
    }
    public abstract class TaskManager<J> : TaskManager where J : IHandle
    {

        public abstract J Create<F>(F Task, bool AutoStart = false) where F : class;
        public abstract J FindByName(Name16 TaskName);
        public abstract bool Destroy(J motion);
    }
    public abstract class TaskManager<T, J> : TaskManager<J> where T : ITask<J> where J : IHandle
    {
        [Header("Setting")]
        [SerializeField]
        private bool EnableTick = true;


        public event Action<J> OnTaskAdded;
        public event Action<J> OnTaskRemoved;


        // Unity Method
        private void Update()
        {
            if (EnableTick == false)
                return;

            float delta = Time.deltaTime;
            foreach (var task in Tasks)
            {
                if (task is T provider)
                    provider.Tick(delta);
            }
        }

        // Function
        public override J Create<F>(F Task, bool autoStart = false) where F : class
        {
            if (TryCreateNewInstance(Task, out F TaskObject))
            {
                J Provider = AddTask(TaskObject);
                if (Provider != null)
                {
                    if (autoStart)
                        AutoStart(Provider);
                    return Provider;
                }
                else
                {
                    return default;
                }
            }
            else
            {
                Debug.LogWarning($"Can't create instance with Type({typeof(F)})");
                return default;
            }
        }
        public override J FindByName(Name16 TaskName)
        {
            foreach (var task in Tasks)
            {
                if (task.HasNameID(TaskName))
                {
                    if (task is J provider)
                    {
                        return provider;
                    }
                    else
                    {
                        Debug.LogWarning($"Task({task}) should implement {typeof(J)}");
                        return default;
                    }
                }
            }
            return default;
        }
        public override bool Destroy(J motion)
        {
            return RemoveTask(motion);
        }

        // Method
        protected J AddTask(object Task)
        {
            if (Task == null)
            {
                Debug.LogWarning($"Task is not valid");
                return default;
            }

            if (Task is T provider)
            {
                Tasks.Add(Task);
                provider.Active();
                OnTaskAdded?.Invoke(provider.Controller);
                Task.InvokeSpawnEvent();
                return provider.Controller;
            }
            else
            {
                Debug.LogWarning($"Task({Task}) should implement {typeof(T)}");
                return default;
            }
        }
        protected bool RemoveTask(T task)
        {
            if (Tasks.Contains(task))
            {
                task.Deactive();
                OnTaskRemoved?.Invoke(task.Controller);
                return Tasks.Remove(task);
            }
            else
            {
                return false;
            }
        }
        protected bool RemoveTask(J behavior)
        {
            foreach (object task in Tasks)
            {
                if (task is T provider)
                {
                    if (provider.Controller.IsSame(behavior))
                    {
                        provider.Deactive();
                        OnTaskRemoved?.Invoke(provider.Controller);
                        provider.InvokeDespawnEvent();
                        return Tasks.Remove(provider);
                    }
                }
            }
            return false;
        }

        // Abstract Methods
        protected abstract bool TryCreateNewInstance<P>(object ClassType, out P result) where P : class;
        protected abstract void AutoStart(J Handle);
    }

}