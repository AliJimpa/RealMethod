using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace RealMethod
{

    /// <summary>
    /// Core game singleton that manages the active <see cref="World"/>, registered <see cref="IService"/>s,
    /// configuration and high-level game lifecycle (initialization, start, and shutdown).
    /// Derive from this class to implement project-specific behavior for the game's lifecycle hooks.
    /// </summary>
    public abstract class Game : Scope
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
        public static readonly EnumStorage State = new EnumStorage();
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
        /// <summary>
        /// Indicates whether the game is in loading stage
        /// Return true when game in loading section for new scene
        /// </summary>
        public static bool IsLoading
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.IsGameLoading();
                }
                else
                {
                    Debug.LogWarning("GameInstance did not called !");
                    return false;
                }
            }
        }
        /// <summary>
        /// Indicate whether the game complitly initialized.
        /// after true OnGameInitialized called.
        /// befor first scene load
        /// </summary>
        public static bool IsGameInitialized { get; private set; } = false;




        /// <summary>
        /// Initializes the game singleton and core systems on subsystem registration.
        /// This sets up the <see cref="Instance"/>, game <see cref="Bridge"/>,
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
                Bridge = new DefaultGameBridge(Instance.Notify_OnWorldInitiate);
            }
            else
            {
                if (typeof(GameBridge).IsAssignableFrom(targetService))
                {
                    try
                    {
                        var parameter = new object[1] { (Action<World>)Instance.Notify_OnWorldInitiate };
                        Bridge = (GameBridge)Activator.CreateInstance(targetService, parameter);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to instantiate {targetService}: {ex.Message}. DefaultGameBridge Created");
                        Bridge = new DefaultGameBridge(Instance.Notify_OnWorldInitiate);
                    }
                }
                else
                {
                    Debug.LogWarning($"Type {targetService} is not assignable to GameBridge. DefaultGameBridge Created");
                    Bridge = new DefaultGameBridge(Instance.Notify_OnWorldInitiate);
                }
            }

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
            GameObject[] Objects = new GameObject[3];
            if (ProjectSettings.GetPrefab_1() != null)
            {
                GameObject newobj = Instantiate(ProjectSettings.GetPrefab_1());
                newobj.name = "GameScope(Runtime)";
                Objects[0] = newobj;
                newobj.transform.SetParent(RealObject.transform);
            }
#if UNITY_EDITOR
            if (ProjectSettings.GetPrefab_2() != null)
            {
                GameObject newobj = Instantiate(ProjectSettings.GetPrefab_2());
                newobj.name = "GameScope(Editor)";
                Objects[1] = newobj;
                newobj.transform.SetParent(RealObject.transform);
            }
#endif
#if UNITY_SERVER
            if (ProjectSettings.GetPrefab_3() != null)
            {
                GameObject newobj = Instantiate(ProjectSettings.GetPrefab_3());
                newobj.name = "GameScope(Server)";
                Objects[2] = newobj;
                newobj.transform.SetParent(RealObject.transform);
            }
