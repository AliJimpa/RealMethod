using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [RequireComponent(typeof(Collider)), AddComponentMenu("RealMethod/Toolkit/Pickup/Pickup3D")]
    public sealed class Pickup3D : Pickup<Collider>
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