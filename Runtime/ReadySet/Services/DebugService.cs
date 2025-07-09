using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public sealed class Rebug : Service
    {
        private static Rebug instance;
        public static Rebug Service
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }
                else
                {
                    Debug.LogError("For Debugging You need Add DebugService in Game. [Game Class > InstanceCreated Method]");
                    return null;
                }
            }
            private set { }
        }

        private DebugManager Debugger;

        protected override void OnStart(object Author)
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogError($"DebugService Is Created befor, You Can't Create Twice! [Author:{Author}]");
                return;
            }

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
            Service.Debugger.Add(new LogData(Message));
        }
        public static void LogWarning(string Message)
        {
            Service.Debugger.Add(new LogData(Message, LogType.Warning));
        }
        public static void LogError(string Message)
        {
            Service.Debugger.Add(new LogData(Message, LogType.Error));
        }
        public static void LogAssertion(string Message)
        {
            Service.Debugger.Add(new LogData(Message, LogType.Assert));
        }



    }

}