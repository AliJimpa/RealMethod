using UnityEngine;

namespace RealMethod
{
    public class DebugService : Service, ILogHandler
    {
        private ILogHandler defaultLogHandler;
        private PrintManager Printer;


        // Implement ILogHandler Interfacwe
        void ILogHandler.LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            string message = string.Format(format, args);

            // Send message to screen system
            if (Printer)
            {
                Printer.Print(message, logType);
            }

            // Still send message to Unity console
            defaultLogHandler.LogFormat(logType, context, format, args);
        }
        void ILogHandler.LogException(System.Exception exception, Object context)
        {
            // Send message to screen system
            if (Printer)
            {
                Printer.Print(exception.Message, LogType.Exception);
            }

            defaultLogHandler.LogException(exception, context);
        }


        // Service Methods
        protected override void OnStart(object Author)
        {
            defaultLogHandler = Debug.unityLogger.logHandler;
            Debug.unityLogger.logHandler = this;
            CheckManager();
        }
        protected override void OnWorldChanging(World Previous, World New)
        {
            CheckManager();
        }
        protected override void OnEnd(object Author)
        {
            if(Printer != null)
            {
                Printer.Clear();
            }
        }




        private void CheckManager()
        {
            if (Printer == null)
                Printer = Game.GetManager<PrintManager>();
        }

    }
}