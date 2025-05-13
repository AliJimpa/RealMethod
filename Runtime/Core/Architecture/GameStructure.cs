using System;
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

    public class GameService : Service
    {
        public Action<World> OnWorldUpdate;
        public Action<World> OnAdditiveWorld;
        public Action<Service> OnServiceCreate;

        // override Method
        public override void Created(object Author)
        {
        }
        public override void Removed(object Author)
        {
        }

        // Any World in Awake time acall this method
        public bool IntroduceWorld(World NewWorld)
        {
            if (Game.World == null)
            {
                OnWorldUpdate?.Invoke(NewWorld);
                return true;
            }
            else
            {
                OnAdditiveWorld?.Invoke(NewWorld);
                return false;
            }
        }
    }

}