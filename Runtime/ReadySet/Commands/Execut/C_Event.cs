using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Commands/Execut/Event")]
    public sealed class C_Event : ExecutCommand
    {
        public UnityEvent<MonoBehaviour> OnExecute;

        // ExecutCommand Methods
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
            if (Owner is MonoBehaviour MyOwner)
            {
                OnExecute?.Invoke(MyOwner);
            }
            else
            {
                Debug.LogError("Event_com.Execute: Owner is not a MonoBehaviour. Event not invoked.");
            }
        }
    }
}