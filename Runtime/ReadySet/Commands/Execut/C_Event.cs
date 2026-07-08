using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Command/Event")]
    public sealed class C_Event : Command
    {
        public UnityEvent<MonoBehaviour> OnExecute;

        // ExecutCommand Methods
        protected override bool OnInitiate( Object owner)
        {
            return true;
        }
        protected override bool CanExecute(object Executer)
        {
            return enabled;
        }
        protected override void Execute(object Executer)
        {
            if (Executer is MonoBehaviour MyOwner)
            {
                OnExecute?.Invoke(MyOwner);
            }
            else
            {
                Debug.LogError("Event_com.Execute: Executer is not a MonoBehaviour. Event not invoked.");
            }
        }
    }
}