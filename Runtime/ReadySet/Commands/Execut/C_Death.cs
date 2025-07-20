using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Commands/Execut/Death")]
    public sealed class C_Death : ExecutCommand
    {
        [Header("Setting")]
        [SerializeField]
        private bool UseExecuter = true;
        [SerializeField, ConditionalHide("UseExecuter", true, true)]
        private GameObject Target;
        [Header("Method")]
        [SerializeField]
        private bool WithInterface = true;
        [SerializeField]
        private bool WithSendMessage = false;


        // Base ExecutCommand Methods
        protected override bool OnInitiate(Object author, Object owner)
        {
            return true;
        }
        protected override bool CanExecute(object Owner)
        {
            return enabled;
        }
        protected override void Execute(object Owner)
        {

            if (UseExecuter)
            {
                if (Owner is MonoBehaviour Mono)
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

            // Interface
            if (WithInterface)
            {
                IDamage interfce = target.GetComponent<IDamage>();
                if (interfce != null)
                {
                    interfce.Die();
                }
                else
                {
                    Debug.LogError($"IDamage interface not found on the {target}.");
                }
            }

            // SendMessage
            if (WithSendMessage)
            {
                target.SendMessage("Die", SendMessageOptions.RequireReceiver);
            }
        }

    }

}