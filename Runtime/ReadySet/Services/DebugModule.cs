using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public sealed class DebugModule : GameModule, ILogHandler, IDebugService
    {
        private class LogLine : IDrawTask
        {
            private DebugModule MyOwner;
            private string MyMessage;
            private LogType MyType;
            private Vector2 MyOffcet;
            private float ActiveTime;

            public Vector2 Pivot => MyOwner.Pivot;
            public float Space => ((IDebugService)MyOwner).PrintSpace;
            private int Size => MyOwner.Size;
            public bool IsFinished => !(Time.time - ActiveTime <= Duration);
            public float Duration
            {
                get
                {
                    int index = (int)MyType;

                    if (index >= 0 && index < MyOwner.LogDuraion.Length)
                        return MyOwner.LogDuraion[index];

                    return 8; // fallback
                }
            }
            public Color Color
            {
                get
                {
                    int index = (int)MyType;

                    if (index >= 0 && index < MyOwner.LogColors.Length)
                        return MyOwner.LogColors[index];

                    return Color.white; // fallback
                }
            }



            public LogLine(DebugModule owner, string message, LogType type)
            {
                MyOwner = owner;
                MyMessage = message;
                MyType = type;
                MyOffcet = Vector2.zero;
                ActiveTime = 0;
            }
            public LogLine(DebugModule owner, string message, LogType type, Vector2 offcet)
            {
                MyOwner = owner;
                MyMessage = message;
                MyType = type;
                MyOffcet = offcet;
                ActiveTime = 0;
            }

            // Implemetn ITask Interface
            void ITask.Active(object Instigator)
            {
                ActiveTime = Time.time;
            }
            void ITask.Deactive(object Instigator)
            {
                MyOwner.RemoveLog(this);
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
        private Vector2 Pivot = new Vector2(10, 10);
        private int Size = 1;
        private List<LogLine> Lines = new List<LogLine>(10);
        private ILogHandler defaultLogHandler;
        private float[] LogDuraion;
        private Color[] LogColors;


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
            OnLogWrited?.Invoke(logType, format);
        }
        void ILogHandler.LogException(System.Exception exception, Object context)
        {
            // Send message to screen system
            LogLine NewLine = new LogLine(this, exception.Message, LogType.Exception);
            Lines.Add(NewLine);
            Game.Draw(NewLine);

            defaultLogHandler.LogException(exception, context);
            OnLogWrited?.Invoke(LogType.Exception, exception.Message);
        }
        // Implement IGUIService Interface
        Vector2 IGUIService.PrintPivot => Pivot;
        int IGUIService.PrintSize => Size;
        void IGUIService.SetPivot(Vector2 pivot)
        {
            Pivot = pivot;
        }
        void IGUIService.SetSize(int size)
        {
            Size = size;
        }
        // Implement IDebugService Interface
        float IDebugService.PrintSpace => Screen.height / 40;
        public event System.Action<LogType, string> OnLogWrited;



        // Module Methods
        protected override void OnBegin()
        {
            defaultLogHandler = Debug.unityLogger.logHandler;
            Debug.unityLogger.logHandler = this;

            ProjectSettingAsset Setting;
            Setting = RM_Framework.LoadProjectSetting();
            LogDuraion = new float[5] { Setting.Error_Duration, Setting.Assert_Duration, Setting.Warning_Duration, Setting.Log_Duration, Setting.Exception_Duration };
            LogColors = new Color[5] { Setting.Error_Color, Setting.Assert_Color, Setting.Warning_Color, Setting.Log_Color, Setting.Exception_Color };
            RM_Framework.UnloadProjectSetting();
        }
        protected override void OnWorldChanged()
        {
            // Nothing
        }
        protected override void OnEnd()
        {
            Debug.unityLogger.logHandler = defaultLogHandler;
            Lines.Clear();
        }


        // Methods
        private void RemoveLog(LogLine line)
        {
            Lines.Remove(line);
        }



#if UNITY_EDITOR
        protected override string GetInspectorInfo()
        {
            return $"Lines ({Lines.Count})";
        }
#endif

    }
}