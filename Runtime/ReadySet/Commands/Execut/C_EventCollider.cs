using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Commands/Execut/Collide")]
    public class C_EventCollider : ExecutCommand
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
                Debug.LogError("EventCollider_com.Execute: Owner is not a Collider. Event not invoked.");
            }
        }
    }
}