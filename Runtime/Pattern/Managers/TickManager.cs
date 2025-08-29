using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public abstract class TickManager<T> : MonoBehaviour, IGameManager where T : ITick
    {
        [Header("Tick")]
        [SerializeField]
        private UpdateMethod updateMethod = UpdateMethod.Update;
        [SerializeField]
        private int baseCapacity = 10;

        protected List<T> units { get; private set; }
        public int Count => units != null ? units.Count : 0;

        // Implement IGameManager Interface
        MonoBehaviour IGameManager.GetManagerClass()
        {
            return this;
        }
        void IGameManager.InitiateManager(bool AlwaysLoaded)
        {
            units = new List<T>(baseCapacity);
            InitiateManager(AlwaysLoaded);
        }
        void IGameManager.InitiateService(Service service)
        {
            InitiateService(service);
        }

        // Unity Methods
        private void LateUpdate()
        {
            if (updateMethod == UpdateMethod.LateUpdate)
            {
                callUnits();
            }
        }
        private void Update()
        {
            if (updateMethod == UpdateMethod.Update)
            {
                callUnits();
            }
        }
        private void FixedUpdate()
        {
            if (updateMethod == UpdateMethod.FixedUpdate)
            {
                callUnits();
            }
        }

        // Public Functions
        public bool IsValid(T unit)
        {
            return units.Contains(unit);
        }
        public T[] GetAllTasks()
        {
            return units.ToArray();
        }

        // Private Functions
        private void callUnits()
        {
            float delta = Time.deltaTime;
            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                if (CheckUnit(unit))
                    unit.Tick(delta);
            }
        }

        // Abstract Methods
        protected abstract void InitiateManager(bool alwaysLoaded);
        protected abstract void InitiateService(Service service);
        protected abstract bool CheckUnit(T unit);
    }
}