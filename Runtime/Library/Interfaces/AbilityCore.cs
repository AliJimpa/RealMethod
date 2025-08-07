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
        void Apply(GameObject caster, IAbilityContext target);
    }






    public interface IAbility2
    {
        string Id { get; }
        float Cooldown { get; }
        bool IsOnCooldown { get; }

        void Activate(GameObject caster, GameObject target);
        void TickCooldown(float deltaTime);
    }
}