using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Ability/AbilityController")]
    public sealed class AbilityControllerComponent : AbilityController
    {
        [Header("Raycast")]
        [SerializeField]
        private Transform aimTransform;
        [SerializeField]
        private float RaycastLength = 100;
        [Header("DrawGizmos")]
        [SerializeField]
        private bool debug = false;
        [SerializeField]
        private Color RayColor = Color.blue;
        [SerializeField]
        private Color HitColor = Color.red;

        public override IAbilityContext GetTarget(AbilityAction ability)
        {
            // Simplified raycast targeting
            if (Physics.Raycast(aimTransform.position, aimTransform.forward, out RaycastHit hit, RaycastLength))
            {
                if (debug)
                    Debug.DrawRay(aimTransform.position, aimTransform.forward * RaycastLength, HitColor);
                return hit.collider.GetComponent<IAbilityContext>();
            }
            if (debug)
                Debug.DrawRay(aimTransform.position, aimTransform.forward * RaycastLength, RayColor);
            return null;
        }
    }
}