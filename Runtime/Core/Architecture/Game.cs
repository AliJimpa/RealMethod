using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace RealMethod
{
    /// <summary>
    /// Represents the possible outcomes of a Game process.
    /// </summary>
    public enum GameProcess
    {
        /// <summary>
        /// The process finished successfully.
        /// </summary>
        Success,
        /// <summary>
        /// The process failed.
        /// </summary>
        Failure,
        /// <summary>
        /// The process was cancelled before completion.
        /// </summary>
        Cancelled
    }

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
        /// The core <see cref="GameBridge"/> implementation used for scene/world loading and basic events.
        /// </summary>
        public static GameBridge Bridge { get; private set; }
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
        /// Reperesent Game State that youser can define state in ProjectSetting
        /// </summary>
        public static GlobalEnum State { get; private set; } = 0;
        /// <summary>
        /// Indicates whether a scene or world load operation is currently in progress.
        /// </summary>
        public static bool IsPaused
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.IsGamePaused();
                }
                else
                {
                    Debug.LogWarning("GameInstance did not called !");
                    return false;
                }
            }
        }
        public static bool IsGameInitialized { get; private set; } = false;
        /// <summary>
        /// Indicates whether the game is in loading stage
        /// Return true when game in loading section for new scene
        /// </summary>
        public static bool IsLoading => Instance.IsGameLoading();
        /// <summary>
        /// Event invoked when a scene or world starts or finishes loading.
        /// The boolean parameter is true when loading starts and false when loading ends.
        /// </summary>
        public static event Action<bool> OnSceneLoading
        {
            add { ((ILoadScneBridge)Bridge).OnSceneLoading += value; }
            remove { ((ILoadScneBridge)Bridge).OnSceneLoading -= value; }
        }
        /// <summary>
        /// Event invoked during scene or world loading to report progress.
        /// The float parameter represents the loading progress from 0 (start) to 1 (complete).
        /// </summary>
        public static event Action<float> OnSceneLoadingProcess
        {
            add { ((ILoadScneBridge)Bridge).OnSceneLoadingProcess += value; }
            remove { ((ILoadScneBridge)Bridge).OnSceneLoadingProcess -= value; }
        }
        /// <summary>
        /// This action called every time your game ready to play after load Scene & setup RealMethod
        /// you can enshure that your game and world do anything and player can ready to play game
        /// when you change scene after world initiate this evet invoke again.
        /// </summary>
        public static event Action OnReady
        {
            add { ((IRelationBridge)Bridge).OnGameReady += value; }
            remove { ((IRelationBridge)Bridge).OnGameReady -= value; }
        }
        /// <summary>
        /// Invoked when the process finishes.
        /// Process in your game take define with yourelf.
        /// (for example: show win screen, game over UI, load next level, etc).
        /// </summary>
        public static event Action<GameProcess> OnCompleted;
        /// <summary>
        /// Invoked when game state changed.
        /// </summary>
        public static event Action<int> OnStateChanged;




        /// <summary>
        /// Cached array of managers that were instantiated from configured game prefabs.
        /// </summary>
        private IGameManager[] Managers;
        /// <summary>
        /// List of runtime-registered <see cref="Service"/> instances owned by the game.
        /// </summary>
        private readonly Dictionary<Type, IService> Services = new();




        /// <summary>
        /// Initializes the game singleton and core systems on subsystem registration.
        /// This sets up the <see cref="Instance"/>, game <see cref="Service"/>,
        /// configuration, prefabs and managers and registers quit callbacks.
        /// Invoked when starting up the runtime. Called before the first scene is loaded.
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

            // Initiate GameClass
            var RealObject = new GameObject("RealMethod");
            Type TargetClass = ProjectSettings.GetGameType();
            if (TargetClass == null)
            {
                Debug.LogWarning("GameInstanceClass that was empty. DefaultGame Created");
                Instance = RealObject.AddComponent<DefultGame>();
            }
            else
            {
                if (typeof(Game).IsAssignableFrom(TargetClass))
                {
                    Instance = (Game)RealObject.AddComponent(TargetClass);
                }
                else
                {
                    Debug.LogWarning($"Component of type {TargetClass} is not assignable from Game. DefaultGame Created");
                    Instance = RealObject.AddComponent<DefultGame>();
                }
            }
            Instance.OnProjectSeettingLoaded(ref ProjectSettings);

            Instance.OnGameOpen();

            // Create GameBridge
            Type targetService = ProjectSettings.GetBridgeType();
            if (targetService == null)
            {
                Debug.LogWarning($"GetGameBridgeType that was empty. DefaultGameBridge Created");
                Bridge = new DefaultGameBridge();
            }
            else
            {
                if (typeof(GameBridge).IsAssignableFrom(targetService))
                {
                    try
                    {
                        Bridge = (GameBridge)Activator.CreateInstance(targetService);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to instantiate {targetService}: {ex.Message}. DefaultGameBridge Created");
                        Bridge = new DefaultGameBridge();
                    }
                }
                else
                {
                    Debug.LogWarning($"Type {targetService} is not assignable to GameBridge. DefaultGameBridge Created");
                    Bridge = new DefaultGameBridge();
                }
            }
            ((IRelationBridge)Bridge).BindWorldCreated(Instance.Notify_OnWorldInitiate);
            ((IService)Bridge).Created(Instance);

            // Set GameConfig 
            if (ProjectSettings.GetGameConfigAsset() != null)
            {
                Config = ProjectSettings.GetGameConfigAsset();
            }
            else
            {
                Config = ScriptableObject.CreateInstance<DefaultGameConfig>();
            }

            // Initiate GamePrefab & Managers
            HashSet<IGameManager> CashManagers = new HashSet<IGameManager>();

            if (ProjectSettings.GetPrefab_1() != null)
            {
                GameObject newobj = Instantiate(ProjectSettings.GetPrefab_1());
                newobj.name = "GameScope(Runtime)";
                foreach (var manager in newobj.GetComponents<IGameManager>())
                {
                    if (!CashManagers.Contains(manager))
                    {
                        manager.InitiateManager(true);
                        CashManagers.Add(manager);
                    }
                    else
                    {
                        Debug.LogError($"You should not use a manager {manager} twice");
                        throw new NotImplementedException();
                    }

                }
                newobj.transform.SetParent(RealObject.transform);
            }
