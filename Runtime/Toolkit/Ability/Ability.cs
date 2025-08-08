using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "Ability", menuName = "RealMethod/Ability/Asset", order = 0)]
    public class Ability : AbilityEffectAsset
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

    }



}