#endif
            Instance.OpenScope(Objects);

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
        /// Retrieves a manager of type <typeparamref name="T"/> from the active manager scopes.
        /// The search is performed in the following order:
        /// 1. The current <c>World</c> scope (if available).
        /// 2. The global <c>Instance</c> scope.
        /// </summary>
        /// <typeparam name="T">The type of manager to retrieve.</typeparam>
        /// <returns>
        /// The manager of type <typeparamref name="T"/> if found; otherwise <c>null</c>.
        /// </returns>
        public static T GetManager<T>() where T : Component, IGameManager
        {
            if (World != null && World.TryFindGameManager(out T worldResult))
                return worldResult;

            if (Instance.TryFindGameManager(out T gameResult))
                return gameResult;

            return null;
        }
        /// <summary>
        /// Retrieves the service instance of type <typeparamref name="T"/> if available.
        /// </summary>
        /// <typeparam name="T">IService type to retrieve.</typeparam>
        /// <returns>The service instance of type <typeparamref name="T"/>, or <c>null</c> if not found.</returns>
        public static T GetService<T>() where T : IService
        {
            if (Services.TryGet(out T Myservice))
            {
                return Myservice;
            }
            else
            {
                return default;
            }
        }
        /// <summary>
        /// Injects dependencies into the specified target object using the internal dependency injection system.
        /// </summary>
        /// <param name="target">The object whose dependencies should be injected.</param>
        public static void Inject(object target)
        {
            DInjection.Inject(target);
        }
        /// <summary>
        /// Registers and enables a feature instance in the dependency injection container.
        /// </summary>
        /// <typeparam name="F">Type of the feature implementing <see cref="IFeature"/>.</typeparam>
        /// <param name="instance">The feature instance to register.</param>
        /// <param name="overwrite">
        /// If true, replaces an existing registered feature of the same type; otherwise keeps the existing one.
        /// </param>
        public static void Enable<F>(F instance, bool overwrite = false) where F : IFeature
        {
            DInjection.Register(instance, overwrite);
        }
        /// <summary>
        /// Disables and unregisters a feature of the specified type from the dependency injection container.
        /// </summary>
        /// <typeparam name="F">Type of the feature implementing <see cref="IFeature"/>.</typeparam>
        /// <returns>True if the feature was successfully removed; otherwise false.</returns>
        public static bool Disable<F>() where F : IFeature
        {
            return DInjection.Unregister<F>();
        }
        /// <summary>
        /// Publishes an event to all subscribers of the specified event type.
        /// </summary>
        /// <typeparam name="T">Type of the event.</typeparam>
        /// <param name="eventData">The event data to publish.</param>
        public static void Publish<T>(T eventData) where T : IEvent
        {
            EventBus.Publish(eventData);
        }
        /// <summary>
        /// Subscribes a listener to a specific event type.
        /// </summary>
        /// <typeparam name="T">Type of the event to listen for.</typeparam>
        /// <param name="listener">Callback invoked when the event is published.</param>
        public static void Subscribe<T>(Action<T> listener) where T : IEvent
        {
            EventBus.Subscribe(listener);
        }
        /// <summary>
        /// Unsubscribes a listener from a specific event type.
        /// </summary>
        /// <typeparam name="T">Type of the event.</typeparam>
        /// <param name="listener">The listener to remove.</param>
        public static void Unsubscribe<T>(Action<T> listener) where T : IEvent
        {
            EventBus.Subscribe(listener);
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
        /// Requests a Add scene by build index via the configured <see cref="IService"/>.
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
        /// Reloads the currently active scene via the configured <see cref="IService"/>.
        /// </summary>
        /// <returns>A <see cref="Coroutine"/> driving the reload operation.</returns>
        public static Coroutine ReOpenScene()
        {
            return Instance.StartCoroutine(Bridge.GetLoadScneCorotine(SceneManager.GetActiveScene().name)); ;
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
        /// Saves the provided file using the active <see cref="IStorageService"/> implementation.
        /// Logs a warning if no save system is available.
        /// </summary>
        public static void Save(bool cloude = false)
        {
            IStorageService storage = GetService<IStorageService>();
            if (storage != null)
            {
                storage.Save();
            }
            else
            {
                Debug.LogWarning("There is not any ISaveSystem Implemntation");
            }

            if (cloude)
            {
                ICloudService cloudeserv = GetService<ICloudService>();
                if (cloudeserv != null)
                {
                    cloudeserv.Save();
                }
                else
                {
                    Debug.LogWarning("There is not any ICloudService Implemntation");
                }
            }
        }
        /// <summary>
        /// Loads the provided file using the active <see cref="IStorageService"/> implementation.
        /// Logs a warning if no save system is available.
        /// </summary>
        public static void Load(bool cloude = false)
        {
            IStorageService storage = GetService<IStorageService>();
            if (storage != null)
            {
                storage.Load();
            }
            else
            {
                Debug.LogWarning("There is not any ISaveSystem Implemntation");
            }

            if (cloude)
            {
                ICloudService cloudeserv = GetService<ICloudService>();
                if (cloudeserv != null)
                {
                    cloudeserv.Load();
                }
                else
                {
                    Debug.LogWarning("There is not any ICloudService Implemntation");
                }
            }
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
        [HideInCallstack]
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
        [HideInCallstack]
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
        [HideInCallstack]
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
        [HideInCallstack]
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
        [HideInCallstack]
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
        [HideInCallstack]
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
        [HideInCallstack]
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
        [HideInCallstack]
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
        [HideInCallstack]
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
        [HideInCallstack]
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
        [HideInCallstack]
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
        [HideInCallstack]
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
        [HideInCallstack]
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
        [HideInCallstack]
        public static void Assert(bool condition, string message, UnityEngine.Object context)
        {
            Debug.Assert(condition, message, context);
        }
        /// <summary>
        /// Adds a new draw task to the rendering queue.
        /// </summary>
        /// <param name="element">The draw task to add. Ignored if null.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        [HideInCallstack]
        public static void Draw(IDrawTask element)
        {
            if (element != null)
            {
                Instance.DrawTasks.Add(element);
                if (element.Priority != 0)
                {
                    Instance.DrawTasks.Sort((a, b) => a.Priority.CompareTo(b.Priority));
                }
                element.Active();
            }
        }
        /// <summary>
        /// Removes the specified draw task from the rendering queue.
        /// </summary>
        /// <param name="element">The draw task to remove.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        [HideInCallstack]
        public static void Erase(IDrawTask element)
        {
            Instance.DrawTasks.Remove(element);
            element.Deactive();
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
            return Bridge.IsLoading;
        }



        /// <summary>
        /// Callback invoked when a new <see cref="World"/> is created or assigned.
        /// Updates the static <see cref="World"/> reference and notifies all services.
        /// </summary>
        /// <param name="NewWorld">The newly initiated world instance.</param>
        private void Notify_OnWorldInitiate(World NewWorld)
        {
            World = NewWorld;
            OnWorldChanged(World);
        }
        /// <summary>
        /// Handles application quit events: unbinds world callbacks, deletes modules and invokes <see cref="OnGameClosed"/>.
        /// </summary>
        private void Notify_OnGameQuit()
        {
            Application.quitting -= Notify_OnGameQuit;
            OnGameClosed();
            // State
            ((IDisposable)State).Dispose();
            // Scope
            CloseScope();
            // Bridge
            ((IDisposable)Bridge).Dispose();
            Bridge = null;
            // Draw
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            DrawTasks.Clear();
#endif
            // GC
#if UNITY_EDITOR
            // Debug only: force GC to verify no references remain
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
#endif
            // Kernel
            ClearKernel();
        }


#if UNITY_EDITOR
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"This [{this}] component should not be added manually to the scene.");
                Destroy(this);
            }
        }
        private void OnEnable()
        {

        }
        private void Start()
        {

        }
        private void OnDisable()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return; // Ignore disable caused by editor closing play mode

            if (gameObject.activeSelf == false)
            {
                Debug.LogWarning($"You can't Deactive {gameObject.name} GameObject");
                gameObject.SetActive(true);
            }
            if (enabled == false)
            {
                Debug.LogWarning($"You can't Disable {GetType().Name} Component");
                enabled = true;
            }
        }
        private void LateUpdate()
        {
            if (transform.position != Vector3.zero)
            {
                Debug.LogWarning($"{gameObject.name} position should not be changed!");
                transform.position = Vector3.zero;
            }
            if (transform.rotation != Quaternion.identity)
            {
                Debug.LogWarning($"{gameObject.name} rotation should not be changed!");
                transform.rotation = Quaternion.identity;
            }
        }
        private void Update()
        {

        }
        private void FixedUpdate()
        {

        }
        /// <summary>
        /// Returns the type names of all registered game services.
        /// </summary>
        /// <returns>Array of service type names.</returns>
        public override IInspectorInfo[] GetAllInfo()
        {
            List<IInspectorInfo> Result = new List<IInspectorInfo>(10) { Bridge, Config };
            foreach (var item in base.GetAllInfo())
            {
                Result.Add(item);
            }
            return Result.ToArray();
        }
#endif


#if UNITY_EDITOR || DEVELOPMENT_BUILD
        /// <summary>
        /// A list containing all active draw tasks queued for rendering.
        /// </summary>
        private readonly List<IDrawTask> DrawTasks = new List<IDrawTask>();
        private void OnGUI()
        {
            for (int i = 0; i < DrawTasks.Count; i++)
            {
                IDrawTask task = DrawTasks[i];

                if (task == null || task.IsExpired())
                {
                    if (task != null)
                        task.Deactive();
                    DrawTasks.RemoveAt(i);
                    continue;
                }

                if (task.GetDrawMode() != DrawMode.GUI)
                    continue;

                if (task.CanDraw(i))
                    task.Draw(i);
            }
        }
        private void OnDrawGizmos()
        {
            for (int i = 0; i < DrawTasks.Count; i++)
            {
                IDrawTask task = DrawTasks[i];

                if (task == null || task.IsExpired())
                {
                    if (task != null)
                        task.Deactive();
                    DrawTasks.RemoveAt(i);
                    continue;
                }

                if (task.GetDrawMode() != DrawMode.Gizmo)
                    continue;

                if (task.CanDraw(i))
                    task.Draw(i);
            }
        }
#endif



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