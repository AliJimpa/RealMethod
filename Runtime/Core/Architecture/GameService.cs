using System;
using UnityEngine;

namespace RealMethod
{
    public abstract class GameService : IBridge, IInspectorInfo, IDisposable
    {
        public GameService()
        {
            // Check if you game not initialized
            if (!Game.IsGameInitialized)
            {
                Debug.LogWarning($"Game doesn't initialized !");
                return;
            }

            Game.Bridge.Bind(this);
            OnBegin();
        }

        // Implement IBridge Interface
        void IBridge.OnWorldChanged(World world) => OnWorldChanged();
        void IDisposable.Dispose()
        {
            Game.Bridge.Unbind(this);
            OnEnd();
        }

        // Abstraction Methods
        protected abstract void OnBegin();
        protected abstract void OnWorldChanged();
        protected abstract void OnEnd();

#if UNITY_EDITOR
        string IInspectorInfo.GetInfo()
        {
            return GetInspectorInfo();
        }
        protected virtual string GetInspectorInfo()
        {
            return null;
        }
#endif
    }


}