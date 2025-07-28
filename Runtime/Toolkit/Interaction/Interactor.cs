using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RealMethod
{
    public interface IInteraction
    {
        bool IsHover { get; }
        bool CanHover(Interactor Owner);
        bool CanInteract(Interactor Owner);
        void OnHover(Interactor Owner);
        void OnUnhover(Interactor Owner);
        void OnInteract(Interactor Owner);
    }

    [AddComponentMenu("RealMethod/Toolkit/Interaction/Interactor")]
    public class Interactor : MonoBehaviour
    {
        private enum Direction
        {
            Forward = 0,
            Right = 1,
            Back = 3,
            Left = 4
        };
        private enum Raycasttype
        {
            Line = 0,
            Sphere = 1,
        }
        private enum InteractBehavior
        {
            Nothing,
            SendMessage,
            Action,
            Both
        }
        [Header("Setting")]
        [SerializeField]
        private UpdateMethod PerformMethod = UpdateMethod.Update;
        [SerializeField]
        private InteractBehavior Behavior;
        [Header("Raycast")]
        [SerializeField]
        private Raycasttype RayType = Raycasttype.Line;
        [SerializeField]
        private LayerMask Layer;
        [SerializeField]
        private QueryTriggerInteraction QueryTriggerInteraction;
        [ConditionalHide("RayType", true, true)]
        [SerializeField]
        private float RaycastLength = 100;
        [ConditionalHide("RayType", true, false)]
        [SerializeField]
        private float SphereRadius = 1;
        [SerializeField]
        private bool CustomDirection = false;
        [ConditionalHide("CustomDirection", true, true)]
        [SerializeField]
        private Direction directionstype;
        [ConditionalHide("CustomDirection", true, false)]
        [SerializeField]
        private Vector3 direction;
        [Header("Input")]
        [SerializeField]
        private bool UseInputAction = false;
        [ConditionalHide("UseInputAction", true, false)]
        [SerializeField]
        private InputActionReference interactAction;
        [Header("DrawGizmos")]
        [SerializeField]
        private bool Debug = false;
        [SerializeField]
        private Color RayColor = Color.blue;
        [SerializeField]
        private Color HitColor = Color.red;
        [Header("Events")]
        [SerializeField]
        private Action<bool> OnHover;
        [SerializeField]
        private Action<GameObject> Interact;


        private RaycastHit Hitresult = new RaycastHit();
        public RaycastHit hitResult => Hitresult;
        private IInteraction providers;


        // Unity Method
        private void OnEnable()
        {
            // Enable the input action
            if (UseInputAction && interactAction != null)
            {
                interactAction.action.Enable();
                interactAction.action.performed += InteractInput;
            }
        }
        private void LateUpdate()
        {
            if (PerformMethod == UpdateMethod.LateUpdate)
            {
                PerformHover();
            }
        }
        private void Update()
        {
            if (PerformMethod == UpdateMethod.Update)
            {
                PerformHover();
            }
        }
        private void FixedUpdate()
        {
            if (PerformMethod == UpdateMethod.FixedUpdate)
            {
                PerformHover();
            }
        }
        private void OnDisable()
        {
            // Disable the input action when the object is disabled
            if (interactAction != null || UseInputAction)
            {
                interactAction.action.performed -= InteractInput;
                interactAction.action.Disable();
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Debug && RayType == Raycasttype.Sphere && Hitresult.collider)
            {
                Gizmos.color = HitColor;
                Gizmos.DrawWireSphere(Hitresult.point, SphereRadius);
            }
        }
#endif

        // Public Fundions
        public void PerformHover()
        {
            switch (RayType)
            {
                case Raycasttype.Line:
                    CastLineRay(out Hitresult);
                    break;
                case Raycasttype.Sphere:
                    CastSphereRay(out Hitresult);
                    break;
            }

            if (Hitresult.collider != null)
            {
                IInteraction NewProvider = Hitresult.collider.gameObject.GetComponent<IInteraction>();
                if (NewProvider != null)
                {
                    if (NewProvider != providers)
                    {
                        if (providers != null)
                        {
                            providers.OnUnhover(this);
                            ExecuteBehavior(2);
                        }

                        if (NewProvider.CanHover(this))
                        {
                            NewProvider.OnHover(this);
                            ExecuteBehavior(1);
                        }

                        providers = NewProvider;
                    }
                }
                else
                {
                    if (providers != null)
                    {
                        providers.OnUnhover(this);
                        ExecuteBehavior(2);
                        providers = null;
                    }
                }
            }
            else
            {
                if (providers != null)
                {
                    providers.OnUnhover(this);
                    ExecuteBehavior(2);
                    providers = null;
                }
            }
        }
        public bool PerformInteract()
        {
            bool Result = false;
            if (providers != null)
            {
                if (providers.CanInteract(this))
                {
                    providers.OnInteract(this);
                    ExecuteBehavior(3);
                    Result = true;
                }
            }
            return Result;
        }
        public Vector3 GetDirection()
        {
            if (CustomDirection)
            {
                return direction;
            }
            else
            {
                switch (directionstype)
                {
                    case Direction.Forward:
                        return transform.forward;
                    case Direction.Right:
                        return transform.right;
                    case Direction.Back:
                        return -transform.forward;
                    case Direction.Left:
                        return -transform.right;
                    default:
                        return Vector3.zero;
                }
            }
        }
        public void SetDirection(Vector3 NewDirection)
        {
            direction = NewDirection;
        }


        // Private Fundions
        private void InteractInput(InputAction.CallbackContext context)
        {
            PerformInteract();
        }
        private bool CastLineRay(out RaycastHit HitResult)
        {
            bool Result;
            Result = Physics.Raycast(transform.position, GetDirection(), out HitResult, RaycastLength, Layer, QueryTriggerInteraction);
            if (Debug)
            {
                float debuglength = HitResult.collider ? HitResult.distance : RaycastLength;
                Color debugcolor = HitResult.collider ? HitColor : RayColor;
                UnityEngine.Debug.DrawRay(transform.position, GetDirection() * debuglength, debugcolor);
            }
            return Result;
        }
        private bool CastSphereRay(out RaycastHit HitResult)
        {
            bool Result;
            Result = Physics.SphereCast(transform.position, SphereRadius, GetDirection(), out HitResult, RaycastLength, Layer, QueryTriggerInteraction);
            if (Debug)
            {
                float debuglength = HitResult.collider ? HitResult.distance : RaycastLength;
                Color debugcolor = HitResult.collider ? HitColor : RayColor;
                UnityEngine.Debug.DrawRay(transform.position, GetDirection() * debuglength, Color.white);
            }
            return Result;
        }
        private void ExecuteBehavior(int stage)
        {
            if (stage == 1)
            {
                switch (Behavior)
                {
                    case InteractBehavior.Action:
                        OnHover?.Invoke(true);
                        break;
                    case InteractBehavior.SendMessage:
                        gameObject.SendMessage("OnHover", this, SendMessageOptions.RequireReceiver);
                        break;
                    case InteractBehavior.Both:
                        OnHover?.Invoke(true);
                        gameObject.SendMessage("OnHover", this, SendMessageOptions.RequireReceiver);
                        break;
                }
            }
            if (stage == 2)
            {
                switch (Behavior)
                {
                    case InteractBehavior.Action:
                        OnHover?.Invoke(false);
                        break;
                    case InteractBehavior.SendMessage:
                        gameObject.SendMessage("OnUnhover", this, SendMessageOptions.RequireReceiver);
                        break;
                    case InteractBehavior.Both:
                        OnHover?.Invoke(false);
                        gameObject.SendMessage("OnUnhover", this, SendMessageOptions.RequireReceiver);
                        break;
                }
            }
            if (stage == 3)
            {
                switch (Behavior)
                {
                    case InteractBehavior.Action:
                        Interact?.Invoke(gameObject);
                        break;
                    case InteractBehavior.SendMessage:
                        gameObject.SendMessage("OnInteract", this, SendMessageOptions.RequireReceiver);
                        break;
                    case InteractBehavior.Both:
                        OnHover?.Invoke(true);
                        gameObject.SendMessage("OnInteract", this, SendMessageOptions.RequireReceiver);
                        break;
                }
            }
        }
    }

}
