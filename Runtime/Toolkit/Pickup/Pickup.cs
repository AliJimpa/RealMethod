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
        protected sealed override bool CanEnter(T other)
        {
            if (!base.CanEnter(other))
                return false;

            return CheckPicking(other);
        }
        protected sealed override bool CanStay(T other)
        {
            if (!base.CanStay(other))
                return false;

            return CheckPicking(other);
        }
        protected sealed override bool CanExit(T other)
        {
            if (!base.CanExit(other))
                return false;

            return CheckPicking(other);
        }
        protected sealed override void OnEnter(T other)
        {
            PickedUp(other);
        }
        protected sealed override void OnExit(T other)
        {
            PickedUp(other);
        }
        protected sealed override void OnStay(T other)
        {
            OnPickUp(other);
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
            CurrentState = TriggerStage.Enter;
            if (CanEnter(other))
            {
                OnEnter(other);
            }
            CurrentState = TriggerStage.None;
        }
        private void OnTriggerStay(Collider other)
        {
            CurrentState = TriggerStage.Stay;
            if (CanStay(other))
            {
                OnStay(other);
            }
            CurrentState = TriggerStage.None;
        }
        private void OnTriggerExit(Collider other)
        {
            CurrentState = TriggerStage.Exit;
            if (CanExit(other))
            {
                OnExit(other);
            }
            CurrentState = TriggerStage.None;
        }

    }
    [RequireComponent(typeof(Collider2D))]
    public abstract class Pickup2D : Pickup<Collider2D>
    {
        // Collider Message
        private void OnTriggerEnter2D(Collider2D collision)
        {
            CurrentState = TriggerStage.Enter;
            if (CanEnter(collision))
            {
                OnEnter(collision);
            }
            CurrentState = TriggerStage.None;
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            CurrentState = TriggerStage.Stay;
            if (CanStay(collision))
            {
                OnStay(collision);
            }
            CurrentState = TriggerStage.None;
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            CurrentState = TriggerStage.Exit;
            if (CanExit(collision))
            {
                OnEnter(collision);
            }
            CurrentState = TriggerStage.None;
        }
    }

}