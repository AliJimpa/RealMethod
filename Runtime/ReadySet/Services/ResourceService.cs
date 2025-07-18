using System.Collections.Generic;

namespace RealMethod
{
    public sealed class Memory : Service
    {
        private static Memory CacheInstance = null;
        public static Memory instance
        {
            get
            {
                if (CacheInstance != null)
                {
                    return CacheInstance;
                }
                else
                {
                    if (Game.TryFindService(out Memory CacheInstance))
                    {
                        return CacheInstance;
                    }
                    CacheInstance = Game.AddService<Memory>(null);
                    return CacheInstance;
                }
            }
            private set { }
        }

        private Dictionary<ResourcAsset, int> Assets = new Dictionary<ResourcAsset, int>();

        // Service Methods
        protected override void OnStart(object Author)
        {
        }
        protected override void OnNewWorld()
        {
        }
        protected override void OnEnd(object Author)
        {
        }


    }
}