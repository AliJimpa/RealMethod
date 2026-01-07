using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// Defines the core behavior required for all game manager classes.
    /// </summary>
    public interface IGameManager
    {
        /// <summary>
        /// Returns the MonoBehaviour instance that implements this manager.
        /// </summary>
        /// <returns>
        /// The MonoBehaviour associated with this game manager.
        /// </returns>
        MonoBehaviour GetManagerClass();

        /// <summary>
        /// Initializes the manager and prepares it for use in the game lifecycle.
        /// </summary>
        /// <param name="AlwaysLoaded">
        /// If true, the manager should persist and remain loaded at all times.
        /// </param>
        void InitiateManager(bool AlwaysLoaded);

        /// <summary>
        /// Enables or disables a service handled by this manager.
        /// </summary>
        /// <param name="service">
        /// The service to be resolved or managed.
        /// </param>
        /// <param name="active">
        /// Indicates whether the service should be activated (true) or deactivated (false).
        /// </param>
        void ResolveService(Service service, bool active);
    }

}