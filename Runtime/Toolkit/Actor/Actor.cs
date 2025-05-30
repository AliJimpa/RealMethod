using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/ToolKit/Actor/Actor")]
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
}