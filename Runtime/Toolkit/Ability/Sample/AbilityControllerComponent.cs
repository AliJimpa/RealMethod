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
        [SerializeField]
        private LayerMask layer = 1;
        [Header("DrawGizmos")]
        [SerializeField]
        private bool debug = false;
        [SerializeField, ConditionalHide("debug", true, false)]
        private Color RayColor = Color.blue;
        [SerializeField, ConditionalHide("debug", true, false)]
        private Color HitColor = Color.red;

        public override IAbilityContext GetTarget(AbilityAsset ability)
        {
            // Simplified raycast targeting
            if (Physics.Raycast(aimTransform.position, aimTransform.forward, out RaycastHit hit, RaycastLength, layer))
            {
                if (debug)
                    Debug.DrawRay(aimTransform.position, aimTransform.forward * RaycastLength, HitColor, 1);
                return hit.collider.GetComponent<IAbilityContext>();
            }
            if (debug)
                Debug.DrawRay(aimTransform.position, aimTransform.forward * RaycastLength, RayColor, 1);
            return null;
        }
    }
}