using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Input/DeviceDetection")]
    public sealed class DeviceDetectionComponent : DeviceDetection
    {
        [Header("Events")]
        public UnityEvent<InputDeviceType> OnDeviceDetection;

        // DeviceDetection Methods
        protected override void OnInputUserChange(InputUser user, InputUserChange change, InputDevice device)
        {
            if (change == InputUserChange.DevicePaired ||
                change == InputUserChange.DeviceUnpaired ||
                change == InputUserChange.ControlSchemeChanged)
            {
                CheckDevice();
            }
        }
        protected override void DetectGamepad(Gamepad provider)
        {
            SendMessage("OnDeviceDetection", InputDeviceType.Gamepad, SendMessageOptions.DontRequireReceiver);
            OnDeviceDetection?.Invoke(InputDeviceType.Gamepad);
        }
        protected override void DetectJoystick(Joystick provider)
        {
            SendMessage("OnDeviceDetection", InputDeviceType.Joystick, SendMessageOptions.DontRequireReceiver);
            OnDeviceDetection?.Invoke(InputDeviceType.Joystick);
        }
        protected override void DetectMouse(Mouse provider)
        {
            SendMessage("OnDeviceDetection", InputDeviceType.MouseKeyboard, SendMessageOptions.DontRequireReceiver);
            OnDeviceDetection?.Invoke(InputDeviceType.MouseKeyboard);
        }
        protected override void DetectTouch(Touchscreen provider)
        {
            SendMessage("OnDeviceDetection", InputDeviceType.Touch, SendMessageOptions.DontRequireReceiver);
            OnDeviceDetection?.Invoke(InputDeviceType.Touch);
        }
        protected override void DetectCustom(InputDevice provider)
        {
            SendMessage("OnDeviceDetection", InputDeviceType.Custom, SendMessageOptions.DontRequireReceiver);
            OnDeviceDetection?.Invoke(InputDeviceType.Custom);
        }
    }
}