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
                    if (OnTriggered != null)
                        OnTriggered(Picker, TriggerStage.None);
                    break;
                case PickupBehavior.Both:
                    SendMessage("OnPickedUp", Picker, SendMessageOptions.RequireReceiver);
                    if (OnTriggered != null)
                        OnTriggered(Picker, TriggerStage.None);
                    break;
                default:
                    break;
            }
        }

        // Abstract Method
        protected abstract bool CanPickUp(T Picker);
        protected abstract void OnPickUp(T Picker);

    }


    [RequireComponent(typeof(Collider))]
    public abstract class Pickup3D : Pickup<Collider>
    {
        // Collider Message
        private void OnTriggerEnter(Collider other)
        {
            if (CanEnter(other))
            {
                CurrentState = TriggerStage.Enter;
                OnEnter(other);
                CurrentState = TriggerStage.None;
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (CanStay(other))
            {
                CurrentState = TriggerStage.Stay;
                OnStay(other);
                CurrentState = TriggerStage.None;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (CanExit(other))
            {
                CurrentState = TriggerStage.Exit;
                OnExit(other);
                CurrentState = TriggerStage.None;
            }
        }

    }
    [RequireComponent(typeof(Collider2D))]
    public abstract class Pickup2D : Pickup<Collider2D>
    {
        // Collider Message
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (CanEnter(collision))
            {
                CurrentState = TriggerStage.Enter;
                OnEnter(collision);
                CurrentState = TriggerStage.None;
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (CanStay(collision))
            {
                CurrentState = TriggerStage.Stay;
                OnStay(collision);
                CurrentState = TriggerStage.None;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (CanExit(collision))
            {
                CurrentState = TriggerStage.Exit;
                OnEnter(collision);
                CurrentState = TriggerStage.None;
            }
        }
    }

}