using System;
using System.Collections.Generic;

namespace RealMethod
{
    public abstract class GameSubsystem : IInspectorInfo
    {
        public class ShareData
        {
            public Dictionary<Type, WeakReference<object>> Repository { get; }

            public ShareData(int prewarm)
            {
                Repository = new Dictionary<Type, WeakReference<object>>(prewarm);
            }
        }
        protected ShareData Data { get; private set; }


        public GameSubsystem(ShareData data)
        {
            Data = data;
        }

#if UNITY_EDITOR
        string IInspectorInfo.GetInfo()
        {
            return GetInspectorInfor();
        }
        protected abstract string GetInspectorInfor();
#endif
    }
}