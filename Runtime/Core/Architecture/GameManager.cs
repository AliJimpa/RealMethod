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
        /// <param name="owner">
        /// Whitch owner class Initiation the manager
        /// </param>
        void InitiateManager(Scope owner);
    }

    public class GameManager : Method, IGameManager
    {
        // Implement IGameManager Interface
        public void InitiateManager(Scope owner)
        {
            OnInitiateManager(owner);
        }

        protected virtual void OnInitiateManager(Scope owner)
        {

        }
    }

}