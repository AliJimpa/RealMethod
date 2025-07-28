using UnityEngine;

namespace RealMethod
{
    public interface IGameManager
    {
        MonoBehaviour GetManagerClass();
        void InitiateManager(bool AlwaysLoaded);
        void InitiateService(Service service);
    }

    public interface IService
    {
        void Created(object author);
        void WorldUpdated();
        void Deleted(object author);
    }

    public abstract class Service : IService
    {
        public IService provider => this;
        
        // Implement IService Interface
        void IService.Created(object author)
        {
            OnStart(author);
        }
        void IService.WorldUpdated()
        {
            OnNewWorld();
        }
        void IService.Deleted(object author)
        {
            OnEnd(author);
        }

        // Abstract Method
        protected abstract void OnStart(object Author);
        protected abstract void OnNewWorld();
        protected abstract void OnEnd(object Author);
    }

}