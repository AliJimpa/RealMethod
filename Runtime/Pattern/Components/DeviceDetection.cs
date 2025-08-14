using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace RealMethod
{
    public enum InputDeviceType
    {
        Touch,
        MouseKeyboard,
        Gamepad,
        Joystick,
        Custom,
    }

    public abstract class DeviceDetection : MonoBehaviour
    {
        [Header("Setting")]
        [SerializeField]
        private bool CheckOnAwake = true;
        [Space]
        [SerializeField]
        private bool Detect_Touch = true;
        [SerializeField]
        private bool Detect_MouseKeyboard = true;
        [SerializeField]
        private bool Detect_Gamepad = true;
        [SerializeField]
        private bool Detect_Joystick = true;
        [SerializeField]
        private bool Detect_Custom = true;
        [Header("Resource")]
        [SerializeField]
        private InputActionAsset inputAsset;

        // Unity Methods
        private void Awake()
        {
            if (CheckOnAwake)
            {
                // Initial check
                CheckDevice();
            }
        }
        private void OnEnable()
        {
            // Start listening for device changes
            InputUser.onChange += OnInputUserChange;
        }
        private void OnDisable()
        {
            InputUser.onChange -= OnInputUserChange;
        }

        // Protected Functions
        protected void CheckDevice()
        {
            foreach (var dev in InputSystem.devices)
            {
                switch (dev)
                {
                    case Touchscreen touch:
                        if (Detect_Touch)
                            DetectTouch(touch);
                        break;
                    case Mouse mouse:
                        if (Detect_MouseKeyboard)
                            DetectMouse(mouse);
                        break;
                    case Gamepad gamepad:
                        if (Detect_Gamepad)
                            DetectGamepad(gamepad);
                        break;
                    case Joystick joystick:
                        if (Detect_Joystick)
                            DetectJoystick(joystick);
                        break;
                    default:
                        if (Detect_Custom)
                            DetectCustom(dev);
                        break;
                }
            }
        }

        // Abstract Methods
        protected abstract void OnInputUserChange(InputUser user, InputUserChange change, InputDevice device);
        protected abstract void DetectTouch(Touchscreen provider);
        protected abstract void DetectMouse(Mouse provider);
        protected abstract void DetectGamepad(Gamepad provider);
        protected abstract void DetectJoystick(Joystick provider);
        protected abstract void DetectCustom(InputDevice provider);
    }
}