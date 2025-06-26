using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Pickup/SamplePickup2D")]
    public sealed class SamplePickup2D : PickupCollider2D
    {
        [Header("Sample")]
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