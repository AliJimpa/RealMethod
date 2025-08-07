namespace RealMethod
{
    public interface IResourceItem : IItem
    {
        IResource GetResourceProvider();
    }



    public interface IResource
    {
        float MaxValue { get; }
        float Value { get; }
        void Refill(); // Sets to MaxValue
        void Deplete(); // Sets to MinValue/Empty
    }


    // Resource Type
    interface IModifiableResource : IResource
    {
        void Set(float val);
        void Modify(float amount);
    }
    public interface IConsumableResource : IResource
    {
        bool CanConsume(float amount);
        void Consume(float amount);
    }
    public interface IRegenerableResource : IResource
    {
        float RegenerationRate { get; } // Units per second
        void Regenerate(float deltaTime);
    }
    public interface IChargeableResource : IResource
    {
        void Charge(float amount);
        void ResetCharge();
        bool IsFullyCharged { get; }
    } 
    public interface IDecayingResource : IResource
    {
        float DecayRate { get; }
        void Decay(float deltaTime);
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