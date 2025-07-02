using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Pickup/Pickup3D")]
    public sealed class EventPickup3D : Pickup3D
    {
        [Header("Event")]
        public UnityEvent<Collider> OnPickedUpBy;
        protected override bool CanPickUp(Collider Picker)
        {
            return enabled;
        }
        protected override void OnStayPicking()
        {

        }
        protected override void OnPickUp(Collider Picker)
        {
            OnPickedUpBy?.Invoke(Picker);
        }
    }
}