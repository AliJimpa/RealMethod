namespace RealMethod
{
    /// <summary>
    /// Defines the contract for a object that can respond to lifecycle events.
    /// </summary>
    public interface IRegistrable
    {
        /// <summary>
        /// Called when the object registerd by servicelocator.
        /// </summary>
        void OnRegister();
        /// <summary>
        /// Called when unregisterd the object is deleted or destroyed from servicelocator.
        /// </summary>
        void OnUnregister();
    }
}