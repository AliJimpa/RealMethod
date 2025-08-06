using UnityEngine;

namespace RealMethod
{
    public class ResourceData : IResource
    {
        // Implement IResourceUnit Interface
        public float MaxValue => throw new System.NotImplementedException();
        public float CurrentValue => throw new System.NotImplementedException();
        public string NameID => throw new System.NotImplementedException();
        public void Deplete()
        {
            throw new System.NotImplementedException();
        }
        public void Refill()
        {
            throw new System.NotImplementedException();
        }
    }
    // public class GameResource : IConsumableResource, IRegenerableResource
    // {
    //     private string resourceName;
    //     [SerializeField]
    //     private float maxValue;
    //     [SerializeField]
    //     private float currentValue;
    //     [SerializeField]
    //     private float minValue;
    //     [SerializeField]
    //     private bool isRegenerated = false;
    //     [SerializeField, ConditionalHide("isRegenerated", true, false)]
    //     private float regenRate = 5;

    //     public GameResource(string name)
    //     {
    //         resourceName = name;
    //     }
    //     public GameResource(string name, float min, float max)
    //     {
    //         resourceName = name;
    //         maxValue = max;
    //         currentValue = max;
    //         minValue = min;
    //     }
    //     public GameResource(string name, float min, float max, float genRate)
    //     {
    //         resourceName = name;
    //         maxValue = max;
    //         regenRate = genRate;
    //         isRegenerated = genRate > 0 ? true : false;
    //         currentValue = maxValue;
    //         minValue = min;
    //     }

    //     // Implement Interfaces
    //     public string NameID => resourceName;
    //     public float MaxValue => maxValue;
    //     public float CurrentValue => currentValue;
    //     public float MinValue => minValue;
    //     public float RegenRate => regenRate;

    //     // Public Functions
    //     public bool CanConsume(float amount) => currentValue >= amount;
    //     public bool Consume(float amount)
    //     {
    //         if (!CanConsume(amount))
    //             return false;

    //         ModifyValue(-amount);
    //         return true;
    //     }
    //     public void SetValue(float value)
    //     {
    //         currentValue = Mathf.Clamp(value, MinValue, MaxValue);
    //     }
    //     public void ModifyValue(float delta)
    //     {
    //         SetValue(CurrentValue + delta);
    //     }
    //     public void Regenerate(float deltaTime)
    //     {
    //         if (isRegenerated)
    //             ModifyValue(RegenRate * deltaTime);
    //     }
    //     public void Refill()
    //     {
    //         currentValue = MaxValue;
    //     }
    //     public void Deplete()
    //     {
    //         currentValue = MinValue;
    //     }

    // }
}