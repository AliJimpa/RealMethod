






/// <summary>
/// List of runtime-registered <see cref="IService"/> instances owned by the game.
/// </summary>
// private readonly Dictionary<Type, IService> Services = new();

/// <summary>
/// Returns true if the Game system has one or more registered services;
/// otherwise, returns false.
/// </summary>
// public static bool HasService => Instance.Services.Count > 0;







/// <summary>
/// Registers a service instance of type <typeparamref name="T"/> in the service container.
/// </summary>
/// <typeparam name="T">
/// The type of the service to register. Must implement <see cref="IService"/>.
/// </typeparam>
/// <param name="service">
/// The service instance to register.
/// </param>
/// <param name="author">
/// Optional object providing context or ownership information for the registration.
/// Passed to <see cref="IService.OnRegister(object)"/>.
/// </param>
/// <returns>
/// <c>true</c> if the service was successfully registered; 
/// <c>false</c> if a service of the same type is already registered.
/// </returns>
/// <remarks>
/// This method prevents duplicate registrations. If a service of the same type
/// is already registered, an error is logged and the method returns <c>false</c>.
/// Upon successful registration, <see cref="IService.OnRegister(object)"/> is invoked.
/// </remarks>
// public static bool Register<T>(T service, object author = null) where T : IService
// {
//     Type TypeService = typeof(T);

//     if (Instance.Services.ContainsKey(TypeService))
//     {
//         Debug.LogError($"IService {TypeService} already registered.");
//         return false;
//     }

//     service.OnRegister(author);
//     Instance.Services[TypeService] = service;
//     return true;

// }
/// <summary>
/// Checks whether a service of type <typeparamref name="T"/> is currently registered
/// in the Service Locator.
/// </summary>
/// <typeparam name="T">
/// The type of service to check. The type must implement <see cref="IService"/>.
/// </typeparam>
/// <returns>
/// <c>true</c> if a service of type <typeparamref name="T"/> is registered; otherwise <c>false</c>.
/// </returns>
// public static bool IsRegistered<T>() where T : IService
// {
//     return Instance.Services.ContainsKey(typeof(T));
// }
/// <summary>
/// Remove service instance 
/// </summary>
/// <typeparam name="T">The object type that implement 'IService'</typeparam>
// public static bool Unregister<T>(object author = null) where T : IService
// {
//     Type TypeService = typeof(T);

//     if (Instance.Services.ContainsKey(TypeService))
//     {
//         Instance.Services[TypeService].OnUnregister(author);
//         return Instance.Services.Remove(TypeService); ;
//     }
//     else
//     {
//         Debug.LogWarning($"IService {TypeService} Not found.");
//         return false;
//     }
// }
/// <summary>
/// Retrieves the service instance of type <typeparamref name="T"/> if available.
/// </summary>
/// <typeparam name="T">IService type to retrieve.</typeparam>
/// <returns>The service instance of type <typeparamref name="T"/>, or <c>null</c> if not found.</returns>
// public static T GetService<T>() where T : IService
// {
//     Type TypeService = typeof(T);

//     if (Instance.Services.TryGetValue(TypeService, out var provider))
//         return (T)provider.Self;

//     throw new Exception($"Service {TypeService} not registered.");
// }
/// <summary>
/// Attempts to find a service of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">IService type to find.</typeparam>
/// <param name="service">Out parameter that receives the service if found.</param>
/// <returns><c>true</c> if the service was found; otherwise <c>false</c>.</returns>
// public static bool TryGetService<T>(out T service) where T : IService
// {
//     if (Instance.Services.TryGetValue(typeof(T), out var provider))
//     {
//         service = (T)provider.Self;
//         return true;
//     }

//     service = default;
//     return false;
// }
/// <summary>
///  Clear all services in Game
/// </summary>
// public static void ClearService()
// {
//     foreach (var service in Instance.Services.Values)
//     {
//         service.OnUnregister(null);
//     }
//     Instance.Services.Clear();
// }



// private void Notify_OnWorldInitiate(World NewWorld)
//         {
//             foreach (var service in Services)
//             {
//                 service.Value.OnWorldChanging(World, NewWorld);
//             }
//             World = NewWorld;
//             OnWorldChanged(World);
//         }












/// <summary>
/// Base abstract class implementing <see cref="IService"/>.
/// Provides a framework for derived services to handle lifecycle events.
/// </summary>
//     public abstract class Service : IService
//     {
//         // Implement IService Interface
//         /// <summary>
//         /// Called when the service starts. Must be implemented by derived classes.
//         /// </summary>
//         /// <param name="Author">The object responsible for creating the service.</param>
//         public abstract void OnRegister(object author);
//         /// <summary>
//         /// Called when a new world or environment is initialized.
//         /// Must be implemented by derived classes.
//         /// </summary>
//         public abstract void OnWorldChanging(World Previous, World New);
//         /// <summary>
//         /// Called when the service ends or is deleted.
//         /// Must be implemented by derived classes.
//         /// </summary>
//         /// <param name="Author">The object responsible for deleting the service.</param>
//         public abstract void OnUnregister(object author);
// #if UNITY_EDITOR
//         string IService.GetInspectorInfo()
//         {
//             return GetDisplayInfo();
//         }
//         protected virtual string GetDisplayInfo()
//         {
//             return GetType().ToString() + ": ";
//         }
// #endif

//     }



/// <summary>
        /// Returns the object instance that implements this service.
        /// </summary>
        // object Self => this;



