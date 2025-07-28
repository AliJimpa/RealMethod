using UnityEngine;

namespace RealMethod
{
    public sealed class Print : Service
    {
        private static Print CacheInstance = null;
        public static Print Instance
        {
            get
            {
                if (CacheInstance != null)
                {
                    return CacheInstance;
                }
                else
                {
                    if (Game.TryFindService(out Print CacheInstance))
                    {
                        return CacheInstance;
                    }
                    CacheInstance = Game.AddService<Print>(null);
                    return CacheInstance;
                }
            }
            private set { }
        }
        public static PrintManager Manager => Instance.Printer;

        private PrintManager Printer;

        protected override void OnStart(object Author)
        {
            Printer = Game.Instance.GetManager<PrintManager>();
            if (Printer == null)
            {
                Printer = Game.Instance.gameObject.AddComponent<PrintManager>();
                IGameManager manager = Printer;
                manager.InitiateManager(true);
            }
        }
        protected override void OnNewWorld()
        {
            Printer.Clear();
        }
        protected override void OnEnd(object Author)
        {
            Object.Destroy(Printer);
        }


        public static void Log(string message, float duration = 2)
        {
            Instance.Printer.Add(new LogData(message, duration));
        }
        public static void LogWarning(string message, float duration = 2)
        {
            Instance.Printer.Add(new LogData(message, duration, LogType.Warning));
        }
        public static void LogError(string message, float duration = 2)
        {
            Instance.Printer.Add(new LogData(message, duration, LogType.Error));
        }
        public static void LogAssertion(string message, float duration = 2)
        {
            Instance.Printer.Add(new LogData(message, duration, LogType.Assert));
        }
        public static void Button(string name, object author, string funtionName)
        {
            Instance.Printer.Add(new ButtonData(name, author, funtionName));
        }
    }

}