#if UNITY_EDITOR
            if (ProjectSettings.GetPrefab_2() != null)
            {
                GameObject newobj = Instantiate(ProjectSettings.GetPrefab_2());
                newobj.name = "GameScope(Editor)";
                foreach (var manager in newobj.GetComponents<IGameManager>())
                {
                    if (!CashManagers.Contains(manager))
                    {
                        manager.InitiateManager(true);
                        CashManagers.Add(manager);
                    }
                    else
                    {
                        Debug.LogError($"You should not use a manager {manager} twice");
                        throw new NotImplementedException();
                    }

                }
                newobj.transform.SetParent(RealObject.transform);
            }
#endif
#if UNITY_SERVER
            if (ProjectSettings.GetPrefab_3() != null)
            {
                GameObject newobj = Instantiate(ProjectSettings.GetPrefab_3());
                newobj.name = "GameScope(Server)";
                foreach (var manager in newobj.GetComponents<IGameManager>())
                {
                    if (!CashManagers.Contains(manager))
                    {
                        manager.InitiateManager(true);
                        CashManagers.Add(manager);
                    }
                    else
                    {
                        Debug.LogError($"You should not use a manager {manager} twice");
                        throw new NotImplementedException();
                    }

                }
                newobj.transform.SetParent(RealObject.transform);
            }
