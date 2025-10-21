using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RealMethod
{

    [Serializable]
    public class LogData
    {
        public string message;
        public float duration;
        public float x, y;
        public int size;
        public Color TextColor;
        public LogType messagetype;
        public Vector2 position => new Vector2(x, y);

        public LogData(string Message)
        {
            message = Message;
            duration = 2f;
            x = -1;
            y = -1;
            size = 1;
            TextColor = Color.black;
            messagetype = LogType.Log;
        }

        public LogData(string Message, LogType type)
        {
            message = Message;
            duration = 2f;
            x = -1;
            y = -1;
            size = 1;
            TextColor = GetColor(type);
            messagetype = type;
        }
        public LogData(string Message, bool Console)
        {
            message = Message;
            duration = 2f;
            x = -1;
            y = -1;
            size = 1;
            TextColor = Color.cyan;
            messagetype = LogType.Log;
        }

        public LogData(string Message, float Duration)
        {
            message = Message;
            duration = Duration;
            x = -1;
            y = -1;
            size = 1;
            TextColor = Color.cyan;
            messagetype = LogType.Log;
        }
        public LogData(string Message, float Duration, LogType type)
        {
            message = Message;
            duration = Duration;
            x = -1;
            y = -1;
            size = 1;
            TextColor = GetColor(type);
            messagetype = type;
        }
        public LogData(string Message, float Duration, bool Console)
        {
            message = Message;
            duration = Duration;
            x = -1;
            y = -1;
            size = 1;
            TextColor = Color.cyan;
            messagetype = LogType.Exception;
        }

        public LogData(string Message, float Duration, Color TargetColor)
        {
            message = Message;
            duration = Duration;
            x = -1;
            y = -1;
            size = 1;
            TextColor = TargetColor;
            messagetype = LogType.Log;
        }

        public LogData(string Message, Color TargetColor)
        {
            message = Message;
            duration = 2f;
            x = -1;
            y = -1;
            size = 1;
            TextColor = TargetColor;
            messagetype = LogType.Log;
        }

        public LogData(string Message, float Duration, float X, float Y)
        {
            message = Message;
            duration = Duration;
            x = X;
            y = Y;
            size = 1;
            TextColor = Color.cyan;
            messagetype = LogType.Log;
        }
        public LogData(string Message, float Duration, float X, float Y, Color TargetColor)
        {
            message = Message;
            duration = Duration;
            x = X;
            y = Y;
            size = 1;
            TextColor = TargetColor;
            messagetype = LogType.Log;
        }

        public LogData(string Message, float X, float Y)
        {
            message = Message;
            duration = 2f;
            x = X;
            y = Y;
            size = 1;
            TextColor = Color.cyan;
            messagetype = LogType.Log;
        }

        public LogData(string Message, float Duration, float X, float Y, int Size)
        {
            message = Message;
            duration = Duration;
            x = X;
            y = Y;
            size = Size;
            TextColor = Color.cyan;
            messagetype = LogType.Log;
        }

        public LogData(string Message, float X, float Y, int Size, bool Console, LogType type)
        {
            message = Message;
            duration = 2f;
            x = X;
            y = Y;
            size = Size > 0 ? Size : 1;
            TextColor = Color.cyan;
            messagetype = type;
        }

        public LogData(string Message, float Duration, float X, float Y, int Size, Color TargetColor)
        {
            message = Message;
            duration = Duration;
            x = X;
            y = Y;
            size = Size > 0 ? Size : 1;
            TextColor = TargetColor;
            messagetype = LogType.Log;
        }
        public LogData(string Message, float Duration, float X, float Y, int Size, LogType type)
        {
            message = Message;
            duration = Duration;
            x = X;
            y = Y;
            size = Size > 0 ? Size : 1;
            TextColor = GetColor(type);
            messagetype = type;
        }


        private Color GetColor(LogType type)
        {
            switch (type)
            {
                case LogType.Log:
                    return Color.cyan;
                case LogType.Warning:
                    return Color.yellow;
                case LogType.Error:
                    return Color.red;
                case LogType.Assert:
                    return Color.black;
                case LogType.Exception:
                    return Color.white;
                default:
                    return Color.blue;
            }
        }
    }
    [Serializable]
    public class ButtonData
    {
        [SerializeField] string Name = "Button";
        [SerializeField] string Description = "This is a tooltip for the button";

        [SerializeField] object TargetObject;
        [SerializeField] string MethodName;
        bool IsFunctionMethod = false;
        [SerializeField] object[] MethodParametr;

        public ButtonData(string name, string description, object targetObject, string methodName, object[] methodParametr)
        {
            Name = name;
            Description = description;
            TargetObject = targetObject;
            MethodName = methodName;
            IsFunctionMethod = true;
            MethodParametr = methodParametr;
        }

        public ButtonData(string name, string description, object targetObject, string methodName)
        {
            Name = name;
            Description = description;
            TargetObject = targetObject;
            MethodName = methodName;
        }

        public ButtonData(string name, object targetObject, string methodName)
        {
            Name = name;
            TargetObject = targetObject;
            MethodName = methodName;
        }

        public bool ButtonPressed()
        {
            bool Result;
            if (IsFunctionMethod)
            {
                Type type = TargetObject.GetType();
                MethodInfo method = type.GetMethod(MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (method != null)
                {
                    method.Invoke(TargetObject, MethodParametr);
                    Result = true;
                }
                else
                {
                    Print.LogWarning("Method " + MethodName + " not found on " + TargetObject);
                    Result = false;
                }
            }
            else
            {
                // Get the type of the target object
                Type type = TargetObject.GetType();

                // Find the method with the specified name
                MethodInfo method = type.GetMethod(MethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (method != null)
                {
                    // Invoke the method on the target object
                    method.Invoke(TargetObject, null);
                    Result = true;
                }
                else
                {
                    Print.LogWarning("Method " + MethodName + " not found on " + TargetObject);
                    Result = false;
                }
            }

            return Result;
        }
        public string GetName()
        {
            return Name;
        }
        public string GetDescription()
        {
            return Description;
        }

    }

    [AddComponentMenu("RealMethod/Manager/PrintManager")]
    public sealed class PrintManager : GizmoManager
    {
        private class LogRender : GUIRenderer
        {
            private PrintManager MyOWner;
            public override void Start(GizmoManager Manager)
            {
                MyOWner = Manager as PrintManager;
            }
            public override bool CanRender()
            {
                return MyOWner.Logs.Count > 0;
            }
            public override void Draw()
            {
                for (int i = 0; i < MyOWner.Logs.Count; i++)
                {
                    DrawLog(MyOWner.Logs[i], i);
                }
            }

            private void DrawLog(LogData log, int index)
            {
                int w = Screen.width * log.size;
                int h = Screen.height * log.size;
                float Xpos = log.position.x > 0 ? log.position.x : MyOWner.DefualtPosition.x;
                float Ypos = log.position.y > 0 ? log.position.y : MyOWner.DefualtPosition.y + (index * MyOWner.PrintSpace);

                GUIStyle style = new GUIStyle();
                Rect rect = new Rect(Xpos, Ypos, w, h * 2 / 100);
                style.alignment = TextAnchor.UpperLeft;
                style.fontSize = h * 2 / 100;
                style.normal.textColor = log.TextColor;
                GUI.Label(rect, log.message, style);
            }
        }
        private class ButtonRender : GUIRenderer
        {
            private PrintManager MyOWner;

            public override void Start(GizmoManager Manager)
            {
                MyOWner = Manager as PrintManager;
            }
            public override bool CanRender()
            {
                return MyOWner.Buttons.Count > 0;
            }
            public override void Draw()
            {
                RectOffset padding = GUI.skin.button.padding;
                RectOffset margin = GUI.skin.button.margin;

                // TODO: The height calculation should be done more correctly.
                Rect viewRect = new Rect(0, 0, MyOWner.ButtonSize.x,
                    ((MyOWner.ButtonSize.y + (padding.vertical + margin.vertical)) * MyOWner.Buttons.Count) - MyOWner.ButtonSize.y);


                MyOWner.ScrollPosition = GUI.BeginScrollView(
                    position: new Rect(Screen.width - MyOWner.ButtonSize.x - MyOWner.ButtonMargin, 10, MyOWner.ButtonSize.x + MyOWner.ButtonMargin, Screen.height - 10),
                    scrollPosition: MyOWner.ScrollPosition,
                    viewRect: viewRect,
                    alwaysShowHorizontal: false,
                    alwaysShowVertical: false
                );

                for (int i = 0; i < MyOWner.Buttons.Count; i++)
                {
                    if (GUI.Button(new Rect(0, 50 * i, MyOWner.ButtonSize.x, MyOWner.ButtonSize.y), MyOWner.Buttons[i].GetName()))
                    {
                        MyOWner.Buttons[i].ButtonPressed();
                    }
                }
                GUI.EndScrollView();
            }
        }


        [Header("Log")]
        public float PrintSpace = 20;
        [Header("Button")]
        [SerializeField]
        private Vector2 ScrollPosition = Vector2.zero;
        [SerializeField]
        private Vector2 ButtonSize = new Vector2(200, 40);
        [SerializeField]
        private float ButtonMargin = 20;
        [Header("Advance")]
        [SerializeField, ReadOnly]
        private List<LogData> Logs = new List<LogData>();
        [SerializeField, ReadOnly]
        private List<ButtonData> Buttons = new List<ButtonData>();


        public override void ResolveService(Service service, bool active)
        {
        }
        protected override GUIRenderer[] GetRenderSlots()
        {
            return new GUIRenderer[2] { new LogRender(), new ButtonRender() };
        }

        public void Add(LogData Log)
        {
            if (Log.duration > 0)
            {
                StartCoroutine(RemoveLog(Log));
            }
            Logs.Add(Log);
        }
        public void Add(ButtonData Button)
        {
            Buttons.Add(Button);
        }
        public void Clear()
        {
            Logs.Clear();
            Buttons.Clear();
        }


        // Enumerators
        private IEnumerator RemoveLog(LogData log)
        {
            yield return new WaitForSeconds(log.duration);
            Logs.Remove(log);
        }

    }


}







