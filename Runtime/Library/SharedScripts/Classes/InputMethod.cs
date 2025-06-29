using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RealMethod
{
    [Serializable]
    public class InputMethod
    {
        [SerializeField]
        private InputActionAsset InputAsset;
        [SerializeField]
        private string ActionMap;
        [SerializeField]
        private string[] Inputs;

        public InputActionMap Map { get; private set; }
        private InputAction[] Actions;

        public InputAction this[int index]
        {
            get
            {
                if (Actions == null || index < 0 || index >= Actions.Length)
                {
                    Debug.LogError($"InputMethod: Input Not Match [You have to call MatchInputs()]");
                    throw new IndexOutOfRangeException();
                }
                return Actions[index];
            }
        }
        public InputAction this[string name]
        {
            get
            {
                if (Actions == null || name == string.Empty)
                {
                    Debug.LogError($"InputMethod: Input Not Match [You have to call MatchInputs()]");
                    throw new IndexOutOfRangeException();
                }
                return Actions[System.Array.IndexOf(Inputs, name)];
            }
        }

        public void EnableMap(bool CheckValidate = false)
        {
            if (CheckValidate && Map != null)
            {
                Debug.LogWarning("You have already Map First DisableMap");
                return;
            }

            if (ActionMap != string.Empty)
            {
                Map = InputAsset.FindActionMap(ActionMap, true);
                Map.Enable();
            }
            else
            {
                Debug.LogError("InputMethod: ActionMap name is empty. Cannot enable InputActionMap.");
            }

            Actions = new InputAction[Inputs.Length];
        }
        public void DisableMap()
        {
            if (Map != null)
            {
                Map.Disable();
            }
            else
            {
                Debug.LogError("InputMethod: No InputActionMap to disable. [FirstEnabled]");
            }
        }

        public void MatchInputs()
        {
            if (Map != null && Inputs != null)
            {
                for (int i = 0; i < Inputs.Length; i++)
                {
                    Actions[i] = Map.FindAction(Inputs[i], true);
                }
            }
            else
            {
                Debug.LogError("InputMethod: Map or Inputs is null. Cannot match inputs.[EnableMap & Write your Inputs Name]");
                return;
            }
        }

    }
}