#endif
            Instance.Managers = CashManagers.ToArray();

            // Unload Project Setting
            Resources.UnloadAsset(ProjectSettings);
            ProjectSettings = null;

            // Move Self GameObject to DontDestroy
            DontDestroyOnLoad(RealObject);
            Application.quitting += Instance.Notify_OnGameQuit;
        }
        /// <summary>
        /// Called before any scene is loaded. Invokes <see cref="OnGameInitialized"/> on the active instance.
        /// Invoked when the first scene's objects are loaded into memory but before Awake has been called.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RuntimeBeforeSceneLoad()
        {
            if (Instance != null)
            {
                IsGameInitialized = true;
                Instance.OnGameInitialized();
            }
        }
        /// <summary>
        /// Called after a scene has finished loading. Invokes <see cref="OnGameStart"/> on the active instance.
        /// Right after all Awake() and OnEnable() calls but befor Start()
        /// Invoked when the first scene's objects are loaded into memory but before Awake has been called.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RuntimeAfterSceneLoad()
        {
            if (Instance != null)
            {
                Instance.OnGameStart();
            }
        }


        /// <summary>
        /// Attempts to invoke 
        /// </summary>
        public static void Complete(GameProcess result)
        {
            OnCompleted.Invoke(result);
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
            // Check if you game not initialized
            if (!IsGameInitialized)
            {
                Debug.LogWarning($"Game doesn't initialized !");
                return null;
            }

            // Create Service
            T newService = new T();
            if (!RegisterService(newService, author))
                return null;
            if (Instance.Managers != null)
            {
                foreach (var manager in Instance.Managers)
                {
                    manager.ResolveService(newService, true);
                }
            }
            ((IRelationBridge)Bridge).ServiceCreated(newService);
            return newService;
        }
        /// <summary>
        /// Register new instance (<typeparamref name="T"/>) that include 'IService' interface .
        /// New Service instance register to the game if one des not already exist
        /// </summary>
        /// <typeparam name="T">Any Type can inplement 'IService' interface</typeparam>
        /// <param name="service">Any Instance can inplement 'IService' interface </param>
        /// <param name="author">The object responsible for creating the service (used for provider callbacks).</param>
        /// <returns>The newly created service instance, or <c>null</c> if a service of the same type already exists.</returns>
        public static bool RegisterService<T>(T service, object author)
        {
            if (service is IService provider)
            {
                Type TypeService = typeof(T);

                if (Instance.Services.ContainsKey(TypeService))
                {
                    Debug.LogError($"Service {TypeService} already registered.");
                    return false;
                }

                provider.Created(author);
                Instance.Services[TypeService] = provider;
                return true;
            }
            else
            {
                Debug.LogError($"The class '{service.GetType().Name}' must implement the IService interface.");
                return false;
            }
        }
        /// <summary>
        /// Removes the service instance of type <typeparamref name="T"/> if present.
        /// Managers and the global service system will be notified of the removal.
        /// </summary>
        /// <typeparam name="T">Service type to remove.</typeparam>
        /// <param name="author">The object requesting the removal (used for provider callbacks).</param>
        /// <returns><c>true</c> if a service was found and removed; otherwise <c>false</c>.</returns>
        public static bool RemoveService<T>(object author) where T : Service
        {
            Type TypeService = typeof(T);
            var provider = Instance.Services[TypeService];
            if (provider != null)
            {
                if (provider.GetServiceClass() is Service service)
                {
                    if (Instance.Managers != null)
                    {
                        foreach (var manager in Instance.Managers)
                        {
                            manager.ResolveService(service, false);
                        }
                    }
                    ((IRelationBridge)Bridge).ServiceRemoved(service);
                }
                provider.Deleted(author);
                Unregister<T>();
                return true;
            }

            Debug.LogWarning($"Service of type {typeof(T).Name} not found to remove.");
            return false;
        }
        /// <summary>
        /// Remove service instance 
        /// </summary>
        /// <typeparam name="T">The object type that implement 'IService'</typeparam>
        public static void Unregister<T>()
        {
            Instance.Services.Remove(typeof(T));
        }
        /// <summary>
        /// Retrieves the service instance of type <typeparamref name="T"/> if available.
        /// </summary>
        /// <typeparam name="T">Service type to retrieve.</typeparam>
        /// <returns>The service instance of type <typeparamref name="T"/>, or <c>null</c> if not found.</returns>
        public static T GetService<T>()
        {
            Type TypeService = typeof(T);

            if (Instance.Services.TryGetValue(TypeService, out var provider))
                return (T)provider.GetServiceClass();

            Debug.LogError($"Service {TypeService} not found.");
            return default;
        }
        /// <summary>
        /// Attempts to find a service of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Service type to find.</typeparam>
        /// <param name="service">Out parameter that receives the service if found.</param>
        /// <returns><c>true</c> if the service was found; otherwise <c>false</c>.</returns>
        public static bool TryGetService<T>(out T service) where T : Service
        {
            if (Instance.Services.TryGetValue(typeof(T), out var provider))
            {
                service = (T)provider.GetServiceClass();
                return true;
            }

            service = default;
            return false;
        }
        /// <summary>
        ///  Clear all services in Game
        /// </summary>
        public static void ClearService()
        {
            Instance.Services.Clear();
        }
        /// <summary>
        /// Returns the type names of all registered game services.
        /// </summary>
        /// <returns>Array of service type names.</returns>
        public string[] GetAllServiceNames()
        {
            return Services.Select(service => service.GetType().Name).ToArray();
        }
        /// <summary>
        /// Requests a scene load by build index .
        /// If the requested scene is already active, a warning is logged and <c>null</c> is returned.
        /// </summary>
        /// <param name="sceneIndex">Build index of the scene to open.</param>
        /// <returns>A <see cref="Coroutine"/> driving the load operation, or <c>null</c> if not started.</returns>
        public static Coroutine OpenScene(int sceneIndex)
        {
            if (SceneManager.GetActiveScene().buildIndex != sceneIndex)
            {
                return Instance.StartCoroutine(Bridge.GetLoadScneCorotine(sceneIndex));
            }
            else
            {
                Debug.LogWarning("The scene is already loaded.");
                return null;
            }
        }
        /// <summary>
        /// Requests a scene load using a <see cref="SceneReference"/>.
        /// Note: this method work in [Editor].
        /// </summary>
        /// <param name="scene">Reference describing the scene to load.</param>
        /// <returns>A <see cref="Coroutine"/> driving the load operation, or <c>null</c> if not started.</returns>
        public static Coroutine OpenScene(SceneAsset scene)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorSceneManager.OpenScene(scene.ScenePath, OpenSceneMode.Single);
                return null;
            }
            else
            {
                return OpenScene(scene.ScneName);
            }
