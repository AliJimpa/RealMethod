
namespace RealMethod
{
    /// <summary>
    /// Defines the contract for a service that can respond to lifecycle events.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Called when the service is created.
        /// </summary>
        /// <param name="author">The object responsible for creating the service.</param>
        void Created(object author);
        /// <summary>
        /// Called when the world or environment updates.
        /// </summary>
        void WorldUpdated();
        /// <summary>
        /// Called when the service is deleted or destroyed.
        /// </summary>
        /// <param name="author">The object responsible for deleting the service.</param>
        void Deleted(object author);
    }

    /// <summary>
    /// Base abstract class implementing <see cref="IService"/>.
    /// Provides a framework for derived services to handle lifecycle events.
    /// </summary>
    public abstract class Service : IService
    {
        // Implement IService Interface
        void IService.Created(object author)
        {
            OnStart(author);
        }
        void IService.WorldUpdated()
        {
            OnNewWorld();
        }
        void IService.Deleted(object author)
        {
            OnEnd(author);
        }


        /// <summary>
        /// Called when the service starts. Must be implemented by derived classes.
        /// </summary>
        /// <param name="Author">The object responsible for creating the service.</param>
        protected abstract void OnStart(object Author);
        /// <summary>
        /// Called when a new world or environment is initialized.
        /// Must be implemented by derived classes.
        /// </summary>
        protected abstract void OnNewWorld();
        /// <summary>
        /// Called when the service ends or is deleted.
        /// Must be implemented by derived classes.
        /// </summary>
        /// <param name="Author">The object responsible for deleting the service.</param>
        protected abstract void OnEnd(object Author);
    }

}