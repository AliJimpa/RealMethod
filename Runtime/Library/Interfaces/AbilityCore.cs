using UnityEngine;

namespace RealMethod
{
    public interface IAbility
    {
        void CanUse();
        bool TryUse();
    }

    public interface IAbilityEffect
    {
        void Apply(GameObject caster, GameObject target);
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