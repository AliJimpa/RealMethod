using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Commands/Trigger/DeathZone")]
    public sealed class Death_com : ExecutCommand
    {
        [Header("Events")]
        [SerializeField]
        private bool UseInterface = true;
        [Header("Events")]
        [ConditionalHide("UseInterface", true, true)]
        public UnityEvent<Death_com> OnDeath;

        // Base ExecutCommand Methods
        protected override bool OnInitiate(Object author, Object owner)
        {
            return true;
        }
        protected override bool CanExecute(object Owner)
        {
            return true;
        }
        protected override void Execute(object Owner)
        {
            if (UseInterface)
            {
                if (Owner is MonoBehaviour Mono)
                {
                    IDamage interfce = Mono.gameObject.GetComponent<IDamage>();
                    if (interfce != null)
                    {
                        interfce.Die();
                    }
                    else
                    {
                        Debug.LogError("IDamage interface not found on the Owner GameObject.");
                    }
                }
                else
                {
                    Debug.LogError($"This command ({nameof(Death_com)}) should Execute by Monobehaviort");
                }
            }
            else
            {
                OnDeath?.Invoke(this);
            }

        }


    }

}