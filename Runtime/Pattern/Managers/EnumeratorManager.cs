using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{

    public interface ICoroutineTask
    {
        bool IsRunning { get; }
        bool IsPaused { get; }
        void Stop();
        void Pause();
        void Resume();
    }

    public abstract class EnumeratorManager : MonoBehaviour, IGameManager
    {

        private class CoroutineTask : ICoroutineTask
        {
            private IEnumerator routine;

            public CoroutineTask(IEnumerator routine)
            {
                this.routine = routine;
                IsRunning = true;
                IsPaused = false;
            }

            // Public Funtions
            public bool Tick()
            {
                if (!IsRunning) return false;
                if (IsPaused) return true;

                if (!routine.MoveNext())
                {
                    IsRunning = false;
                    return false;
                }
                return true;
            }

            // Implement ICoroutineTask Interface
            public bool IsRunning { get; private set; }
            public bool IsPaused { get; private set; }
            public void Stop()
            {
                IsRunning = false;
            }
            public void Pause()
            {
                IsPaused = true;
            }
            public void Resume()
            {
                IsPaused = false;
            }
        }

        private readonly List<CoroutineTask> tasks = new List<CoroutineTask>();

        // Implement IGameManager Interface
        public MonoBehaviour GetManagerClass()
        {
            return this;
        }
        void IGameManager.InitiateManager(bool AlwaysLoaded)
        {
            InitiateManager(AlwaysLoaded);
        }
        void IGameManager.ResolveService(Service service, bool active)
        {
            InitiateService(service);
        }


        // Unity Methods
        private void Update()
        {
            for (int i = tasks.Count - 1; i >= 0; i--)
            {
                if (!tasks[i].Tick())
                {
                    tasks.RemoveAt(i);
                }
            }
        }

        // Public Functions
        public ICoroutineTask StartTask(IEnumerator routine)
        {
            var task = new CoroutineTask(routine);
            tasks.Add(task);
            return task;
        }
        public Coroutine Run(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }
        public void Stop(Coroutine coroutine)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }


        // Abstract Methods
        protected abstract void InitiateManager(bool alwaysLoaded);
        protected abstract void InitiateService(Service newService);

    }

}