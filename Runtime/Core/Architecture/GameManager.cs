using UnityEngine;

namespace RealMethod
{
    public interface IGameManager
    {
        void InitiateManager(bool AlwaysLoaded);
        MonoBehaviour GetManagerClass();
        void InitiateService(Service service);
    }

    public abstract class Service
    {
        public abstract void Start(object Author);
        public abstract void WorldUpdated();
        public abstract void End(object Author);
    }

}