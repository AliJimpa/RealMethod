using System;
using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Pickup/Pickup3D")]
    public class Pickup3D : PickupCollider
    {
        public Action<Collider> OnPickedUpBy;
        protected override bool CanPickedUp(Collider Picker)
        {
            return true;
        }
        protected override void OnPickedUp(Collider Picker)
        {
            OnPickedUpBy?.Invoke(Picker);
        }
    }
}