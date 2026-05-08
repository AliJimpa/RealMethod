using System;

namespace RealMethod
{

    /// <summary>
    /// Generic singleton base class.
    /// Ensures only one instance of type <typeparamref name="T"/> exists.
    /// </summary>
    public abstract class Context<T> : IDisposable where T : Context<T>, new()
    {
        private static T _instance;
        private static readonly object _lock = new object();



        /// <summary>
        /// Gets the single instance of this type.
        /// </summary>
        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        // Use Reflection to create the instance since 
                        // the constructor is private/protected
                        _instance = (T)Activator.CreateInstance(typeof(T), true);
                    }
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Prevents external instantiation.
        /// </summary>
        protected Context()
        {
            OnBegin();
        }

        /// <summary>
        /// Cleans up the singleton instance.
        /// </summary>
        public virtual void Dispose()
        {
            lock (_lock)
            {
                if (_instance != null)
                {
                    OnEnd();
                    _instance = null; // Removing the reference
                }
            }
        }




        /// <summary>
        /// Override this to perform custom Starter logic (like Subscribing events).
        /// </summary>
        protected abstract void OnBegin();
        /// <summary>
        /// Override this to perform custom cleanup logic (like Unsubscribing events).
        /// </summary>
        protected abstract void OnEnd();
    }
}
