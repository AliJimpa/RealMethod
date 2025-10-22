using UnityEngine;

namespace RealMethod
{
    public interface IAbilityContext
    {
        Transform targetPoint { get; }
    }

    public interface IAbility : IIdentifier
    {
        bool CanUse(GameObject user);
        bool TryUse(GameObject user, IAbilityContext context);
    }

    public interface IAbilityEffect
    {
        void Initiate(Object owner);
        void Apply(GameObject caster, IAbilityContext target);
    }
}