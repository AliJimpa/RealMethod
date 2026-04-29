using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public sealed class DebugService : Service, ILogHandler
    {
        private class LogLine : IDrawTask
        {
            private DebugService MyOwner;
            private string MyMessage;
            private LogType MyType;
            private Vector2 MyOffcet;
            private float ActiveTime;


            public Vector2 Pivot => MyOwner.PrintPivot;
            public float Space => MyOwner.PrintSpace;
            private int Size => MyOwner.PrintSize;
            public bool IsFinished => !(Time.time - ActiveTime <= Duration);
            public float Duration
            {
                get
                {
                    switch (MyType)
                    {
                        case LogType.Log:
                            return 5;
                        case LogType.Warning:
                            return 10;
                        case LogType.Error:
                            return 15;
                        case LogType.Assert:
                            return 20;
                        case LogType.Exception:
                            return 25;
                        default:
                            return 0;
                    }
                }
            }
            public Color Color
            {
                get
                {
                    switch (MyType)
                    {
                        case LogType.Log:
                            return Color.cyan;
                        case LogType.Warning:
                            return Color.yellow;
                        case LogType.Error:
                            return Color.red;
                        case LogType.Assert:
                            return Color.white;
                        case LogType.Exception:
                            return Color.blue;
                        default:
                            return Color.black;
                    }
                }
            }



            public LogLine(DebugService owner, string message, LogType type)
            {
                MyOwner = owner;
                MyMessage = message;
                MyType = type;
                MyOffcet = Vector2.zero;
                ActiveTime = 0;
            }
            public LogLine(DebugService owner, string message, LogType type, Vector2 offcet)
            {
                MyOwner = owner;
                MyMessage = message;
                MyType = type;
                MyOffcet = offcet;
                ActiveTime = 0;
            }

            // Implement IIdentifier Interface
            Name16 IIdentifier.NameID => throw new System.NotImplementedException();
            // Implemetn ITask Interface
            void ITask.Active()
            {
                ActiveTime = Time.time;
            }
            void ITask.Deactive()
            {
                MyOwner.OnLineDeactive(this);
            }
            // Implement IDraw Interface
            bool IDraw.CanDraw(int Index)
            {
                return !IsFinished;
            }
            void IDraw.Draw(int Index)
            {
                int w = Screen.width * Size;
                int h = Screen.height * Size;
                float Xpos = Pivot.x + MyOffcet.x;
                float Ypos = Pivot.y + MyOffcet.y + (Index * Space);

                GUIStyle style = new GUIStyle();
                Rect rect = new Rect(Xpos, Ypos, w, h * 2 / 100);
                style.alignment = TextAnchor.UpperLeft;
                style.fontSize = h * 2 / 100;
                style.normal.textColor = Color;
                GUI.Label(rect, MyMessage, style);
            }
            // Implement IDrawTask Interface
            int IDrawTask.Priority => 0;
            bool IDrawTask.IsExpired()
            {
                return IsFinished;
            }
            DrawMode IDrawTask.GetDrawMode()
            {
                return DrawMode.GUI;
            }

        }
        private ILogHandler defaultLogHandler;

        private List<LogLine> Lines = new List<LogLine>(10);
        public Vector2 PrintPivot = new Vector2(10, 10);
        public float PrintSpace => Screen.height / 40;
        public int PrintSize = 1;


        // Implement ILogHandler Interfacwe
        void ILogHandler.LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            string message = string.Format(format, args);

            // Send message to screen system
            LogLine NewLine = new LogLine(this, message, logType);
            Lines.Add(NewLine);
            Game.Draw(NewLine);

            // Still send message to Unity console
            if (defaultLogHandler != null)
            {
                defaultLogHandler.LogFormat(logType, context, format, args);
            }
            else
            {
                Debug.unityLogger.logHandler.LogFormat(logType, context, format, args);
            }
        }
        void ILogHandler.LogException(System.Exception exception, Object context)
        {
            // Send message to screen system
            LogLine NewLine = new LogLine(this, exception.Message, LogType.Exception);
            Lines.Add(NewLine);
            Game.Draw(NewLine);

            defaultLogHandler.LogException(exception, context);
        }


        // Service Methods
        protected override void OnStart(object Author)
        {
            defaultLogHandler = Debug.unityLogger.logHandler;
            Debug.unityLogger.logHandler = this;
        }
        protected override void OnWorldChanging(World Previous, World New)
        {
        }
        protected override void OnEnd(object Author)
        {
            if (defaultLogHandler != null)
                Debug.unityLogger.logHandler = defaultLogHandler;
            Lines.Clear();
        }
        protected override string GetDisplayInfo()
        {
            return $"{base.GetDisplayInfo()}Lines:{Lines.Count}";
        }

        // Methods
        private void OnLineDeactive(LogLine line)
        {
            Lines.Remove(line);
        }
    }
}