using System;
using System.Collections.Generic;

namespace RealMethod
{

    /// <summary>
    /// Global service locator with automatic garbage collection safety.
    /// Stores services by interface type using WeakReferences.
    /// Thread‑safe for Register / Get / Remove operations.
    /// </summary>
    public class ServiceLocator
    {
        private readonly Dictionary<Type, WeakReference<object>> _services = new();
        private readonly object _lock = new();

        /// <summary>
        /// Register a service instance under its concrete or interface type T.
        /// T must implement IService so we know this is a service.
        /// Stored internally as WeakReference&lt;object&gt;.
        /// </summary>
        public void Register<T>(T service, bool overwrite = false)
            where T : class
        {
            Register(typeof(T), service, overwrite);
        }

        /// <summary>
        /// Register a service instance under its concrete or interface type T.
        /// T must implement IService so we know this is a service.
        /// Stored internally as WeakReference&lt;object&gt;.
        /// </summary>
        public void Register(Type type, object service, bool overwrite = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (service == null)
                throw new ArgumentNullException(nameof(service));

            if (!type.IsAssignableFrom(service.GetType()))
                throw new ArgumentException($"{service.GetType().Name} is not assignable to {type.Name}");

            lock (_lock)
            {
                if (_services.TryGetValue(type, out var weak))
                {
                    if (!weak.TryGetTarget(out _))
                    {
                        _services[type] = new WeakReference<object>(service);
                        return;
                    }

                    if (overwrite)
                    {
                        _services[type] = new WeakReference<object>(service);
                        return;
                    }

                    throw new InvalidOperationException(
                        $"Service of type {type.Name} is already registered and still alive.");
                }

                _services[type] = new WeakReference<object>(service);
            }
        }

        /// <summary>
        /// Register only if no live instance is currently registered for T.
        /// </summary>
        public void RegisterIfAbsent<T>(T service)
            where T : class
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            var type = typeof(T);

            lock (_lock)
            {
                if (!_services.TryGetValue(type, out var weak) ||
                    !weak.TryGetTarget(out _))
                {
                    _services[type] = new WeakReference<object>(service);
                }
            }
        }

        /// <summary>
        /// Get the registered instance for type T.
        /// Returns null if not found or GC-collected.
        /// You can call this as Get&lt;PlayerObject&gt;().
        /// </summary>
        public T Get<T>() where T : class
        {
            var type = typeof(T);

            lock (_lock)
            {
                if (_services.TryGetValue(type, out var weak))
                {
                    if (weak.TryGetTarget(out var obj) && obj != null)
                    {
                        return obj as T;
                    }

                    // Weak reference dead → remove entry
                    _services.Remove(type);
                }
                return null;
            }
        }

        /// <summary>
        /// Try to get the registered instance for type T.
        /// </summary>
        public bool TryGet<T>(out T result) where T : class
        {
            var type = typeof(T);

            lock (_lock)
            {
                if (_services.TryGetValue(type, out var weak))
                {
                    if (weak.TryGetTarget(out var obj) && obj != null)
                    {
                        result = obj as T;
                        return result != null;
                    }

                    // Weak reference dead → remove entry
                    _services.Remove(type);
                }
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Check whether a live instance exists for type T.
        /// </summary>
        public bool Exists<T>() where T : class
        {
            var type = typeof(T);

            lock (_lock)
            {
                return _services.TryGetValue(type, out var weak) &&
                       weak.TryGetTarget(out _);
            }
        }

        /// <summary>
        /// Manually unregisters (removes) a service type.
        /// </summary>
        public void Unregister<T>() where T : class
        {
            var type = typeof(T);
            lock (_lock)
            {
                _services.Remove(type);
            }
        }
        /// <summary>
        /// Removes the service associated with type <typeparamref name="T"/> 
        /// from the Service Locator. 
        /// </summary>
        /// <remarks>
        /// If no service of this type is registered, the method does nothing.
        /// Use this when services are registered using generic calls such as
        /// Register&lt;T&gt;(instance).
        /// </remarks>
        public void Unregister(Type type)
        {
            if (_services.ContainsKey(type))
                _services.Remove(type);
        }

        /// <summary>
        /// Force-clears all services from the locator.
        /// </summary>
        public void ClearAll()
        {
            lock (_lock)
            {
                _services.Clear();
            }
        }
    }
}
