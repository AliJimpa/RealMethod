using System;
using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Pickup/SamplePickup2D")]
    public sealed class SamplePickup2D : PickupCollider2D
    {
        public Action<Collider2D> OnPickedUpBy;
        protected override bool CanPickedUp(Collider2D Picker)
        {
            return enabled;
        }
        protected override void OnPickedUp(Collider2D Picker)
        {
            OnPickedUpBy?.Invoke(Picker);
        }
    }
}