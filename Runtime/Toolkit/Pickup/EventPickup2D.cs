using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Pickup/Pickup2D")]
    public sealed class EventPickup2D : Pickup2D
    {
        [Header("Event")]
        public UnityEvent<Collider2D> OnPickedUpBy;
        protected override bool CanPickUp(Collider2D Picker)
        {
            return enabled;
        }
        protected override void OnPickUp(Collider2D Picker)
        {
            OnPickedUpBy?.Invoke(Picker);
        }


    }
}