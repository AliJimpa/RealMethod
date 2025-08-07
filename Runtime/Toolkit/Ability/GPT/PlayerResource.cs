using RealMethod;
using UnityEngine;

public class PlayerResource : MonoBehaviour//, IConsumableResource
{
    public float mana = 100f;
    public float stamina = 100f;

    public string Name => throw new System.NotImplementedException();

    public float MaxValue { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public float CurrentValue { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public float MinValue => throw new System.NotImplementedException();

    public bool HasResource(AbilityResourceType type, float amount)
    {
        return type switch
        {
            AbilityResourceType.Mana => mana >= amount,
            AbilityResourceType.Stamina => stamina >= amount,
            _ => true
        };
    }

    public void ConsumeResource(AbilityResourceType type, float amount)
    {
        switch (type)
        {
            case AbilityResourceType.Mana: mana -= amount; break;
            case AbilityResourceType.Stamina: stamina -= amount; break;
        }
    }

    public bool CanConsume(float amount)
    {
        throw new System.NotImplementedException();
    }

    public bool Consume(float amount)
    {
        throw new System.NotImplementedException();
    }

    public void SetValue(float value)
    {
        throw new System.NotImplementedException();
    }

    public void ModifyValue(float delta)
    {
        throw new System.NotImplementedException();
    }

    public void Refill()
    {
        throw new System.NotImplementedException();
    }

    public void Deplete()
    {
        throw new System.NotImplementedException();
    }
}
