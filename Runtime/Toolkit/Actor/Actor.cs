using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Actor/Actor")]
    public class Actor : MonoBehaviour
    {
        private enum ActorType
        {
            None,
            LeadingActor,
            SupportingActor
        }


        [Header("Setting")]
        [SerializeField]
        private Act Role;
        [SerializeField]
        private ActorType Pose;


        public Actor Partner { get; private set; }
        public Act CurrentRole => Role;
        public bool Acting => Role != null;



        public bool PlayAct(Act role, Actor partner)
        {
            if (Role == null)
            {
                Role = role;
                Partner = partner;
                return true;
            }
            else
            {
                return false;
            }
        }

    }


    public abstract class Act : LifecycleCommand
    {
        protected override bool CanUpdate()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnBegin()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnEnd()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnInitiate()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnUpdate()
        {
            throw new System.NotImplementedException();
        }
    }
}