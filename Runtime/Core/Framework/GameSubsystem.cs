using System;
using System.Collections.Generic;

namespace RealMethod
{
    public interface IBootstrap
    {
        void Setup(GameSubsystem.ShareData data);
    }

    public abstract class GameSubsystem : IInspectorInfo, IDisposable, IBootstrap
    {
        public sealed class ShareData : IInspectorInfo, IDisposable
        {
            public Dictionary<Type, WeakReference<object>> Repository { get; }

            public ShareData(int prewarm)
            {
                Repository = new Dictionary<Type, WeakReference<object>>(prewarm);
            }


            // Implement IDisposable Interface
            void IDisposable.Dispose()
            {
                Repository.Clear();
            }

#if UNITY_EDITOR
            string IInspectorInfo.GetInfo()
            {
                return $"Repository ({Repository.Count})";
            }
#endif
        }
        protected ShareData Data { get; private set; }


        // Implement IBootstrap Interface
        void IBootstrap.Setup(ShareData data)
        {
            Data = data;
            OnBegin();
        }
        // Implement IDisposable Interface
        void IDisposable.Dispose()
        {
            OnEnd();
            Data = null;
        }

        protected abstract void OnBegin();
        protected abstract void OnEnd();


#if UNITY_EDITOR
        string IInspectorInfo.GetInfo()
        {
            return GetInspectorInfor();
        }
        protected abstract string GetInspectorInfor();
#endif
    }
}