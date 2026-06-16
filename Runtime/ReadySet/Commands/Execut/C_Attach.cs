using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Command/Attach")]
    public sealed class C_Attach : Command
    {
        private enum AttachType
        {
            Executer,
            GameObject,
            Null,
        }
        [SerializeField]
        private AttachType Mode;
        [ConditionalShowByEnum("Mode", AttachType.GameObject)]
        public GameObject Target;

        // ExecutCommand Methods
        protected override bool OnInitiate(Object owner)
        {
            return true;
        }
        protected override bool CanExecute(object Executer)
        {
            return enabled;
        }
        protected override void Execute(object Executer)
        {
            switch (Mode)
            {
                case AttachType.Executer:
                    if (Executer is MonoBehaviour MyOwner)
                    {
                        transform.SetParent(MyOwner.transform);
                    }
                    else
                    {
                        Debug.LogError("Executer is not a MonoBehaviour.");
                    }
                    break;
                case AttachType.GameObject:
                    transform.SetParent(Target.transform);
                    break;
                case AttachType.Null:
                    transform.SetParent(null);
                    break;
            }

        }
    }
}