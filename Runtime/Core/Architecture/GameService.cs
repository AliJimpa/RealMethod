using System;

namespace RealMethod
{
    public interface IGameService : IBridge, IInspectorInfo, IDisposable
    {
    }

    public abstract class GameService : IGameService
    {
        public GameService()
        {
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