using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RealMethod
{
    [Serializable]
    public class InputMethod
    {
        [Serializable]
        private struct InputState
        {
            public string InputName;
            public bool Enable;
            public InputAction Action { get; private set; }
            public InputState(string name, bool enable)
            {
                InputName = name;
                Enable = enable;
                Action = null;
            }
            public InputState(string name, bool enable, InputAction action)
            {
                InputName = name;
                Enable = enable;
                Action = action;
            }
            public void SetAction(InputAction action)
            {
                Action = action;
            }
        }
        [SerializeField]
        private InputActionAsset InputAsset;
        [SerializeField]
        private string ActionMap;
        [SerializeField]
        private InputState[] Actions;
        public int Count => Actions.Length;
        public InputActionMap Map { get; private set; }
        public bool IsActive => Map != null;
        public Action<bool> OnInputMethodActive;
        public Action<string> OnMapEnable;
        public Action<string> OnMapDisable;


        public InputAction this[int index]
        {
            get
            {
                if (Actions == null || index < 0 || index >= Actions.Length)
                {
                    throw new ArgumentException($"there isn't any input with this {index} in '{this}'.");
                }
                return Actions[index].Action;
            }
        }
        public InputAction this[string name]
        {
            get
            {
                if (Actions == null || name == string.Empty)
                {
                    throw new ArgumentException($"there isn't any input with this {name} in '{this}'.");
                }
                return Actions[System.Array.IndexOf(Actions, name)].Action;
            }
        }

        // Constructor Class
        public InputMethod()
        {

        }
        public InputMethod(InputActionMap actionMap)
        {
            Map = actionMap;
        }

        // Public Methods
        public void Active()
        {
            if (Map == null)
            {
                Map = InputAsset.FindActionMap(ActionMap, true);
                OnInputMethodActive?.Invoke(true);
            }
        }
        public void Deactive()
        {
            if (Map != null)
            {
                Map = null;
                OnInputMethodActive?.Invoke(false);
            }
        }
        public void EnableMap(bool throwIfNotFound = true)
        {
            if (!IsActive)
                Active();


            if (ActionMap != string.Empty)
            {
                Map.Enable();
                OnMapEnable?.Invoke(ActionMap);
            }
            else
            {
                if (throwIfNotFound)
                {
                    throw new ArgumentException($"ActionMap name is empty in '{this}'.");
                }
            }

        }
        public void DisableMap(bool throwIfNotFound = true)
        {
            if (Map != null)
            {
                Map.Disable();
                OnMapDisable?.Invoke(ActionMap);
            }
        }
        public void AssignInputs(bool autoEnableMap = true, bool throwIfNotFound = true)
        {
            if (autoEnableMap)
            {
                EnableMap();
            }
            else
            {
                if (!IsActive)
                    Active();
            }

            if (Actions != null)
            {
                for (int i = 0; i < Actions.Length; i++)
                {
                    Actions[i].SetAction(Map.FindAction(Actions[i].InputName, true));
                }
            }
            else
            {
                if (throwIfNotFound)
                {
                    throw new ArgumentException($"You didn't have any input for assign in '{this}'.");
                }
            }
        }
        public void EnableInput(string name)
        {
            int targetindex = System.Array.IndexOf(Actions, name);
            Actions[targetindex].Enable = true;
        }
        public void DisableInput(string name)
        {
            int targetindex = System.Array.IndexOf(Actions, name);
            Actions[targetindex].Enable = false;
        }
        T ReadValue<T>(int index) where T : struct
        {
            if (Actions[index].Enable)
            {
                return Actions[index].Action.ReadValue<T>();
            }
            else
            {
                return default;
            }
        }
        T ReadValue<T>(int index, T disableValue) where T : struct
        {
            if (Actions[index].Enable)
            {
                return Actions[index].Action.ReadValue<T>();
            }
            else
            {
                return disableValue;
            }
        }
        T ReadValue<T>(string name) where T : struct
        {
            int targetindex = System.Array.IndexOf(Actions, name);
            if (Actions[targetindex].Enable)
            {
                return Actions[targetindex].Action.ReadValue<T>();
            }
            else
            {
                return default;
            }
        }
        T ReadValue<T>(string name, T disableValue) where T : struct
        {
            int targetindex = System.Array.IndexOf(Actions, name);
            if (Actions[targetindex].Enable)
            {
                return Actions[targetindex].Action.ReadValue<T>();
            }
            else
            {
                return disableValue;
            }
        }
    }
}