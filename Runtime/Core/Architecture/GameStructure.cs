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
        public Action<CoreWorld> OnWorldUpdat;

        // override Method
        public override void Created(object Author)
        {
        }
        public override void Removed(object Author)
        {
        }

        // Any World in Awake time acall this method
        public void IntroduceWorld(CoreWorld NewWorld, Action<CoreWorld> CallAdditive)
        {
            if (CoreGame.World == null)
            {
                Debug.LogWarning("Null");
                OnWorldUpdat?.Invoke(NewWorld);
            }
            else
            {
                Debug.LogWarning("Valid");
                CallAdditive?.Invoke(NewWorld);
            }
        }

    }

}