using UnityEngine;

namespace RealMethod
{
    public abstract class AbilityEffect : IAbilityEffect
    {
        // Implement IAbilityEffect interface
        void IAbilityEffect.Apply(GameObject caster, IAbilityContext target)
        {
            Apply(caster, target);
        }

        // Abstract Methods
        protected abstract void Apply(GameObject caster, IAbilityContext target);
    }
}