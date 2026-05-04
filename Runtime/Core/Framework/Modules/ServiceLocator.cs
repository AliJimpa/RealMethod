using System;
using System.Collections.Generic;

namespace RealMethod
{
    /// <summary>
    /// Global service locator with automatic garbage collection safety.
    /// Stores services by interface type using WeakReferences.
    /// Thread‑safe for Register / Get / Remove operations.
    /// </summary>
    public sealed class ServiceLocator : GameModule
    {
        private readonly object _lock = new();
        private Dictionary<Type, WeakReference<object>> _services => Repository;

        /// <summary>
        /// Register a service instance under its concrete or interface type T.
        /// T must implement IService so we know this is a service.
        /// Stored internally as WeakReference&lt;object&gt;.
        /// </summary>
        public void Register<T>(T service, bool overwrite = false)
            where T : class
        {
            Register(service, typeof(T), overwrite);
        }
        /// <summary>
        /// Register a service instance under its concrete or interface type T.
        /// T must implement IService so we know this is a service.
        /// Stored internally as WeakReference&lt;object&gt;.
        /// </summary>
        public void Register(object service, Type type = null, bool overwrite = false)
        {
            Type serviceType = null;
            if (type == null)
            {
                serviceType = service.GetType();
            }
            else
            {
                serviceType = type;
            }


            if (service == null)
                throw new ArgumentNullException(nameof(service));

            if (!serviceType.IsAssignableFrom(service.GetType()))
                throw new ArgumentException($"{service.GetType().Name} is not assignable to {serviceType.Name}");

            lock (_lock)
            {
                if (_services.TryGetValue(serviceType, out var weak))
                {
                    if (!weak.TryGetTarget(out _))
                    {
                        _services[serviceType] = new WeakReference<object>(service);
                        if (service is IRegistrable provider1)
                            provider1.OnRegister();
                        return;
                    }

                    if (overwrite)
                    {
                        _services[serviceType] = new WeakReference<object>(service);
                        if (service is IRegistrable provider2)
                            provider2.OnRegister();
                        return;
                    }

                    throw new InvalidOperationException(
                        $"Service of type {serviceType.Name} is already registered and still alive.");
                }

                _services[serviceType] = new WeakReference<object>(service);
                if (service is IRegistrable provider3)
                    provider3.OnRegister();
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
                    if (service is IRegistrable provider)
                        provider.OnRegister();
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
        public bool Unregister<T>() where T : class
        {
            var type = typeof(T);
            lock (_lock)
            {
                return Unregister(type);
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
        public bool Unregister(Type type)
        {
            lock (_lock)
            {
                if (_services.ContainsKey(type))
                {
                    if (_services[type].TryGetTarget(out object target))
                    {
                        if (target is IRegistrable provider)
                            provider.OnRegister();
                    }
                    return _services.Remove(type);
                }
                return false;
            }
        }
        /// <summary>
        /// Force-clears all services from the locator.
        /// </summary>
        public void ClearAll()
        {
            lock (_lock)
            {
                foreach (var item in _services)
                {
                    Unregister(item.Key);
                }
                _services.Clear();
            }
        }


#if UNITY_EDITOR
        protected override string GetInspectorInfor()
        {
            return $"Repository ({_services.Count})";
        }
#endif

    }
}
