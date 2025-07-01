using System;
using UnityEngine;

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
            Action,
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
        protected PickType State;
        [SerializeField]
        protected PickupBehavior Behavior;

        // Action
        public Action<T> OnPickedUp;

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

            if (type != State)
                return;

            if (compareTag)
            {
                if (other.CompareTag(Tag))
                {
                    IPicker Picker = other.GetComponent<IPicker>();
                    if (Picker != null)
                    {
                        if (Picker.CanTake(this) && CanPickUp(other))
                        {
                            PickedUp(other);
                        }
                    }
                    else
                    {
                        if (CanPickUp(other))
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
                    if (Picker.CanTake(this) && CanPickUp(other))
                    {
                        PickedUp(other);
                    }
                }
                else
                {
                    if (CanPickUp(other))
                    {
                        PickedUp(other);
                    }
                }
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

        protected abstract void OnPickUp(T Picker);
        protected abstract bool CanPickUp(T Picker);
    }



    [RequireComponent(typeof(Collider))]
    public abstract class PickupCollider3D : Pickup<Collider>
    {
        // Unity Methods
        protected virtual void OnValidate()
        {
            Collider Sidecollide = GetComponent<Collider>();
            if (Sidecollide)
            {
                if (Sidecollide is MeshCollider m_meshcollider)
                {
                    m_meshcollider.convex = true;
                    m_meshcollider.isTrigger = true;
                }
                else
                {
                    Sidecollide.isTrigger = true;
                }
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


#if UNITY_EDITOR
        [Header("Debug")]
        public bool draw = true;
        [ConditionalHide("draw", true, false)]
        public Color color = new Color(0, 1, 0, 0.2f);

        private void OnDrawGizmos()
        {
            if (draw)
            {
                Collider Sidecollide = GetComponent<Collider>();
                if (Sidecollide != null && Sidecollide.isTrigger && Sidecollide.enabled)
                {
                    switch (Sidecollide)
                    {
                        case BoxCollider box:
                            DrawBoxCollider(box);
                            break;
                        case SphereCollider sphere:
                            DrawSpherCollider(sphere);
                            break;
                        default:
                            Debug.LogWarning("Cant' Draw Debug for This Collider");
                            draw = false;
                            break;
                    }
                }
            }
        }
        protected void DrawBoxCollider(BoxCollider boxCollider)
        {
            Gizmos.color = color;

            // Get box collider properties
            Vector3 center = boxCollider.center;
            Vector3 size = boxCollider.size;

            // Transform to world position
            Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            Gizmos.matrix = cubeTransform;

            // Draw wire cube
            Gizmos.DrawWireCube(center, size);

            // Optional: Draw a semi-transparent filled cube
            Gizmos.color = new Color(color.r, color.g, color.b, color.a);
            Gizmos.DrawCube(center, size);
        }
        protected void DrawSpherCollider(SphereCollider sphere)
        {
            Vector3 center = sphere.center;
            float radius = sphere.radius;

            Gizmos.DrawWireSphere(center, radius);
            Gizmos.color = new Color(color.r, color.g, color.b, 0.2f);
            Gizmos.DrawSphere(center, radius);
        }
#endif


    }

    [RequireComponent(typeof(Collider2D))]
    public abstract class PickupCollider2D : Pickup<Collider2D>
    {
        // Unity Methods
        protected virtual void OnValidate()
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