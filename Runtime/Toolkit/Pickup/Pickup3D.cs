using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Pickup/Pickup3D")]
    public class Pickup3D : PickupCollider
    {
        public UnityEvent<Collider> OnPickedUpBy;
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