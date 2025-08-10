using UnityEngine;

namespace RealMethod
{
    public abstract class Trigger : MonoBehaviour
    {
        protected enum TriggerMethods
        {
            Enter = 0,
            Stay = 1,
            Exit = 2,
            Enter_Exit = 3,
            All = 4,
        }
        [Header("Trigger")]
        [SerializeField]
        protected bool CheckTag = true;
        [SerializeField, ConditionalHide("CheckTag", true, false), TagSelector]
        protected string CompairTag = "Player";
        [SerializeField]
        protected TriggerMethods Mode = TriggerMethods.Enter_Exit;
        public TriggerStage CurrentState { get; protected set; } = TriggerStage.None;
        public delegate void TriggerEvent(Component other, TriggerStage stage);
        public TriggerEvent OnTriggered;
    }
    public abstract class Trigger<T> : Trigger where T : Component
    {
        // Virtual Methods
        protected virtual bool CanEnter(T other)
        {
            if (!enabled)
                return false;

            if (Mode == TriggerMethods.Enter || Mode == TriggerMethods.Enter_Exit || Mode == TriggerMethods.All)
            {
                if (CheckTag)
                {
                    return other.CompareTag(CompairTag);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        protected virtual bool CanStay(T other)
        {
            if (!enabled)
                return false;

            if (Mode == TriggerMethods.Stay || Mode == TriggerMethods.All)
            {
                if (CheckTag)
                {
                    return other.CompareTag(CompairTag);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        protected virtual bool CanExit(T other)
        {
            if (!enabled)
                return false;

            if (Mode == TriggerMethods.Exit || Mode == TriggerMethods.Enter_Exit || Mode == TriggerMethods.All)
            {
                if (CheckTag)
                {
                    return other.CompareTag(CompairTag);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        // Abstract Methods
        protected abstract void OnEnter(T other);
        protected abstract void OnStay(T other);
        protected abstract void OnExit(T other);

#if UNITY_EDITOR
        [Header("Draw")]
        public bool _draw = true;
        [ConditionalHide("Draw", true, false)]
        public Color _drawColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
        protected virtual void OnValidate()
        {
            T TargetComp = GetComponent<T>();
            if (TargetComp == null)
                return;
            if (TargetComp is Collider My3D)
            {
                if (!My3D.isTrigger)
                {
                    My3D.isTrigger = true;
                }
            }
            else
            {
                if (TargetComp is Collider2D My2D)
                {
                    if (!My2D.isTrigger)
                    {
                        My2D.isTrigger = true;
                    }
                }
            }
        }
        private void OnDrawGizmos()
        {
            if (_draw)
            {
                T TargetComp = GetComponent<T>();
                if (TargetComp == null)
                    return;

                if (TargetComp is Collider My3D)
                {
                    if (My3D != null && My3D.isTrigger && My3D.enabled)
                    {
                        switch (My3D)
                        {
                            case BoxCollider box:
                                DrawBoxCollider3D(box);
                                break;
                            case SphereCollider sphere:
                                DrawSpherCollider3D(sphere);
                                break;
                            default:
                                Debug.LogWarning("Cant' Draw Debug for This Collider");
                                _draw = false;
                                break;
                        }
                    }
                }
                else
                {
                    if (TargetComp is Collider2D My2D)
                    {
                        if (My2D != null && My2D.isTrigger && My2D.enabled)
                        {
                            switch (My2D)
                            {
                                case BoxCollider2D box:
                                    DrawBoxCollider2D(box);
                                    break;
                                case CircleCollider2D circle:
                                    DrawCircleCollider2D(circle);
                                    break;
                                default:
                                    Debug.LogWarning("Cant' Draw Debug for This Collider");
                                    _draw = false;
                                    break;
                            }
                        }
                    }
                }
            }
        }
        protected virtual void DrawBoxCollider3D(BoxCollider box)
        {
            Gizmos.color = _drawColor;

            // Get box collider properties
            Vector3 center = box.center;
            Vector3 size = box.size;

            // Transform to world position
            Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            Gizmos.matrix = cubeTransform;

            // Draw wire cube
            Gizmos.DrawWireCube(center, size);

            // Optional: Draw a semi-transparent filled cube
            Gizmos.color = new Color(_drawColor.r, _drawColor.g, _drawColor.b, _drawColor.a);
            Gizmos.DrawCube(center, size);
        }
        protected virtual void DrawSpherCollider3D(SphereCollider sphere)
        {
            Vector3 center = sphere.center;
            float radius = sphere.radius;

            Gizmos.DrawWireSphere(center, radius);
            Gizmos.color = new Color(_drawColor.r, _drawColor.g, _drawColor.b, 0.2f);
            Gizmos.DrawSphere(center, radius);
        }
        protected virtual void DrawBoxCollider2D(BoxCollider2D box)
        {

        }
        protected virtual void DrawCircleCollider2D(CircleCollider2D circle)
        {
            
        }
#endif
    }


    [RequireComponent(typeof(Collider))]
    public abstract class Trigger3D : Trigger<Collider>
    {
        // Collider Message
        private void OnTriggerEnter(Collider other)
        {
            if (CanEnter(other))
            {
                CurrentState = TriggerStage.Enter;
                OnEnter(other);
                if (OnTriggered != null)
                    OnTriggered(other, TriggerStage.Enter);
                CurrentState = TriggerStage.None;
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (CanStay(other))
            {
                CurrentState = TriggerStage.Stay;
                OnStay(other);
                if (OnTriggered != null)
                    OnTriggered(other, TriggerStage.Stay);
                CurrentState = TriggerStage.None;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (CanExit(other))
            {
                CurrentState = TriggerStage.Exit;
                OnExit(other);
                if (OnTriggered != null)
                    OnTriggered(other, TriggerStage.Exit);
                CurrentState = TriggerStage.None;
            }
        }
    }

    [RequireComponent(typeof(Collider2D))]
    public abstract class Trigger2D : Trigger<Collider2D>
    {
        // Collider Message
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (CanEnter(collision))
            {
                CurrentState = TriggerStage.Enter;
                OnEnter(collision);
                if (OnTriggered != null)
                    OnTriggered(collision, TriggerStage.Enter);
                CurrentState = TriggerStage.None;
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (CanStay(collision))
            {
                CurrentState = TriggerStage.Stay;
                OnStay(collision);
                if (OnTriggered != null)
                    OnTriggered(collision, TriggerStage.Stay);
                CurrentState = TriggerStage.None;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (CanExit(collision))
            {
                CurrentState = TriggerStage.Exit;
                OnEnter(collision);
                if (OnTriggered != null)
                    OnTriggered(collision, TriggerStage.Exit);
                CurrentState = TriggerStage.None;
            }
        }
    }
}