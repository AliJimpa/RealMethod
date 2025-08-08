using UnityEngine;

namespace RealMethod
{
    [System.Serializable]
    public abstract class AbilityEffect : IAbilityEffect
    {
        // Implement IAbilityEffect interface
        public void Initiate(Object owner)
        {
            OnInitiate(owner);
        }
        void IAbilityEffect.Apply(GameObject caster, IAbilityContext target)
        {
            Apply(caster, target);
        }

        // Abstract Methods
        protected abstract void OnInitiate(Object owner);
        protected abstract void Apply(GameObject caster, IAbilityContext target);
    }
}