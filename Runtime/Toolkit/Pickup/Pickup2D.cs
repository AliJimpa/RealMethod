using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [RequireComponent(typeof(Collider2D)), AddComponentMenu("RealMethod/Toolkit/Pickup/Pickup2D")]
    public sealed class Pickup2D : Pickup<Collider2D>
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