#else
            return OpenScene(scene.ScneName);
#endif
        }
        /// <summary>
        /// Requests a scene load by name.
        /// If the requested scene is already active, a warning is logged and <c>null</c> is returned.
        /// </summary>
        /// <param name="sceneName">Name of the scene to open.</param>
        /// <returns>A <see cref="Coroutine"/> driving the load operation, or <c>null</c> if not started.</returns>
        public static Coroutine OpenScene(string sceneName)
        {
            if (SceneManager.GetActiveScene().name != sceneName)
            {
                return Instance.StartCoroutine(Bridge.GetLoadScneCorotine(sceneName));
            }
            else
            {
                Debug.LogWarning("The scene is already loaded.");
                return null;
            }
        }
        /// <summary>
        /// Requests a Add scene by build index via the configured <see cref="Service"/>.
        /// If the requested scene is already active, a warning is logged and <c>null</c> is returned.
        /// </summary>
        /// <param name="sceneIndex">Build index of the scene to open.</param>
        /// <param name="callback">callback event when scene complitly added</param>
        public static void AddScene(int sceneIndex, Action callback)
        {
            if (SceneManager.GetActiveScene().buildIndex != sceneIndex)
            {
                Instance.StartCoroutine(Bridge.GetAddScneCorotine(sceneIndex, callback));
            }
            else
            {
                Debug.LogWarning("The scene is already loaded.");
            }
        }
        /// <summary>
        /// Requests a scene load using a <see cref="SceneReference"/>.
        /// Note: this method work in [Editor].
        /// </summary>
        /// <param name="scene">Reference describing the scene to load.</param>
        /// <param name="callback">callback event when scene complitly added</param>
        public static void AddScene(SceneAsset scene, Action callback)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorSceneManager.OpenScene(scene.ScenePath, OpenSceneMode.Additive);
            }
            else
            {
                AddScene(scene.ScneName, callback);
            }
#else
            AddScene(scene.ScneName, callback);
#endif
        }
        /// <summary>
        /// Requests a scene load by name.
        /// If the requested scene is already active, a warning is logged and <c>null</c> is returned.
        /// </summary>
        /// <param name="sceneName">Name of the scene to open.</param>
        /// <param name="callback">callback event when scene complitly added</param>
        public static void AddScene(string sceneName, Action callback)
        {
            if (SceneManager.GetActiveScene().name != sceneName)
            {
                Instance.StartCoroutine(Bridge.GetAddScneCorotine(sceneName, callback));
            }
            else
            {
                Debug.LogWarning("The scene is already loaded.");
            }
        }
        /// <summary>
        /// Loads a multi-scene world configuration using the provided <see cref="WorldSceneConfig"/>.
        /// If the persistent scene for the world is already loaded, a warning is logged and <c>null</c> is returned.
        /// Note: this method work in [Editor].
        /// </summary>
        /// <param name="WorldScene">World scene configuration to load.</param>
        /// <returns>A <see cref="Coroutine"/> driving the world load operation, or <c>null</c> if not started.</returns>
        public static Coroutine OpenWorld(WorldAsset WorldScene)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                WorldScene.OnAssetClick();
                return null;
            }
            else
            {
                if (SceneManager.GetActiveScene().buildIndex != SceneManager.GetSceneByPath(WorldScene.Persistent).buildIndex)
                {
                    return Instance.StartCoroutine(Bridge.GetLoadWorldCorotine(WorldScene));
                }
                else
                {
                    Debug.LogWarning("The Persistent Scene is already loaded.");
                    return null;
                }
            }
