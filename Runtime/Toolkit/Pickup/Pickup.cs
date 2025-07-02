using System;
using UnityEngine;

namespace RealMethod
{
    public interface IPicker
    {
        public bool CanTake(Trigger trigger);
    }
    public abstract class Pickup<T> : Trigger<T> where T : Component
    {
        private enum PickupBehavior
        {
            Nothing,
            SendMessage,
            Action,
            Both
        }
        [Header("Pickup")]
        [SerializeField]
        private PickupBehavior Behavior;

        // Action
        public Action<T> OnPickedUp;

        // Trigger Methods
        protected override bool CanEnter(T other)
        {
            if (!base.CanEnter(other))
                return false;

            return CheckPicking(other);
        }
        protected override bool CanStay(T other)
        {
            if (!base.CanStay(other))
                return false;

            return CheckPicking(other);
        }
        protected override bool CanExit(T other)
        {
            if (!base.CanExit(other))
                return false;

            return CheckPicking(other);
        }
        protected override void OnEnter(T other)
        {
            PickedUp(other);
        }
        protected override void OnStay(T other)
        {
            PickedUp(other);
        }
        protected override void OnExit(T other)
        {
            PickedUp(other);
        }

        // Private Functions
        private bool CheckPicking(T other)
        {
            IPicker Picker = other.GetComponent<IPicker>();
            if (Picker != null)
            {
                return Picker.CanTake(this) && CanPickUp(other);
            }
            else
            {
                return CanPickUp(other);
            }
        }
        private void PickedUp(T Picker)
        {
            OnPickUp(Picker);
            switch (Behavior)
            {
                case PickupBehavior.SendMessage:
                    SendMessage("OnPickedUp", Picker, SendMessageOptions.RequireReceiver);
                    break;
                case PickupBehavior.Action:
                    OnPickedUp?.Invoke(Picker);
                    break;
                case PickupBehavior.Both:
                    SendMessage("OnPickedUp", Picker, SendMessageOptions.RequireReceiver);
                    OnPickedUp?.Invoke(Picker);
                    break;
                default:
                    break;
            }
        }

        // Abstract Method
        protected abstract void OnPickUp(T Picker);
        protected abstract bool CanPickUp(T Picker);
    }

}