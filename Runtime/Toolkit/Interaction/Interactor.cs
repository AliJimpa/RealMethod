using UnityEngine;
using UnityEngine.InputSystem;

namespace RealMethod
{
    public interface IInteractable
    {
        public bool CanHover();
        public bool CanInteract();
        public void OnHover(Interactor Owner);
        public void OnUnhover(Interactor Owner);
        public void OnInteract(Interactor Owner);

    }

    public enum InteractionMethod
    {
        Component,
        Interface
    }
    public enum Direction
    {
        Forward = 0,
        Right = 1,
        Back = 3,
        Left = 4
    };
    public enum UpdateMethod
    {
        None = 0,
        LateUpdate = 2,
        Update = 3,
        FixedUpdate = 4

    };
    public enum Raycasttype
    {
        Line = 0,
        Sphere = 1,
    }

    [AddComponentMenu("RealMethod/Toolkit/Interaction/Interactor")]
    public class Interactor : MonoBehaviour
    {
        [Header("Interaction")]
        [SerializeField]
        private UpdateMethod PerformMethod = UpdateMethod.Update;
        [SerializeField]
        private InteractionMethod DetectionMethod = InteractionMethod.Component;


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


        [Header("Debug")]
        [SerializeField]
        private bool Visible = false;
        [SerializeField]
        private Color RayColor = Color.blue;
        [SerializeField]
        private Color HitColor = Color.red;


        /// Private Variables
        private GameObject CurrentObject;
        private Interactable CurrentComp;
        private RaycastHit Hitresult = new RaycastHit();



        // Internal Method
        private void OnEnable()
        {
            // Enable the input action
            if (UseInputAction && interactAction != null)
            {
                interactAction.action.Enable();
                interactAction.action.performed += InteractInput;
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


        // Update Method
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



        // Public Method
        public bool PerformInteract()
        {
            bool Result = false;
            switch (DetectionMethod)
            {
                case InteractionMethod.Component:
                    if (CurrentComp && CurrentComp.CanInteract(this))
                    {
                        CurrentComp.Interacted(this);
                        Result = true;
                    }
                    else
                    {
                        Result = false;
                    }
                    break;
                case InteractionMethod.Interface:
                    if (CurrentObject)
                    {
                        IInteractable[] CurrentInterfaces = CurrentObject.GetComponents<IInteractable>();
                        foreach (var IFace in CurrentInterfaces)
                        {
                            if (IFace.CanInteract())
                            {
                                IFace.OnInteract(this);
                                Result = true;
                            }
                        }
                    }
                    else
                    {
                        Result = false;
                    }
                    break;
            }
            return Result;
        }

        public void PerformHover()
        {

            bool CastingRay = false;

            switch (RayType)
            {
                case Raycasttype.Line:
                    CastingRay = CastLineRay(out Hitresult);
                    break;
                case Raycasttype.Sphere:
                    CastingRay = CastSphereRay(out Hitresult);
                    break;
            }

            if (CastingRay)
            {
                if (DetectionMethod == InteractionMethod.Component)
                {
                    Interactable NewComp = Hitresult.collider.GetComponent<Interactable>();
                    if (NewComp != null)
                    {
                        if (NewComp != CurrentComp)
                        {
                            if (CurrentComp)
                            {
                                CurrentComp.UnHoverd(this);
                            }

                            if (NewComp.CanHover(this))
                            {
                                NewComp.Hoverd(this);
                                CurrentComp = NewComp;
                            }
                            else
                            {
                                CurrentComp = null;
                            }
                        }
                    }
                    else
                    {
                        if (CurrentComp)
                        {
                            CurrentComp.UnHoverd(this);
                            CurrentComp = null;
                        }
                    }
                }

                if (DetectionMethod == InteractionMethod.Interface)
                {
                    GameObject NewObject = Hitresult.collider.gameObject;
                    if (NewObject != null)
                    {
                        if (NewObject != CurrentObject)
                        {
                            if (CurrentObject)
                            {
                                IInteractable[] CurrentInterfaces = CurrentObject.GetComponents<IInteractable>();
                                foreach (var IFace in CurrentInterfaces)
                                {
                                    IFace.OnUnhover(this);
                                }
                            }

                            IInteractable[] NewInterfaces = NewObject.GetComponents<IInteractable>();
                            foreach (var IFace in NewInterfaces)
                            {
                                if (IFace.CanHover())
                                    IFace.OnHover(this);
                            }

                            CurrentObject = NewObject;
                        }
                    }
                    else
                    {
                        if (CurrentObject)
                        {
                            IInteractable[] CurrentInterfaces = CurrentObject.GetComponents<IInteractable>();
                            foreach (var IFace in CurrentInterfaces)
                            {
                                IFace.OnUnhover(this);
                            }
                            CurrentObject = null;
                        }
                    }
                }
            }
            else
            {
                if (CurrentComp)
                {
                    CurrentComp.UnHoverd(this);
                    CurrentComp = null;
                }

                if (CurrentObject)
                {
                    IInteractable[] CurrentInterfaces = CurrentObject.GetComponents<IInteractable>();
                    foreach (var IFace in CurrentInterfaces)
                    {
                        IFace.OnUnhover(this);
                    }
                    CurrentObject = null;
                }
            }
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

        public void UpdateDirection(Vector3 NewDirection)
        {
            direction = NewDirection;
        }

        public RaycastHit GetHitResult()
        {
            return Hitresult;
        }



        // Private Method
        private void InteractInput(InputAction.CallbackContext context)
        {
            PerformInteract();
        }

        private bool CastLineRay(out RaycastHit HitResult)
        {
            bool Result;
            Result = Physics.Raycast(transform.position, GetDirection(), out HitResult, RaycastLength, Layer, QueryTriggerInteraction);
            if (Visible)
            {
                float debuglength = HitResult.collider ? HitResult.distance : RaycastLength;
                Color debugcolor = HitResult.collider ? HitColor : RayColor;
                Debug.DrawRay(transform.position, GetDirection() * debuglength, debugcolor);
            }
            return Result;
        }

        private bool CastSphereRay(out RaycastHit HitResult)
        {
            bool Result;
            Result = Physics.SphereCast(transform.position, SphereRadius, GetDirection(), out HitResult, RaycastLength, Layer, QueryTriggerInteraction);
            if (Visible)
            {
                float debuglength = HitResult.collider ? HitResult.distance : RaycastLength;
                Color debugcolor = HitResult.collider ? HitColor : RayColor;
                Debug.DrawRay(transform.position, GetDirection() * debuglength, Color.white);
            }
            return Result;
        }

        private void OnDrawGizmos()
        {
            if (Visible && RayType == Raycasttype.Sphere && Hitresult.collider)
            {
                Gizmos.color = HitColor;
                Gizmos.DrawWireSphere(Hitresult.point, SphereRadius);
            }

        }
    }



}
