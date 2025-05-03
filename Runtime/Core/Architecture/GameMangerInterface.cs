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
        public abstract void Initiate(object Owner);
    }

}