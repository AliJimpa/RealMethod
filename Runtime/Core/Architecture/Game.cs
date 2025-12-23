using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RealMethod
{
    /// <summary>
    /// Core game singleton that manages the active <see cref="World"/>, registered <see cref="Service"/>s,
    /// configuration and high-level game lifecycle (initialization, start, and shutdown).
    /// Derive from this class to implement project-specific behavior for the game's lifecycle hooks.
    /// </summary>
    public abstract class Game : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance of the active game.
        /// </summary>
        public static Game Instance;
        /// <summary>
        /// The currently active <see cref="World"/>. Updated when a world is initiated.
        /// </summary>
        public static World World { get; private set; }
        /// <summary>
        /// The core <see cref="GameService"/> implementation used for scene/world loading and service events.
        /// </summary>
        public static GameService Service { get; private set; }
        /// <summary>
        /// Active game configuration instance.
        /// </summary>
        public static GameConfig Config { get; private set; }
        /// <summary>
        /// Convenience property returning the player GameObject from the current <see cref="World"/>.
        /// Returns <c>null</c> and logs a warning if no world is set.
        /// </summary>
        public static GameObject Player
        {
            get
            {
                if (World == null)
                {
                    Debug.LogWarning("World is not set. Returning null for Player.");
                    return null;
                }
                return World.GetPlayerObject();
            }
        }
        /// <summary>
        /// Indicates whether the game is currently paused.
        /// Returns true when Time.timeScale is set to 0; otherwise, false.
        /// </summary>
        public static bool IsPaused => Time.timeScale == 0;


        /// <summary>
        /// Cached array of managers that were instantiated from configured game prefabs.
        /// </summary>
        private IGameManager[] Managers;
        /// <summary>
        /// List of runtime-registered <see cref="Service"/> instances owned by the game.
        /// </summary>
        private List<Service> GameServices;




        /// <summary>
        /// Initializes the game singleton and core systems on subsystem registration.
        /// This sets up the <see cref="Instance"/>, game <see cref="Service"/>,
        /// configuration, prefabs and managers and registers quit callbacks.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeGame()
        {
            // Load Project Setting
            ProjectSettingAsset ProjectSettings = Resources.Load<ProjectSettingAsset>("RealMethod/RealMethodSetting");
            if (ProjectSettings == null)
            {
                Debug.LogError("ProjectSettingAsset is missing from Resources folder!");
                Quit();
                return;
            }

            // Initiate Game Class
            var emptyObject = new GameObject("RealGame");
            Type TargetClass = ProjectSettings.GetGameInstanceClass();
            if (TargetClass == null)
            {
                Debug.LogWarning("GameInstanceClass that was empty. DefaultGame Created");
                Instance = emptyObject.AddComponent<DefultGame>();
            }
            if (typeof(Game).IsAssignableFrom(TargetClass))
            {
                Instance = (Game)emptyObject.AddComponent(TargetClass);
            }
            else
            {
                Debug.LogWarning($"Component of type {TargetClass} is not assignable from Game. DefaultGame Created");
                Instance = emptyObject.AddComponent<DefultGame>();
            }

            // Create Game Service
            Type targetService = ProjectSettings.GetGameServiceClass();
            if (targetService == null)
            {
                Debug.LogWarning($"GetGameServiceClass that was empty. DefaultGameService Created");
                Service = new DefaultGameService();
            }
            if (typeof(Service).IsAssignableFrom(targetService))
            {
                try
                {
                    Service = (GameService)Activator.CreateInstance(targetService);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to instantiate {targetService}: {ex.Message}. DefaultGameService Created");
                    Service = new DefaultGameService();
                }
            }
            else
            {
                Debug.LogWarning($"Type {targetService} is not assignable to Service. DefaultGameService Created");
                Service = new DefaultGameService();
            }
            Instance.GameServices = new List<Service>(3);
            ((IMethodSync)Service).BindMainWorldAdd(Instance.Notify_OnWorldInitiate);
            ((IService)Service).Created(Instance);

            // Set Game Config 
            if (ProjectSettings.GetGameConfig() != null)
            {
                Config = ProjectSettings.GetGameConfig();
            }
            else
            {
                Config = ScriptableObject.CreateInstance<DefaultGameConfig>();
            }
            Config.Initialized(Instance);

            // Initiate GamePrefab & Managers
            List<IGameManager> CashManagers = new List<IGameManager>(5);
            foreach (var obj in ProjectSettings.GetGamePrefabs())
            {
                if (obj != null)
                {
                    GameObject newobj = Instantiate(obj);
                    foreach (var manager in newobj.GetComponents<IGameManager>())
                    {
                        manager.InitiateManager(true);
                        CashManagers.Add(manager);
                    }
                    DontDestroyOnLoad(newobj);
                }
            }
            Instance.Managers = CashManagers.ToArray();

            // Unload Project Setting
            Resources.UnloadAsset(ProjectSettings);
            ProjectSettings = null;

            // Move Self GameObject to DontDestroy
            DontDestroyOnLoad(Instance.gameObject);
            Application.quitting += Instance.Notify_OnGameQuit;
        }
        /// <summary>
        /// Called before any scene is loaded. Invokes <see cref="GameInitialized"/> on the active instance.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeBeforeSceneLoad()
        {
            if (Instance != null)
            {
                Instance.GameInitialized();
            }
        }
        /// <summary>
        /// Called after a scene has finished loading. Invokes <see cref="GameStarted"/> on the active instance.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RuntimeAfterSceneLoad()
        {
            if (Instance != null)
            {
                Instance.GameStarted();
            }
        }


        /// <summary>
        /// Attempts to cast the global <see cref="Instance"/> to the specified type <typeparamref name="T"/>.
        /// Logs an error and returns <c>null</c> if the cast fails.
        /// </summary>
        /// <typeparam name="T">The target type to cast the instance to.</typeparam>
        /// <returns>The casted instance of type <typeparamref name="T"/>, or <c>null</c> on failure.</returns>
        public static T CastInstance<T>() where T : class
        {
            if (Instance is T CastedInstance)
            {
                return CastedInstance;
            }
            else
            {
                Debug.LogError($"GameInstance Cast Faild for {typeof(T)} Class");
                return null;
            }
        }
        /// <summary>
        /// Attempts to cast the current <see cref="World"/> to the specified type <typeparamref name="T"/>.
        /// Logs an error and returns <c>null</c> if the cast fails.
        /// </summary>
        /// <typeparam name="T">The target type to cast the world to.</typeparam>
        /// <returns>The casted world of type <typeparamref name="T"/>, or <c>null</c> on failure.</returns>
        public static T CastWorld<T>() where T : class
        {
            if (World is T CastedWorld)
            {
                return CastedWorld;
            }
            else
            {
                Debug.LogError($"World Cast Faild for {typeof(T)} Class");
                return null;
            }
        }
        /// <summary>
        /// Adds a new service of type <typeparamref name="T"/> to the game if one does not already exist.
        /// Newly created service will be bound to managers and notified to the global service system.
        /// </summary>
        /// <typeparam name="T">Service type to add.</typeparam>
        /// <param name="author">The object responsible for creating the service (used for provider callbacks).</param>
        /// <returns>The newly created service instance, or <c>null</c> if a service of the same type already exists.</returns>
        public static T AddService<T>(object author) where T : Service, new()
        {
            // Check if a service of this type already exists
            if (Instance.GameServices.Any(s => s.GetType() == typeof(T)))
            {
                Debug.LogWarning($"Service of type {typeof(T).Name} already exists.");
                return null;
            }

            // Create Service
            T newService = new T();
            ((IService)newService).Created(author);
            if (Instance.Managers != null)
            {
                foreach (var manager in Instance.Managers)
                {
                    manager.ResolveService(newService, true);
                }
            }
            ((IMethodSync)Service).ServiceCreated(newService);
            Instance.GameServices.Add(newService);
            return newService;
        }
        /// <summary>
        /// Removes the first service instance of type <typeparamref name="T"/> if present.
        /// Managers and the global service system will be notified of the removal.
        /// </summary>
        /// <typeparam name="T">Service type to remove.</typeparam>
        /// <param name="author">The object requesting the removal (used for provider callbacks).</param>
        /// <returns><c>true</c> if a service was found and removed; otherwise <c>false</c>.</returns>
        public static bool RemoveService<T>(object author) where T : Service
        {
            var service = Instance.GameServices.FirstOrDefault(s => s.GetType() == typeof(T));
            if (service != null)
            {
                if (Instance.Managers != null)
                {
                    foreach (var manager in Instance.Managers)
                    {
                        manager.ResolveService(service, false);
                    }
                }
                ((IMethodSync)Service).ServiceRemoved(service);
                ((IService)service).Deleted(author);
                Instance.GameServices.Remove(service);
                return true;
            }

            Debug.LogWarning($"Service of type {typeof(T).Name} not found to remove.");
            return false;
        }
        /// <summary>
        /// Retrieves the first service instance of type <typeparamref name="T"/> if available.
        /// </summary>
        /// <typeparam name="T">Service type to retrieve.</typeparam>
        /// <returns>The service instance of type <typeparamref name="T"/>, or <c>null</c> if not found.</returns>
        public static T GetService<T>() where T : Service
        {
            return Instance.GameServices.OfType<T>().FirstOrDefault();
        }
        /// <summary>
        /// Retrieves a service by its concrete <see cref="Type"/>.
        /// Returns <c>null</c> and logs an error if the provided type is not a <see cref="Service"/>.
        /// </summary>
        /// <param name="type">Concrete service type to look up.</param>
        /// <returns>The matching service instance, or <c>null</c> if not found or invalid type.</returns>
        public static Service GetService(Type type)
        {
            if (!typeof(Service).IsAssignableFrom(type))
            {
                Debug.LogError($"Type {type.Name} is not a valid Service.");
                return null;
            }

            return Instance.GameServices.FirstOrDefault(s => s.GetType() == type);
        }
        /// <summary>
        /// Attempts to find a service of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Service type to find.</typeparam>
        /// <param name="service">Out parameter that receives the service if found.</param>
        /// <returns><c>true</c> if the service was found; otherwise <c>false</c>.</returns>
        public static bool TryFindService<T>(out T service) where T : Service
        {
            service = Instance.GameServices.OfType<T>().FirstOrDefault();
            return service != null;
        }
        /// <summary>
        /// Returns the type names of all registered game services.
        /// </summary>
        /// <returns>Array of service type names.</returns>
        public string[] GetAllServiceNames()
        {
            return GameServices.Select(service => service.GetType().Name).ToArray();
        }
        /// <summary>
        /// Requests a scene load by build index via the configured <see cref="Service"/>.
        /// If the requested scene is already active, a warning is logged and <c>null</c> is returned.
        /// </summary>
        /// <param name="sceneIndex">Build index of the scene to open.</param>
        /// <returns>A <see cref="Coroutine"/> driving the load operation, or <c>null</c> if not started.</returns>
        public static Coroutine OpenScene(int sceneIndex)
        {
            if (SceneManager.GetActiveScene().buildIndex != sceneIndex)
            {
                return Instance.StartCoroutine(Service.GetLoadScneCorotine(sceneIndex));
            }
            else
            {
                Debug.LogWarning("The scene is already loaded.");
                return null;
            }
        }
        /// <summary>
        /// Requests a scene load using a <see cref="SceneReference"/>.
        /// </summary>
        /// <param name="scene">Reference describing the scene to load.</param>
        /// <returns>A <see cref="Coroutine"/> driving the load operation, or <c>null</c> if not started.</returns>
        public static Coroutine OpenScene(SceneReference scene)
        {
            return OpenScene(scene.ScneName);
        }
        /// <summary>
        /// Requests a scene load by name via the configured <see cref="Service"/>.
        /// If the requested scene is already active, a warning is logged and <c>null</c> is returned.
        /// </summary>
        /// <param name="sceneName">Name of the scene to open.</param>
        /// <returns>A <see cref="Coroutine"/> driving the load operation, or <c>null</c> if not started.</returns>
        public static Coroutine OpenScene(string sceneName)
        {
            if (SceneManager.GetActiveScene().name != sceneName)
            {
                return Instance.StartCoroutine(Service.GetLoadScneCorotine(sceneName));
            }
            else
            {
                Debug.LogWarning("The scene is already loaded.");
                return null;
            }
        }
        /// <summary>
        /// Loads a multi-scene world configuration using the provided <see cref="WorldSceneConfig"/>.
        /// If the persistent scene for the world is already loaded, a warning is logged and <c>null</c> is returned.
        /// </summary>
        /// <param name="WorldScene">World scene configuration to load.</param>
        /// <returns>A <see cref="Coroutine"/> driving the world load operation, or <c>null</c> if not started.</returns>
        public static Coroutine OpenWorld(WorldSceneConfig WorldScene)
        {
            if (SceneManager.GetActiveScene().buildIndex != SceneManager.GetSceneByPath(WorldScene.Persistent).buildIndex)
            {
                return Instance.StartCoroutine(Service.GetLoadWorldCorotine(WorldScene));
            }
            else
            {
                Debug.LogWarning("The Persistent Scene is already loaded.");
                return null;
            }
        }
        /// <summary>
        /// Reloads the currently active scene via the configured <see cref="Service"/>.
        /// </summary>
        /// <returns>A <see cref="Coroutine"/> driving the reload operation.</returns>
        public static Coroutine ReOpenScene()
        {
            return Instance.StartCoroutine(Service.GetLoadScneCorotine(SceneManager.GetActiveScene().name)); ;
        }
        /// <summary>
        /// Finds a manager of type <typeparamref name="T"/> in the current <see cref="World"/>,
        /// falling back to the global game managers if not found.
        /// </summary>
        /// <typeparam name="T">Manager type to find.</typeparam>
        /// <returns>An instance of the manager if found; otherwise <c>null</c>.</returns>
        public static T FindManager<T>() where T : MonoBehaviour
        {
            T Result = null;

            if (World != null)
            {
                Result = World.GetManager<T>();
            }

            if (Result != null)
            {
                return Result;
            }
            return Instance.GetManager<T>();
        }
        /// <summary>
        /// Parents the provided <paramref name="Target"/> GameObject to the game root instance.
        /// </summary>
        /// <param name="Target">The GameObject to hold under the game root.</param>
        public static void HoldGameObject(GameObject Target)
        {
            Target.transform.SetParent(Instance.transform);
        }
        /// <summary>
        /// Searches for a child GameObject with the given name and reparents it to <paramref name="Target"/>.
        /// </summary>
        /// <param name="GameObjectName">Name of the child GameObject to find.</param>
        /// <param name="Target">New parent GameObject to assign.</param>
        /// <returns><c>true</c> if the child was found and reparented; otherwise <c>false</c>.</returns>
        public static bool TryUnholdGameObject(string GameObjectName, out GameObject result)
        {
            Transform[] Childs = Instance.GetComponentsInChildren<Transform>();
            foreach (var item in Childs)
            {
                if (item.gameObject.name == GameObjectName)
                {
                    item.SetParent(null);
                    result = item.gameObject;
                    return true;
                }
            }
            result = null;
            return false;
        }
        /// <summary>
        /// Pauses or resumes the game by setting <see cref="Time.timeScale"/>.
        /// </summary>
        /// <param name="paused">If <c>true</c> the game is paused; otherwise resumed.</param>
        public static void SetPause(bool paused)
        {
            Time.timeScale = paused ? 0 : 1;
        }
        /// <summary>
        /// Sets the global time scale and optionally adjusts fixed delta time for physics safety.
        /// </summary>
        /// <param name="speed">New time scale to apply.</param>
        /// <param name="physicSafe">If <c>true</c> adjusts <see cref="Time.fixedDeltaTime"/> to keep physics stable.</param>
        public static void SetSpeed(float speed, bool physicSafe = true)
        {
            Time.timeScale = speed;
            if (physicSafe)
                Time.fixedDeltaTime = 0.02f * Time.timeScale; // Keeps physics in sync
        }
        /// <summary>
        /// Quits the application. In the Unity Editor this stops play mode instead.
        /// </summary>
        public static void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }


        /// <summary>
        /// Retrieves a manager of type <typeparamref name="T"/> from this game's instantiated managers.
        /// </summary>
        /// <typeparam name="T">Manager type to retrieve.</typeparam>
        /// <returns>The manager instance if found; otherwise <c>null</c>.</returns>
        public T GetManager<T>() where T : class
        {
            foreach (var manager in Managers)
            {
                if (manager.GetManagerClass() is T Result)
                {
                    return Result;
                }
            }
            return null;
        }
        /// <summary>
        /// Retrieves an <see cref="IGameManager"/> by the name of its GameObject.
        /// </summary>
        /// <param name="ObjectName">Name of the manager GameObject to find.</param>
        /// <returns>The matching <see cref="IGameManager"/>, or <c>null</c> if none match.</returns>
        public IGameManager GetManager(string ObjectName)
        {
            foreach (var manger in Managers)
            {
                if (manger.GetManagerClass().gameObject.name == ObjectName)
                {
                    return manger;
                }
            }
            return null;
        }


        /// <summary>
        /// Callback invoked when a new <see cref="World"/> is created or assigned.
        /// Updates the static <see cref="World"/> reference and notifies all services.
        /// </summary>
        /// <param name="NewWorld">The newly initiated world instance.</param>
        private void Notify_OnWorldInitiate(World NewWorld)
        {
            World = NewWorld;
            foreach (var service in GameServices)
            {
                ((IService)service).WorldUpdated();
            }
            OnWorldChanged(World);
        }
        /// <summary>
        /// Handles application quit events: unbinds world callbacks, deletes services and invokes <see cref="OnGameClosed"/>.
        /// </summary>
        private void Notify_OnGameQuit()
        {
            Application.quitting -= Notify_OnGameQuit;
            ((IMethodSync)Service).UnbindMainWorldAdd();
            if (GameServices != null)
            {
                for (int i = 0; i < GameServices.Count; i++)
                {
                    ((IService)GameServices[i]).Deleted(this);
                    GameServices.RemoveAt(i);
                }
            }
            ((IService)Service).Deleted(this);
            OnGameClosed();
        }


        /// <summary>
        /// Called once when the game framework has finished initial initialization.
        /// Implement this to perform game-specific initialization logic.
        /// </summary>
        protected abstract void GameInitialized();
        /// <summary>
        /// Called after the first scene has been loaded and the game has started.
        /// Implement this to perform logic that should run once the first scene is active.
        /// </summary>
        protected abstract void GameStarted();
        /// <summary>
        /// Called when the current <see cref="World"/> reference changes.
        /// Implement to react to world switches.
        /// </summary>
        /// <param name="NewWorld">The new active world.</param>
        protected abstract void OnWorldChanged(World NewWorld);
        /// <summary>
        /// Called when the game is closing. Implement to perform cleanup logic.
        /// </summary>
        protected abstract void OnGameClosed();

    }
}