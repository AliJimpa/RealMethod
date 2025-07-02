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
        protected TriggerMethods Call = TriggerMethods.Enter_Exit;
        public TriggerStage CurrentState { get; protected set; } = TriggerStage.None;
    }
    public abstract class Trigger<T> : Trigger where T : Component
    {
        // Virtual Methods
        protected virtual bool CanEnter(T other)
        {
            if (!enabled)
                return false;

            if (Call == TriggerMethods.Enter || Call == TriggerMethods.Enter_Exit || Call == TriggerMethods.All)
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

            if (Call == TriggerMethods.Stay || Call == TriggerMethods.All)
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

            if (Call == TriggerMethods.Exit || Call == TriggerMethods.Enter_Exit || Call == TriggerMethods.All)
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
        [Header("Debug")]
        public bool Draw = true;
        [ConditionalHide("Draw", true, false)]
        public Color color = new Color(0.5f, 0.5f, 0.5f, 0.4f);
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
            if (Draw)
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
                                DrawBoxCollider(box);
                                break;
                            case SphereCollider sphere:
                                DrawSpherCollider(sphere);
                                break;
                            default:
                                Debug.LogWarning("Cant' Draw Debug for This Collider");
                                Draw = false;
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
                            // switch (Sidecollide)
                            // {
                            //     case BoxCollider box:
                            //         DrawBoxCollider(box);
                            //         break;
                            //     case SphereCollider sphere:
                            //         DrawSpherCollider(sphere);
                            //         break;
                            //     default:
                            //         Debug.LogWarning("Cant' Draw Debug for This Collider");
                            //         Draw = false;
                            //         break;
                            // }
                        }
                    }
                }
            }
        }
        private void DrawBoxCollider(BoxCollider boxCollider)
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
        private void DrawSpherCollider(SphereCollider sphere)
        {
            Vector3 center = sphere.center;
            float radius = sphere.radius;

            Gizmos.DrawWireSphere(center, radius);
            Gizmos.color = new Color(color.r, color.g, color.b, 0.2f);
            Gizmos.DrawSphere(center, radius);
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
    public abstract class Trigger2D : Trigger<Collider2D>
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