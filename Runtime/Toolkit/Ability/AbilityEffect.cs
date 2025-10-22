using UnityEngine;

namespace RealMethod
{
    [System.Serializable]
    public abstract class AbilityEffect : IAbilityEffect
    {
        // Implement IAbilityEffect interface
        void IAbilityEffect.Initiate(Object owner)
        {
            if (owner is AbilityAsset asset)
            {
                OnInitiate(asset);
            }
            else
            {
                Debug.LogError($"AbilityEffect should initiate by AbilityAsset not {owner}");
            }
        }
        void IAbilityEffect.Apply(GameObject caster, IAbilityContext target)
        {
            Apply(caster, target);
        }

        // Abstract Methods
        protected abstract void OnInitiate(AbilityAsset owner);
        protected abstract void Apply(GameObject caster, IAbilityContext target);

    }
}