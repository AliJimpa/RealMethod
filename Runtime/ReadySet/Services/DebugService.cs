using UnityEngine;

namespace RealMethod
{
    public sealed class Rebug : Service
    {
        private static Rebug CacheInstance = null;
        public static Rebug instance
        {
            get
            {
                if (CacheInstance != null)
                {
                    return CacheInstance;
                }
                else
                {
                    if (Game.TryFindService(out Rebug CacheInstance))
                    {
                        return CacheInstance;
                    }
                    CacheInstance = Game.AddService<Rebug>(null);
                    return CacheInstance;
                }
            }
            private set { }
        }

        private DebugManager Debugger;

        protected override void OnStart(object Author)
        {
            Debugger = Game.Instance.GetManager<DebugManager>();
            if (Debugger == null)
            {
                Debugger = Game.Instance.gameObject.AddComponent<DebugManager>();
                IGameManager manager = Debugger;
                manager.InitiateManager(true);
            }
        }
        protected override void OnNewWorld()
        {
            Debugger.Clear();
        }
        protected override void OnEnd(object Author)
        {
            Object.Destroy(Debugger);
        }

        public static void Log(string Message)
        {
            instance.Debugger.Add(new LogData(Message));
        }
        public static void LogWarning(string Message)
        {
            instance.Debugger.Add(new LogData(Message, LogType.Warning));
        }
        public static void LogError(string Message)
        {
            instance.Debugger.Add(new LogData(Message, LogType.Error));
        }
        public static void LogAssertion(string Message)
        {
            instance.Debugger.Add(new LogData(Message, LogType.Assert));
        }



    }

}