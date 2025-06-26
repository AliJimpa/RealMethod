using System;
using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Pickup/SamplePickup3D")]
    public sealed class SamplePickup3D : PickupCollider3D
    {
        public Action<Collider> OnPickedUpBy;
        protected override bool CanPickedUp(Collider Picker)
        {
            return enabled;
        }
        protected override void OnPickedUp(Collider Picker)
        {
            OnPickedUpBy?.Invoke(Picker);
        }
    }
}