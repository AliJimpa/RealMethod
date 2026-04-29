using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// Defines the core behavior required for all game manager classes.
    /// </summary>
    public interface IGameManager
    {
        /// <summary>
        /// Gets the Unity <see cref="Component"/> instance that implements this interface.
        /// </summary>
        /// <remarks>
        /// This allows systems interacting with the interface to access the underlying
        /// Unity component and its associated GameObject and Transform.
        /// </remarks>
        Component Component => (Component)this;

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