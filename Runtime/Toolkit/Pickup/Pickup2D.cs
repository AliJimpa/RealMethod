using System;
using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Pickup/Pickup2D")]
    public sealed class Pickup2D : PickupCollider2D
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