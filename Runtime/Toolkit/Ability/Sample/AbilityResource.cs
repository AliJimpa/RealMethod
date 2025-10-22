using UnityEngine;
using UnityEngine.InputSystem;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "AbilityResource", menuName = "RealMethod/Ability/AbilityResource", order = 0)]
    public class AbilityResource : AbilityAction
    {
        [Header("Setting")]
        [SerializeField]
        private float cooldown = 2;
        [SerializeField]
        private bool useResource;
        [SerializeField, ConditionalHide("useResource", true, false)]
        private string resourceName;
        [SerializeField, ConditionalHide("useResource", true, false)]
        private float amount;
        [SerializeField]
        private bool useByInput = false;
        [Header("Debug")]
        [SerializeField]
        private bool showDebug = false;

        // AbilityEffectAsset Methods
        protected override bool Prerequisite(GameObject user)
        {
            if (useResource)
            {
                IResourceContainer container = user.GetComponent<IResourceContainer>();
                if (container != null)
                {
                    IConsumableResource resource = container.GetConsumableResource(resourceName);
                    if (resource != null)
                    {
                        if (resource.CanConsume(amount))
                        {
                            resource.Consume(amount);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (showDebug)
                        {
                            Debug.Log($"can't find [{resourceName}] resource in {user}");
                        }
                        return false;
                    }
                }
                else
                {
                    Debug.LogWarning($"Prerequisite false,  user not Implemented {typeof(IResourceContainer)}");
                    return false;
                }
            }
            return true;
        }
        protected override float GetCooldown()
        {
            return cooldown;
        }

        // AbilityAction Methods
        protected override void OnInitiate()
        {
            if (showDebug)
            {
                Debug.Log($"[{name}] Initiated");
            }
        }
        public override void OnEnableInput()
        {
        }
        public override void OnDisableInput()
        {
        }
        protected override bool CheckInput(InputAction.CallbackContext context)
        {
            if (showDebug)
            {
                Debug.Log($"[{name}] Input Check=> {useByInput}");
            }
            return useByInput;
        }

    }
}

public class DebugEffect : RealMethod.AbilityEffect
{
    protected override void OnInitiate(RealMethod.AbilityAsset owner)
    {
        Debug.Log($"DebugEffect Initiated from {owner.name}");
    }
    protected override void Apply(GameObject caster, RealMethod.IAbilityContext target) => Debug.Log($"DebugEffect Apply from {caster.name} to {target.targetPoint.gameObject.name}");
}
public class AddForceEffect : RealMethod.AbilityEffect
{
    // Called when the effect is initialized or bound to an ability
    protected override void OnInitiate(RealMethod.AbilityAsset owner)
    {
    }
    // Called when the effect is applied
    protected override void Apply(GameObject caster, RealMethod.IAbilityContext target)
    {
        // Get target's GameObject
        GameObject targetObject = target.targetPoint.gameObject;
        Vector3 force = -target.targetPoint.position.magnitude * Vector3.one;
        if (targetObject != null)
        {
            Rigidbody rb = targetObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = targetObject.AddComponent<Rigidbody>();
            }
            rb.AddForce(force, ForceMode.Force);
        }
    }
}
public class AddImpulseEffect : RealMethod.AbilityEffect
{
    // Called when the effect is initialized or bound to an ability
    protected override void OnInitiate(RealMethod.AbilityAsset owner)
    {
    }
    // Called when the effect is applied
    protected override void Apply(GameObject caster, RealMethod.IAbilityContext target)
    {
        // Get target's GameObject
        GameObject targetObject = target.targetPoint.gameObject;
        Vector3 force = -target.targetPoint.position.magnitude * Vector3.one;
        if (targetObject != null)
        {
            Rigidbody rb = targetObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = targetObject.AddComponent<Rigidbody>();
            }
            rb.AddForce(force, ForceMode.Impulse);
        }
    }
}
