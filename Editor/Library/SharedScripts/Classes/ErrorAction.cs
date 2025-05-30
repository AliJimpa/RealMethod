using System;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class ErrorAction
    {
        public string ErrorMessage { get; private set; }
        private Action<int> OnFixed;
        private int ID = 0;

        public ErrorAction(string message, int id, Action<int> callback)
        {
            ErrorMessage = message;
            ID = id;
            OnFixed = callback;
        }

        public void RenderError()
        {
            EditorGUILayout.HelpBox(ErrorMessage, MessageType.Error);

            if (GUILayout.Button("Fix Issue"))
            {
                OnFixed?.Invoke(ID);
            }
        }

    }
}
