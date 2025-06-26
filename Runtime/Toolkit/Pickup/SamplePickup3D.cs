using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Pickup/SamplePickup3D")]
    public sealed class SamplePickup3D : PickupCollider3D
    {
        [Header("Sample")]
        public UnityEvent<Collider> OnPickedUpBy;
        protected override bool CanPickUp(Collider Picker)
        {
            return enabled;
        }
        protected override void OnPickUp(Collider Picker)
        {
            OnPickedUpBy?.Invoke(Picker);
        }
    }
}