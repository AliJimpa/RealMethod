using System;
using UnityEngine;

namespace RealMethod
{
    public class GameService : Service
    {
        public Action<CoreWorld> OnWorldUpdat;

        // override Method
        public override void Created(object Author)
        {
            Debug.Log("GameService Created");
        }
        public override void Removed(object Author)
        {
            Debug.Log("GameService Removed");
        }


        public void IntroduceWorld(CoreWorld NewWorld , Action<CoreWorld> CallAdditive)
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