using System;
using System.Collections;

namespace RealMethod
{
    public class LoadService : Service
    {
        public Action<bool> OnSceneLoading;
        public Action<float> OnSceneLoadingProcess;
        public override void Created(object Author)
        {
        }

        public override void Removed(object Author)
        {
            throw new NotImplementedException();
        }
    }
}