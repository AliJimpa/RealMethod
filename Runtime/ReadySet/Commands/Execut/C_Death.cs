using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Command/Death")]
    public sealed class C_Death : Command
    {
        [Header("Setting")]
        [SerializeField]
        private bool UseExecuter = true;
        [SerializeField, ConditionalHide("UseExecuter", true, true)]
        private GameObject Target;


        // Base ExecutCommand Methods
        protected override bool OnInitiate(Object owner)
        {
            return true;
        }
        protected override bool CanExecute(object Owner)
        {
            return enabled;
        }
        protected override void Execute(object Executer)
        {

            if (UseExecuter)
            {
                if (Executer is MonoBehaviour Mono)
                {
                    ApplyDeath(Mono.gameObject);
                }
                else
                {
                    Debug.LogError($"This command ({nameof(C_Death)}) should Execute by Monobehaviort");
                }
            }
            else
            {
                ApplyDeath(Target);
            }

        }

        private void ApplyDeath(GameObject target)
        {
            if (target == null)
            {
                Debug.LogError("Object for die is not valid");
                return;
            }

            target.Death();
        }

    }

}