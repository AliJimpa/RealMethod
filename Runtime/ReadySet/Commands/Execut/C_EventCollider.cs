using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Command/Collide")]
    public sealed class C_EventCollider : Command
    {
        public UnityEvent<Collider> OnTrigger;

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
            if (Executer is Collider col)
            {
                OnTrigger?.Invoke(col);
            }
            else
            {
                Debug.LogError("EventCollider_com.Execute: Executer is not a Collider. Event not invoked.");
            }
        }
    }
}