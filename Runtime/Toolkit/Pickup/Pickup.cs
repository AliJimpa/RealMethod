using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    public interface IPicker
    {
        public bool CanTake(Pickup item);
    }

    public abstract class Pickup : MonoBehaviour
    {
        protected enum PickupBehavior
        {
            Nothing,
            SendMessage,
            UnityEvent,
            Both
        }
        protected enum PickType
        {
            TriggerEnter,
            TriggerExit
        }
    }
    public abstract class Pickup<T> : Pickup where T : Component
    {
        [Header("Basic")]
        [SerializeField]
        protected bool compareTag = true;
        [SerializeField, TagSelector, ConditionalHide("compareTag", true, false)]
        protected string Tag = "Player";
        [SerializeField]
        protected PickType Timing;
        [Space, SerializeField]
        protected PickupBehavior Behavior;
        [SerializeField]
        protected SendMessageOptions SendMessageMethod;
        [SerializeField]
        protected UnityEvent<T> OnPickup;

        /// Private Variable
        protected T m_Collider;

        private void Awake()
        {
            m_Collider = GetComponent<T>();
            if (!m_Collider) Debug.LogWarning("Collider Component cant find");
        }

        protected void CheckPicking(T other, PickType type)
        {
            if (!enabled)
                return;

            if (type != Timing)
                return;

            if (compareTag)
            {
                if (other.CompareTag(Tag))
                {
                    IPicker Picker = other.GetComponent<IPicker>();
                    if (Picker != null)
                    {
                        if (Picker.CanTake(this) && CanPickedUp(other))
                        {
                            PickedUp(other);
                        }
                    }
                    else
                    {
                        if (CanPickedUp(other))
                        {
                            PickedUp(other);
                        }
                    }
                }
            }
            else
            {
                IPicker Picker = other.GetComponent<IPicker>();
                if (Picker != null)
                {
                    if (Picker.CanTake(this) && CanPickedUp(other))
                    {
                        PickedUp(other);
                    }
                }
                else
                {
                    if (CanPickedUp(other))
                    {
                        PickedUp(other);
                    }
                }
            }
        }
        private void PickedUp(T Picker)
        {
            OnPickedUp(Picker);
            switch (Behavior)
            {
                case PickupBehavior.SendMessage:
                    SendMessage("OnPickup", Picker, SendMessageMethod);
                    break;
                case PickupBehavior.UnityEvent:
                    OnPickup.Invoke(Picker);
                    break;
                case PickupBehavior.Both:
                    SendMessage("OnPickup", Picker, SendMessageMethod);
                    OnPickup.Invoke(Picker);
                    break;
                default:
                    break;
            }
        }

        protected abstract void OnPickedUp(T Picker);
        protected abstract bool CanPickedUp(T Picker);
    }



    [RequireComponent(typeof(Collider))]
    public abstract class PickupCollider3D : Pickup<Collider>
    {
        // Unity Methods
        private void OnValidate()
        {
            Collider Sidecollide = GetComponent<Collider>();
            if (Sidecollide)
            {
                Sidecollide.isTrigger = true;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            CheckPicking(other, PickType.TriggerEnter);
        }
        private void OnTriggerExit(Collider other)
        {
            CheckPicking(other, PickType.TriggerExit);
        }
    }

    [RequireComponent(typeof(Collider2D))]
    public abstract class PickupCollider2D : Pickup<Collider2D>
    {
        // Unity Methods
        private void OnValidate()
        {
            Collider2D Sidecollide = GetComponent<Collider2D>();
            if (Sidecollide)
            {
                Sidecollide.isTrigger = true;
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            CheckPicking(collision, PickType.TriggerEnter);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            CheckPicking(collision, PickType.TriggerExit);
        }

    }

}