namespace RealMethod
{
    /// <summary>
    /// Defines the contract for a service that can respond to lifecycle events.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Returns the object instance that implements this service.
        /// </summary>
        object Self => this;
        /// <summary>
        /// Called when the service is created.
        /// </summary>
        /// <param name="author">The object responsible for creating the service.</param>
        void Created(object author);
        /// <summary>
        /// Called when the world or environment updates.
        /// </summary>
        void ChangingWorld(World NewWorld);
        /// <summary>
        /// Called when the service is deleted or destroyed.
        /// </summary>
        /// <param name="author">The object responsible for deleting the service.</param>
        void Deleted(object author);
#if UNITY_EDITOR
        /// <summary>
        /// Returns a formatted string containing information about this service
        /// intended for display in the Unity Inspector or debugging interfaces.
        /// This method is for visualization purposes only and must not affect logic.
        /// </summary>
        /// <returns>Formatted display information.</returns>
        string GetInspectorInfo();
#endif
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
        void IService.ChangingWorld(World NewWorld)
        {
            OnWorldChanging(Game.World, NewWorld);
        }
        void IService.Deleted(object author)
        {
            OnEnd(author);
        }
# if UNITY_EDITOR
        string IService.GetInspectorInfo()
        {
            return GetDisplayInfo();
        }
        protected virtual string GetDisplayInfo()
        {
            return GetType().ToString() + ": ";
        }
#endif



        /// <summary>
        /// Called when the service starts. Must be implemented by derived classes.
        /// </summary>
        /// <param name="Author">The object responsible for creating the service.</param>
        protected abstract void OnStart(object Author);
        /// <summary>
        /// Called when a new world or environment is initialized.
        /// Must be implemented by derived classes.
        /// </summary>
        protected abstract void OnWorldChanging(World Previous, World New);
        /// <summary>
        /// Called when the service ends or is deleted.
        /// Must be implemented by derived classes.
        /// </summary>
        /// <param name="Author">The object responsible for deleting the service.</param>
        protected abstract void OnEnd(object Author);


    }

}