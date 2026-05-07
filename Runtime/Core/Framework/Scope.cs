using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// An abstract base class that extends <see cref="RealKernel"/> to provide a structured
    /// container for managing game systems and features within a specific context.
    /// 
    /// Scope instances are responsible for collecting, initializing, and providing access
    /// to various <see cref="IGameManager"/> implementations.
    /// It acts as a localized service locator for its immediate context and potentially
    /// delegates to parent scopes for unresolved dependencies.
    /// 
    /// This class defines the core manager lifecycle within a given boundary, such as
    /// global application lifetime or a scene-specific context.
    /// </summary>
    /// <remarks>
    /// Developers will typically implement or inherit from concrete Scope types like
    /// <see cref="Game"/>Scope (for global systems) or <see cref="World"/>Scope (for scene-bound systems).
    /// </remarks>
    public abstract class Scope : Kernel
    {
        public bool IsScopeOpened { get; private set; } = false;
        /// <summary>
        /// Cached array of managers that were instantiated from configured game prefabs or World gameobject.
        /// </summary>
        private IGameManager[] _gameManagers;
        /// <summary>
        /// Cached list of modules that were instantiated from scope functions (add / remove).
        /// </summary>
        private List<GameService> _gameServices;



        /// <summary>
        /// Attempts to find a manager whose <c>Component</c> matches the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The component type to search for.</typeparam>
        /// <param name="result">
        /// When this method returns, contains the found component of type <typeparamref name="T"/> if successful;
        /// otherwise <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if a matching manager component was found; otherwise <c>false</c>.
        /// </returns>
        public bool TryFindGameManager<T>(out T result) where T : Component, IGameManager
        {
            result = null;

            if (IsScopeLive())
                return false;

            if (_gameManagers == null)
            {
                return false;
            }

            foreach (var manager in _gameManagers)
            {
                if (manager == null)
                {
                    Debug.LogError($"A manager reference({typeof(T)}) has been removed or destroyed. Managers should never be null.");
                    return false;
                }
                if (manager.Component is T found)
                {
                    result = found;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Attempts to locate an existing game service of type <typeparamref name="T"/> within the current scope.
        /// If the scope is not active, or the service list is unavailable, the method returns false.
        /// Null service entries produce an error log, as game services are expected to remain valid,
        /// and the search is aborted.
        /// </summary>
        /// <typeparam name="T">
        /// The type of game service to search for. Must inherit from <see cref="GameService"/>.
        /// </typeparam>
        /// <param name="result">
        /// When the method returns, contains the found service instance if a match is discovered; otherwise null.
        /// </param>
        /// <returns>
        /// True if a service of type <typeparamref name="T"/> is found; otherwise false.
        /// </returns>
        public bool TryFindGameService<T>(out T result) where T : GameService
        {
            result = null;

            if (IsScopeLive())
                return false;

            if (_gameServices == null)
            {
                return false;
            }

            foreach (var service in _gameServices)
            {
                if (service == null)
                {
                    Debug.LogError($"A GameService reference({typeof(T)}) has been removed or destroyed. GameService should never be null.");
                    return false;
                }
                if (service is T targetclass)
                {
                    result = targetclass;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Creates, stores, and registers a new service of type <typeparamref name="T"/>.
        /// If a service of the same type already exists, the method logs a warning and returns null.
        /// The created service must implement <see cref="IService"/> to be registered and returned.
        /// All implemented interfaces derived from <see cref="IService"/> are registered
        /// in the service container, except the base <see cref="IService"/> interface itself.
        /// </summary>
        /// <typeparam name="T">
        /// The type of service to create and add. Must inherit from <see cref="GameService"/>
        /// and provide a parameterless constructor.
        /// </typeparam>
        /// <returns>
        /// The created service as <see cref="IService"/> if successful; otherwise null.
        /// </returns>
        public IService AddGameService<T>() where T : GameService, new()
        {
            if (IsExistsModule<T>())
            {
                Debug.LogWarning($"Service of type {typeof(T)} is already added and still alive.");
                return null;
            }

            // Create Service
            T newService = new T();
            _gameServices.Add(newService);

            if (newService is IService provider)
            {
                var interfaces = newService.GetType().GetInterfaces();
                foreach (var i in interfaces)
                {
                    // Skip the base Interface
                    if (i == typeof(IService))
                        continue;

                    // Only interfaces derived from Interface
                    if (typeof(IService).IsAssignableFrom(i))
                    {
                        Services.Register(newService, i, false);
                    }
                }
                return provider;
            }
            else
            {
                Debug.LogWarning($"Service of type {typeof(T)} should implement one of tye {typeof(IService)} interface");
                return null;
            }
        }
        /// <summary>
        /// Removes a service of type <typeparamref name="T"/> from the scope.
        /// If the service exists, it will be unregistered and disposed.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the service to remove.
        /// </typeparam>
        /// <returns>
        /// True if the service was successfully removed; otherwise false.
        /// </returns>
        public bool RemoveGameService<T>() where T : GameService
        {
            if (!IsExistsModule<T>())
            {
                Debug.LogWarning($"Service of type {typeof(T)} is not exist.");
                return false;
            }
            return RemoveGameService(typeof(T));
        }
        public void ClearGameServices()
        {
            for (int i = 0; i < _gameServices.Count; i++)
            {
                var interfaces = _gameServices[i].GetType().GetInterfaces();
                foreach (var interf in interfaces)
                {
                    // Only interfaces derived from Interface
                    if (typeof(IService).IsAssignableFrom(interf))
                    {
                        Services.Unregister(interf);
                    }
                }
                ((IDisposable)_gameServices[i]).Dispose();
                _gameServices.RemoveAt(i);
            }
            _gameServices.Clear();
        }




        /// <summary>
        /// Opens the scope and initializes managers and services for the provided GameObjects.
        /// It collects all <see cref="IGameManager"/> instances from the given objects and prepares
        /// the internal service container. If the scope is already opened, the call will be ignored.
        /// </summary>
        /// <param name="Objects">
        /// Array of GameObjects that may contain managers to be collected and registered in this scope.
        /// </param>
        protected void OpenScope(GameObject[] Objects)
        {
            if (IsScopeOpened)
            {
                Debug.LogWarning($"{this} This scope it was opend befor.");
                return;
            }
            IsScopeOpened = true;

            _gameServices = new List<GameService>(10);


            if (Objects == null)
            {
                Debug.LogError("OpeningScope failed: GameObject array 'Objects' is null.");
                return;
            }


            List<IGameManager> result = new List<IGameManager>();
            for (int i = 0; i < Objects.Length; i++)
            {
                var Newmanager = CollectManagers(Objects[i]);
                foreach (var manager in Newmanager)
                {
                    result.Add(manager);
                }
            }
            _gameManagers = result.ToArray();
        }
        /// <summary>
        /// Closes the current scope and releases all registered services and managers.
        /// All services implementing <see cref="IService"/> are unregistered and disposed,
        /// and all managers are properly dispensed. If the scope is already closed,
        /// the call will be ignored.
        /// </summary>
        protected void CloseScope()
        {
            if (!IsScopeOpened)
            {
                Debug.LogWarning($"{this} This scope it was Closed befor.");
                return;
            }
            IsScopeOpened = false;

            for (int i = 0; i < _gameServices.Count; i++)
            {
                var interfaces = _gameServices[i].GetType().GetInterfaces();
                foreach (var interf in interfaces)
                {
                    // Only interfaces derived from Interface
                    if (typeof(IService).IsAssignableFrom(interf))
                    {
                        Services.Unregister(interf);
                    }
                }
                ((IDisposable)_gameServices[i]).Dispose();
                _gameServices.RemoveAt(i);
            }
            _gameServices = null;

            foreach (var manager in _gameManagers)
            {
                DispenseManagers(manager);
            }
            _gameManagers = null;

        }




        private bool RemoveGameService(Type type)
        {
            if (type == null)
                return false;


            for (int i = 0; i < _gameServices.Count; i++)
            {
                if (_gameServices[i].GetType() == type)
                {
                    var interfaces = _gameServices[i].GetType().GetInterfaces();
                    foreach (var interf in interfaces)
                    {
                        // Only interfaces derived from Interface
                        if (typeof(IService).IsAssignableFrom(interf))
                        {
                            Services.Unregister(interf);
                        }
                    }
                    ((IDisposable)_gameServices[i]).Dispose();
                    _gameServices.RemoveAt(i);
                    return true;
                }
            }

            Debug.LogWarning($"Service of type {type.Name} not found to remove.");
            return false;
        }
        private IGameManager[] CollectManagers(GameObject Object)
        {
            if (Object == null)
            {
                Debug.LogWarning("CollectManagers: GameObject 'Object' is null.");
                return null;
            }

            var managers = Object.GetComponents<IGameManager>();

            foreach (var manager in managers)
            {
                Component comp = manager.Component;
                RegisterInterface<IGameManager>(comp);
                RegisterInterface<IService>(comp);
                manager.InitiateManager(this);
            }

            return managers;
        }
        private void DispenseManagers(IGameManager manager)
        {
            if (manager == null)
            {
                Debug.LogWarning("DispenseManagers: Manager 'manager' is null.");
                return;
            }
            Component comp = manager.Component;
            UnregisterInterface<IGameManager>(comp);
            UnregisterInterface<IService>(comp);
            manager.InitiateManager(this);
        }
        private void RegisterInterface<T>(Component comp)
        {
            if (comp is T service)
            {
                var interfaces = comp.GetType().GetInterfaces();

                foreach (var i in interfaces)
                {
                    // Skip the base Interface
                    if (i == typeof(T))
                        continue;

                    // Only interfaces derived from Interface
                    if (typeof(T).IsAssignableFrom(i))
                    {
                        Services.Register(comp, i, false);
                    }
                }
            }
        }
        private void UnregisterInterface<T>(Component comp)
        {
            if (comp is T service)
            {
                var interfaces = comp.GetType().GetInterfaces();

                foreach (var i in interfaces)
                {
                    // Skip the base Interface
                    if (i == typeof(T))
                        continue;

                    // Only interfaces derived from Interface
                    if (typeof(T).IsAssignableFrom(i))
                    {
                        Services.Unregister(i);
                    }
                }
            }
        }
        private bool IsScopeLive()
        {
            if (!IsScopeOpened)
            {
                Debug.LogWarning($"First you shoul open Scope.");
                return false;
            }
            return true;
        }
        private bool IsExistsModule<T>() where T : GameService
        {
            foreach (var module in _gameServices)
            {
                if (module is T)
                    return true;
            }
            return false;
        }

    }



}