#else
            if (SceneManager.GetActiveScene().buildIndex != SceneManager.GetSceneByPath(WorldScene.Persistent).buildIndex)
            {
                return Instance.StartCoroutine(Bridge.GetLoadWorldCorotine(WorldScene));
            }
            else
            {
                Debug.LogWarning("The Persistent Scene is already loaded.");
                return null;
            }
#endif

        }
        /// <summary>
        /// Reloads the currently active scene via the configured <see cref="Service"/>.
        /// </summary>
        /// <returns>A <see cref="Coroutine"/> driving the reload operation.</returns>
        public static Coroutine ReOpenScene()
        {
            return Instance.StartCoroutine(Bridge.GetLoadScneCorotine(SceneManager.GetActiveScene().name)); ;
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
        /// <param name="TargetName">The GameObject name change to new Name for searching.</param>
        public static void HoldGameObject(GameObject Target, Name16 TargetName)
        {
            Target.name = TargetName;
            Target.transform.SetParent(Instance.transform);
        }
        /// <summary>
        /// Searches for a child GameObject with the given name and reparents it to <paramref name="Target"/>.
        /// </summary>
        /// <param name="GameObjectName">Name of the child GameObject to find.</param>
        /// <param name="Target">New parent GameObject to assign.</param>
        /// <returns><c>true</c> if the child was found and reparented; otherwise <c>false</c>.</returns>
        public static bool TryUnholdGameObject(Name16 GameObjectName, out GameObject result)
        {
            string TargetName = GameObjectName.ToString();
            Transform[] Childs = Instance.GetComponentsInChildren<Transform>();
            foreach (var item in Childs)
            {
                if (item.gameObject.name == TargetName)
                {
                    item.SetParent(World.transform);
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
        /// Sets the GameState to new state you want. 
        /// </summary>
        /// <param name="NewState">Target State you want to cahgne</param>
        /// <param name="author">the refrence from who want to change the gamestate</param>
        /// <returns></returns>
        public static bool SetState(int NewState, object author)
        {
            if (Instance.CanChangeState(State, NewState, author))
            {
                State = NewState;
                OnStateChanged?.Invoke(State);
                return true;
            }
            return false;
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
        /// Logs a message to the Unity Console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message)
        {
            Debug.Log(message);
        }
        /// <summary>
        /// Logs a message to the Unity Console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message, UnityEngine.Object context)
        {
            Debug.Log(message, context);
        }
        /// <summary>
        ///  A variant of Debug.Log that logs a warning message to the console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }
        /// <summary>
        ///  A variant of Debug.Log that logs a warning message to the console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message, UnityEngine.Object context)
        {
            Debug.LogWarning(message, context);
        }
        /// <summary>
        ///  A variant of Debug.Log that logs an error message to the console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }
        /// <summary>
        /// A variant of Debug.Log that logs an error message to the console.
        /// </summary>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object message, UnityEngine.Object context)
        {
            Debug.LogError(message, context);
        }
        /// <summary>
        /// A variant of Debug.Log that logs an error message to the console.
        /// </summary>
        /// <param name="exception">Runtime Exception.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }
        /// <summary>
        /// A variant of Debug.Log that logs an error message to the console.
        /// </summary>
        /// <param name="exception">Runtime Exception</param>
        /// <param name="context">Object to which the message applies.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void LogException(Exception exception, UnityEngine.Object context)
        {
            Debug.LogException(exception, context);
        }
        /// <summary>
        /// Assert a condition and logs an error message to the Unity console on failure.
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void Assert(bool condition)
        {
            Debug.Assert(condition);
        }
        /// <summary>
        /// Assert a condition and logs an error message to the Unity console on failure.
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void Assert(bool condition, object message)
        {
            Debug.Assert(condition, message);
        }
        /// <summary>
        /// Assert a condition and logs an error message to the Unity console on failure.
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="context">Object to which the message applies.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void Assert(bool condition, UnityEngine.Object context)
        {
            Debug.Assert(condition, context);
        }
        /// <summary>
        /// Assert a condition and logs an error message to the Unity console on failure.
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void Assert(bool condition, string message)
        {
            Debug.Assert(condition, message);
        }
        /// <summary>
        /// Assert a condition and logs an error message to the Unity console on failure.
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void Assert(bool condition, object message, UnityEngine.Object context)
        {
            Debug.Assert(condition, message, context);
        }
        /// <summary>
        /// Assert a condition and logs an error message to the Unity console on failure.
        /// </summary>
        /// <param name="condition">Condition you expect to be true.</param>
        /// <param name="message">String or object to be converted to string representation for display.</param>
        /// <param name="context">Object to which the message applies.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        public static void Assert(bool condition, string message, UnityEngine.Object context)
        {
            Debug.Assert(condition, message, context);
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
        /// Called after GameInstance Created befor GameBridge , GameConfig & Managers Initilized
        /// if you use custom PtojectSetttingAsset class you can use instances after loaded in game 
        /// this refrence unloaded befor OnGameInitialized Called.
        /// </summary>
        /// <param name="setting">ProjectSettingAsset refrence</param>
        protected virtual void OnProjectSeettingLoaded(ref ProjectSettingAsset setting)
        {
            // Nothing todo
        }
        /// <summary>
        /// Check GamePause with time or any custom override.
        /// </summary>
        /// <returns><c>true</c> if the timescale is 0; otherwise <c>false</c>.</returns>
        protected virtual bool IsGamePaused()
        {
            return Time.timeScale == 0;
        }
        /// <summary>
        /// Check GameLoading with Bridge to check loading stage.
        /// </summary>
        /// <returns><c>true</c> if any scne in loading stage; otherwise <c>false</c>.</returns>
        protected virtual bool IsGameLoading()
        {
            return ((ILoadScneBridge)Bridge).IsLoading;
        }
        /// <summary>
        /// Check for changing state from A to B by author.
        /// </summary>
        /// <param name="A">Current state</param>
        /// <param name="B">Target state</param>
        /// <param name="author">this object want to change state</param>
        /// <returns></returns>
        protected virtual bool CanChangeState(int A, int B, object author)
        {
            return true;
        }


        /// <summary>
        /// Callback invoked when a new <see cref="World"/> is created or assigned.
        /// Updates the static <see cref="World"/> reference and notifies all services.
        /// </summary>
        /// <param name="NewWorld">The newly initiated world instance.</param>
        private void Notify_OnWorldInitiate(World NewWorld)
        {
            foreach (var service in Services)
            {
                service.Value.ChangeWorld(NewWorld);
            }
            World = NewWorld;
            OnWorldChanged(World);
        }
        /// <summary>
        /// Handles application quit events: unbinds world callbacks, deletes services and invokes <see cref="OnGameClosed"/>.
        /// </summary>
        private void Notify_OnGameQuit()
        {
            Application.quitting -= Notify_OnGameQuit;
            ((IRelationBridge)Bridge).UnbindWorldCreated();
            if (Services != null)
            {
                foreach (var service in Services)
                {
                    service.Value.Deleted(this);
                }
                ClearService();
            }
            ((IService)Bridge).Deleted(this);
            OnGameClosed();
        }



        /// <summary>
        /// Called once when game opend in currect platform
        /// Implement anything you want initiate befor realmethod initiate
        /// Invoked when starting up the runtime. Called before the first scene is loaded.
        /// </summary>
        protected abstract void OnGameOpen();
        /// <summary>
        /// Called once when the game framework has finished initial initialization.
        /// Implement this to perform game-specific initialization logic.
        /// Invoked when the first scene's objects are loaded into memory but before Awake has been called.
        /// </summary>
        protected abstract void OnGameInitialized();
        /// <summary>
        /// Called after the first scene has been loaded and the game has started.
        /// Implement this to perform logic that should run once the first scene is active.
        /// Right after all Awake() and OnEnable() calls but befor Start()
        /// Invoked when the first scene's objects are loaded into memory but before Awake has been called.
        /// </summary>
        protected abstract void OnGameStart();
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