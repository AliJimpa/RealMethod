using System;
using UnityEngine;

namespace RealMethod
{
    // [The base object] Everything that exists in your game world.
    public interface IEntity : IIdentifier
    {
        Component SelfComp
        {
            get
            {
                if (Self is Component comp)
                    return comp;

                throw new InvalidOperationException(
                    $"Invalid IEntity usage on '{GetType().FullName}'. " +
                    "Expected Self to be a UnityEngine.Component but it was not. " +
                    "Ensure IEntity is only implemented on MonoBehaviour or Component-derived classes.");
            }
        }

        string Name => SelfComp.gameObject.name;
        bool IsActive => SelfComp.gameObject.activeSelf;
        int InstanceId => SelfComp.GetInstanceID();
    }
    // [The controllable] Something that can be possessed, given orders, or driven by AI.
    public interface IPawn : IEntity
    {
        IController Controller { get; }
        bool isPossessed { get; }
        event Action<IController> OnPossessed;
        event Action<IController> OnUnpossessed;
        public void Possess(IController controller);
        public void Unpossess();
    }
    // [The mover] A physical entity (humanoid/creature) that moves through the world.
    public interface ICharacter : IPawn
    {
        GameObject GameObject => GameObject;
        Transform Transform => Transform;
        Renderer Renderer { get; }
        ICapsule Capsule { get; }
        Transform Arrow { get; }
        Vector3 Direction => Arrow != null ? Arrow.forward : Transform.forward;
    }
    public interface IController2 : IEntity
    {
        IPawn ControlledAgent { get; }
        bool CanControl { get; }
        bool HasAgent => ControlledAgent != null;
        bool CanPossess(IPawn agent);
    }


    public interface ICapsule
    {
        Rigidbody Body { get; }
        Rigidbody2D Body2D { get; }
        Collider Collider { get; }
        bool Is2D => Body == null && Body2D != null;
        float Mass => Body != null ? Body.mass : Body2D.mass;
        float linearDamping => Body != null ? Body.linearDamping : Body2D.linearDamping;
        float angularDamping => Body != null ? Body.angularDamping : Body2D.angularDamping;
        bool UseGravity
        {
            get => Body != null ? Body.useGravity : Body2D.gravityScale > 0;
            set
            {
                if (Body != null) Body.useGravity = value;
                else Body2D.gravityScale = value ? 1f : 0f;
            }
        }
        bool freezeRotation => Body != null ? Body.freezeRotation : Body2D.freezeRotation;
        byte interpolation => Body != null ? (byte)Body.interpolation : (byte)Body2D.interpolation;
        byte collisionDetectionMode => Body != null ? (byte)Body.collisionDetectionMode : (byte)Body2D.collisionDetectionMode;
        bool IsKinematic => Body != null ? Body.isKinematic : Body2D.bodyType == RigidbodyType2D.Kinematic;
        byte constraints => Body != null ? (byte)Body.constraints : (byte)Body2D.constraints;
        bool IsSleeping => Body != null ? Body.IsSleeping() : Body2D.IsSleeping();
        void Sleep() { if (Body != null) Body.Sleep(); else Body2D.Sleep(); }
        void WakeUp() { if (Body != null) Body.WakeUp(); else Body2D.WakeUp(); }
        RigidbodyType2D BodyType =>
            Body != null
                ? (Body.isKinematic ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic)
                : (Body2D.bodyType == RigidbodyType2D.Static
                    ? RigidbodyType2D.Static
                    : Body2D.bodyType == RigidbodyType2D.Kinematic
                        ? RigidbodyType2D.Kinematic
                        : RigidbodyType2D.Dynamic);

    }
}




//     public abstract class Pawn : Method, IPawn
//     {
//         [Header("Pawn")]
//         public Controller Controller { get; private set; }


//         // Implement IPawn Interface
//         IController IPawn.Controller => Controller;
//         public bool isPossessed => throw new System.NotImplementedException();

//         // Events
//         public event System.Action<IController> OnPossessed;
//         public event System.Action<IController> OnUnpossessed;

//         public void Possess(IController controller)
//         {
//             if(controller != null)
//             {
//                 Controller = controller;
//             }
//         }
//         public void Unpossess()
//         {
//             throw new System.NotImplementedException();
//         }
//     }
//     public abstract class Character : Agent, ICharacter, ICapsule
//     {
//         [Header("Character")]
//         [SerializeField]
//         private Renderer _renderer;
//         [SerializeField]
//         private Collider _collider;
//         private Transform _arrow;
//         [SerializeField]
//         private bool Is2D = false;
//         [SerializeField, ConditionalHide("Is2D", true, false)]
//         private Rigidbody2D _rigidbody2D;
//         [SerializeField, ConditionalHide("Is2D", true, true)]
//         private Rigidbody _rigidbody;


//         // Implement ICharacter Interface
//         Renderer ICharacter.Renderer => _renderer;
//         ICapsule ICharacter.Capsule => this;
//         public Transform Arrow => _arrow;
//         // Implement ICapsule Interface
//         Rigidbody ICapsule.Body => _rigidbody;
//         Rigidbody2D ICapsule.Body2D => _rigidbody2D;
//         Collider ICapsule.Collider => _collider;

//         // Unity Events
//         protected virtual void Awake()
//         {
//             ResolveReferences();
//         }

//         // Method
//         private void ResolveReferences()
//         {
//             if (_renderer == null)
//             {
//                 _renderer ??= GetComponent<Renderer>();
//                 _renderer ??= GetComponentInChildren<Renderer>(true);
//             }
//             if (_rigidbody2D == null)
//             {
//                 _rigidbody2D ??= GetComponent<Rigidbody2D>();
//                 _rigidbody2D ??= GetComponentInChildren<Rigidbody2D>(true);
//             }
//             if (_rigidbody == null)
//             {
//                 _rigidbody ??= GetComponent<Rigidbody>();
//                 _rigidbody ??= GetComponentInChildren<Rigidbody>(true);
//             }
//             if (_collider == null)
//             {
//                 _collider ??= GetComponent<Collider>();
//                 _collider ??= GetComponentInChildren<Collider>(true);
//             }
//         }

// #if UNITY_EDITOR
//         protected virtual void OnValidate()
//         {
//             ResolveReferences();
//         }
// #endif
//     }

//     public abstract class Controller : Method, IController
//     {
//         public IAgent ControlledAgent => throw new System.NotImplementedException();

//         public bool CanControl => throw new System.NotImplementedException();

//         public bool CanPossess(IAgent agent)
//         {
//             throw new System.NotImplementedException();
//         }
//     }
