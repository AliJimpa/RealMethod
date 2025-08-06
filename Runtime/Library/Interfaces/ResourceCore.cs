using UnityEngine;

namespace RealMethod
{
    public interface IResourceItem : IIdentifier
    {
        Object GetResourceObject();
    }
    public interface IResource
    {
        float MaxValue { get; }
        float CurrentValue { get; }
        void Refill(); // Sets to MaxValue
        void Deplete(); // Sets to MinValue/Empty
    }



    // Resource Type
    public interface IConsumable : IResource
    {
        bool CanConsume(float amount);
        void Consume(float amount);
    }
    public interface IRegenerable : IResource
    {
        float RegenerationRate { get; } // Units per second
        void Regenerate(float deltaTime);
    }
    public interface IChargeable : IResource
    {
        void Charge(float amount);
        void ResetCharge();
        bool IsFullyCharged { get; }
    }
    public interface ITimeGated
    {
        float CooldownDuration { get; }
        float CooldownRemaining { get; }

        bool IsAvailable { get; }
        void StartCooldown();
        void TickCooldown(float deltaTime);
    }
    public interface IRefundable
    {
        float Refund(float percent); // return refunded amount
    }
    public interface IDecaying : IResource
    {
        float DecayRate { get; }
        void Decay(float deltaTime);
    }





    interface IModifiableResource
    {
        void Add(float amount);
        void Subtract(float amount);
        void Set(float amount);
    }


    // interface IResourceContainer
    // {
    //     void AddResource(GameResource resource);
    //     void RemoveResource(string resourceName);
    //     IResource GetResource(string name);
    //     bool HasResource(string name);
    //     IReadOnlyDictionary<string, IResource> GetAllResources();
    // }
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