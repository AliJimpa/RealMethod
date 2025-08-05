using UnityEngine;

namespace RealMethod
{
    public interface IResourceUnit : IIdentifier
    {
        float MaxValue { get; }
        float CurrentValue { get; }
        float MinValue { get; }
    }
    interface IModifiableResource : IResourceUnit
    {
        void SetValue(float value);
        void ModifyValue(float delta);
        void Refill();        // Sets to MaxValue
        void Deplete();       // Sets to MinValue
    }
    interface IConsumableResource : IModifiableResource
    {
        bool CanConsume(float amount);
        bool Consume(float amount);
    }
    interface IRegenerableResource : IModifiableResource
    {
        float RegenRate { get; }  // Units per second
        void Regenerate(float deltatime);
    }
    // interface IResourceContainer
    // {
    //     void AddResource(GameResource resource);
    //     void RemoveResource(string resourceName);
    //     IResource GetResource(string name);
    //     bool HasResource(string name);
    //     IReadOnlyDictionary<string, IResource> GetAllResources();
    // }

    public class GameResource : IConsumableResource, IRegenerableResource
    {
        private string resourceName;
        [SerializeField]
        private float maxValue;
        [SerializeField]
        private float currentValue;
        [SerializeField]
        private float minValue;
        [SerializeField]
        private bool isRegenerated = false;
        [SerializeField, ConditionalHide("isRegenerated", true, false)]
        private float regenRate = 5;

        public GameResource(string name)
        {
            resourceName = name;
        }
        public GameResource(string name, float min, float max)
        {
            resourceName = name;
            maxValue = max;
            currentValue = max;
            minValue = min;
        }
        public GameResource(string name, float min, float max, float genRate)
        {
            resourceName = name;
            maxValue = max;
            regenRate = genRate;
            isRegenerated = genRate > 0 ? true : false;
            currentValue = maxValue;
            minValue = min;
        }

        // Implement Interfaces
        public string NameID => resourceName;
        public float MaxValue => maxValue;
        public float CurrentValue => currentValue;
        public float MinValue => minValue;
        public float RegenRate => regenRate;

        // Public Functions
        public bool CanConsume(float amount) => currentValue >= amount;
        public bool Consume(float amount)
        {
            if (!CanConsume(amount))
                return false;

            ModifyValue(-amount);
            return true;
        }
        public void SetValue(float value)
        {
            currentValue = Mathf.Clamp(value, MinValue, MaxValue);
        }
        public void ModifyValue(float delta)
        {
            SetValue(CurrentValue + delta);
        }
        public void Regenerate(float deltaTime)
        {
            if (isRegenerated)
                ModifyValue(RegenRate * deltaTime);
        }
        public void Refill()
        {
            currentValue = MaxValue;
        }
        public void Deplete()
        {
            currentValue = MinValue;
        }

    }
    // public class ResourceContainer : MonoBehaviour, IResourceContainer
    // {
    //     private Dictionary<string, IResource> resources = new();


    //     // Unity Method
    //     private void Update()
    //     {
    //         foreach (var source in resources)
    //         {
    //             if (resources is IRegenerableResource provider)
    //                 provider.Regenerate(Time.deltaTime);
    //         }
    //     }

    //     // Implement IResourceContainer Interface
    //     public void AddResource(GameResource resource)
    //     {
    //         resources.Add(resource.Name, resource);
    //     }
    //     public void RemoveResource(string resourceName)
    //     {
    //         resources.Remove(resourceName);
    //     }
    //     public IResource GetResource(string name)
    //     {
    //         resources.TryGetValue(name, out var res);
    //         return res;
    //     }
    //     public IReadOnlyDictionary<string, IResource> GetAllResources() => resources;
    //     public bool HasResource(string name) => resources.ContainsKey(name);

    // }


}