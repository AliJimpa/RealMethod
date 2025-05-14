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
        public abstract void Created(object Author);
        public abstract void Removed(object Author);
    }

}