using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// Defines a contract for services in the game architecture.
    /// Extends the IIdentifier interface.
    /// </summary>
    public interface IService : IIdentifier
    {

    }

    /// <summary>
    /// Represents a service in the game architecture, extending the functionality of GameModule.
    /// Ensures that all derived services implement the IService interface.
    /// </summary>
    public abstract class GameService : GameModule
    {
        /// <summary>
        /// Initializes a new instance of the GameService class.
        /// Validates that the derived class implements the IService interface.
        /// Logs an error if the IService interface is not implemented.
        /// </summary>
        public GameService() : base()
        {
            if (this is not IService)
                Debug.LogError($"Service of type {GetType().Name} should implement one of tye {typeof(IService).Name} interface");
        }
    }
}