using System;
using System.Collections;

namespace RealMethod
{
    public class LoadService : Service
    {
        public Action<bool> OnSceneLoading;
        public Action<float> OnSceneLoadingProcess;
        public override void Start(object Author)
        {
        }

        public override void End(object Author)
        {
            throw new NotImplementedException();
        }
    }
}