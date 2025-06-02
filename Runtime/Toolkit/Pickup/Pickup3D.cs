using System;
using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Pickup/Pickup3D")]
    public class Pickup3D : PickupCollider
    {
        public Action<Collider> OnPickedUpByOther;
        protected override bool CanPickedUp(Collider Picker)
        {
            return true;
        }
        protected override void OnPickedUp(Collider Picker)
        {
            OnPickedUpByOther?.Invoke(Picker);
        }
    }
}