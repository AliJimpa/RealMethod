using UnityEngine;

namespace RealMethod
{
    [RequireComponent(typeof(Collider)), AddComponentMenu("RealMethod/General/Trigger3D")]
    public sealed class CommandTrigger3D : MonoBehaviour
    {
        private enum TriggerTime
        {
            Enter,
            Exit,
            both,
        }
        [Header("Trigger")]
        [SerializeField]
        private bool CheckTag = true;
        [SerializeField, ConditionalHide("CheckTag", true, false), TagSelector]
        private string CompairTag = "Player";
        [SerializeField]
        private TriggerTime ExecuteTime;
        public TriggerStage CurrentState { get; private set; } = TriggerStage.None;


        // Unity Methods
        private void Awake()
        {
            foreach (var command in GetComponents<ICommandInitiator>())
            {
                command.Initiate(null, this);
            }
        }

        // Collider Message
        void OnTriggerEnter(Collider other)
        {
            if (CheckTag)
            {
                if (!other.CompareTag(CompairTag))
                    return;
            }

            CurrentState = TriggerStage.Enter;
            foreach (var command in GetComponents<ICommandExecuter>())
            {
                command.ExecuteCommand(other);
            }
            CurrentState = TriggerStage.None;
        }
        void OnTriggerExit(Collider other)
        {
            if (CheckTag)
            {
                if (!other.CompareTag(CompairTag))
                    return;
            }

            CurrentState = TriggerStage.Exit;
            foreach (var command in GetComponents<ICommandExecuter>())
            {
                command.ExecuteCommand(other);
            }
            CurrentState = TriggerStage.None;
        }


#if UNITY_EDITOR
        [Header("Debug")]
        public bool Draw = true;
        [ConditionalHide("Draw", true, false)]
        public Color color = new Color(0.5f, 0.5f, 0.5f, 0.4f);
        private void OnValidate()
        {
            if (!GetComponent<Collider>().isTrigger)
            {
                GetComponent<Collider>().isTrigger = true;
            }
        }
        private void OnDrawGizmos()
        {
            if (Draw)
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
                            Draw = false;
                            break;
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
}
