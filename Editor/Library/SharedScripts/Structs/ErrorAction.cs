using System;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public struct ErrorAction
    {
        public bool IsValid => string.IsNullOrEmpty(ErrorMessage);
        public string ErrorMessage { get; private set; }
        private Action<int> OnFixed;
        private int ID;


        public void Create(string message, int id, Action<int> callback)
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
                ErrorMessage = string.Empty;
                OnFixed?.Invoke(ID);
            }
        }

    }
}
