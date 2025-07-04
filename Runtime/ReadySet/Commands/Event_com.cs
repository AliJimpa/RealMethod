using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Commands/Trigger/Event")]
    public class Event_com : ExecutCommand
    {
        public UnityEvent<Collider> OnTrigger;

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
            if (Owner is Collider col)
            {
                OnTrigger?.Invoke(col);
            }
            else
            {
                Debug.LogError("Event_com.Execute: Owner is not a Collider. Event not invoked.");
            }
        }
    }
}