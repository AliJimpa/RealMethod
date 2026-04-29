using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Codice.Client.Common;
using System.Runtime.CompilerServices;

namespace RealMethod
{

    /// <summary>
    /// Defines synchronization hooks used internally to coordinate
    /// worlds and Game during runtime initialization.
    /// </summary>
    public interface IRelationBridge
    {
        /// <summary>
        /// This represents the persistent scene
        /// </summary>
        Scene InstanceScene { get; }
        /// <summary>
        /// Introduces a newly created world to the system.
        /// </summary>
        /// <param name="world">The world instance being introduced.</param>
        /// <returns>
        /// True if the world is treated as the main world;
        /// false if it is considered a side/additive world.
        /// </returns>
        bool RegisterWorld(World world);
        /// <summary>
        /// Binds a callback invoked when the main world is added.
        /// </summary>
        void BindWorldCreated(Action<World> func);
        /// <summary>
        /// Unbinds the main world added callback.
        /// </summary>
        void UnbindWorldCreated();
        /// <summary>
        /// Called by world class to tell game the initiation is complite
        /// </summary>
        void WorldIsReady();
        /// <summary>
        /// Event invoked by World class to tell the realmethod initiation complite
        /// Access by Game class
        /// </summary>
        event Action OnGameReady;
    }
    /// <summary>
    /// Defines all events and stat need to know for loading scnes
    /// to make internal connection from Game to unify all game events from Game class
    /// </summary>
    public interface ILoadScneBridge
    {
        /// <summary>
        /// Event invoked when a scene or world starts or finishes loading.
        /// The boolean parameter is true when loading starts and false when loading ends.
        /// </summary>
        event Action<bool> OnSceneLoading;
        /// <summary>
        /// Event invoked during scene or world loading to report progress.
        /// The float parameter represents the loading progress from 0 (start) to 1 (complete).
        /// </summary>
        event Action<float> OnSceneLoadingProcess;
        /// <summary>
        /// Indicates whether a scene or world load operation is currently in progress.
        /// </summary>
        bool IsLoading { get; }
    }


    /// <summary>
    /// Base class for any gamebridge in the framework.
    /// Provides core functionality for:
    /// <list type="bullet">
    /// <item>World synchronization (main and additive worlds)</item>
    /// <item>Service lifecycle notifications (created/removed)</item>
    /// <item>world check for single remove new version</item>
    /// <item>Load & Unload Scene reporting</item>
    /// </list>
    /// Inherits from <see cref="Service"/> and implements <see cref="IRelationBridge"/>
    /// to integrate with the game's internal world and service management system.
    /// </summary>
    public abstract class GameBridge : IService, IRelationBridge, ILoadScneBridge
    {
        // Events
        private Action GameReadyEvent;
        private Action<World> NewWorldEvent;
        private Action<bool> SceneLoadingEvent;
        private Action<float> SceneLoadingProcessEvent;
        private bool isLoading;
        public bool IsHolding { get; private set; } = false;
        protected float FadeTime = 0;
        private Scene CurrrentScene;
        public static Scene LastActiveScene;
        public static bool SceneWillUnload { get; private set; }

        // Implement IService Interface
        void IService.OnRegister(object author)
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }
        void IService.OnWorldChanging(World Previous, World New)
        {
            throw new NotImplementedException();
        }
        void IService.OnUnregister(object author)
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }
#if UNITY_EDITOR
        string IService.GetInspectorInfo()
        {
            return $"IsLoading:{isLoading} - FadeTime:{FadeTime}";
        }
#endif

        // Implement IRelationBridge Interface
        event Action IRelationBridge.OnGameReady
        {
            add
            {
                GameReadyEvent += value;
            }

            remove
            {
                GameReadyEvent -= value;
            }
        }
        Scene IRelationBridge.InstanceScene => CurrrentScene;
        bool IRelationBridge.RegisterWorld(World world)
        {
            return TryToRegisterWorld(world);
        }
        void IRelationBridge.BindWorldCreated(Action<World> func)
        {
            if (NewWorldEvent != null)
            {
                Debug.LogWarning("BindMainWorldAdd is already binded this interface is internal didnt use in another script or your game");
                return;
            }
            NewWorldEvent = func;
        }
        void IRelationBridge.UnbindWorldCreated()
        {
            NewWorldEvent = null;
        }
        void IRelationBridge.WorldIsReady()
        {
            GameReadyEvent?.Invoke();
        }

        // Implement ILoadScneBridge Interface
        event Action<bool> ILoadScneBridge.OnSceneLoading
        {
            add
            {
                SceneLoadingEvent += value;
            }

            remove
            {
                SceneLoadingEvent -= value;
            }
        }
        event Action<float> ILoadScneBridge.OnSceneLoadingProcess
        {
            add
            {
                SceneLoadingProcessEvent += value;
            }

            remove
            {
                SceneLoadingProcessEvent -= value;
            }
        }
        bool ILoadScneBridge.IsLoading => isLoading;


        /// <summary>
        /// Call this when you want to define new World class to game 
        /// </summary>
        /// <param name="world">The world instance will set as main world for game.</param>
        protected void SetMianWorld(World world)
        {
            CurrrentScene = world.gameObject.scene;
            NewWorldEvent?.Invoke(world);
            SceneWillUnload = false;
        }
        /// <summary>
        /// Call this when you want to define new World class that created and should not be main world (probably deleted)
        /// </summary>
        /// <param name="world">The world instance will not set to main world for game.</param>
        protected void SetAdditiveWorld(World world)
        {
            world.enabled = false;
            OnAdditiveWorldDetected(world);
        }





        /// <summary>
        /// Request for this NewWorld to set for Main World in Scene
        /// </summary>
        /// <param name="NewWorld">The New WorldClass Refrence in Scene
        /// <returns>
        /// If this request is valid return true that mean this world set as main world.
        /// If this request false means this world import from scen that is additive and should deactive.
        /// </returns
        protected virtual bool TryToRegisterWorld(World NewWorld)
        {
            Scene NewScene = NewWorld.gameObject.scene;

            // If no instance yet, this becomes the persistent instance
            if (Game.World == null)
            {
                SetMianWorld(NewWorld);
                return true;
            }

            // This is a new scene loaded in Single mode
            if (SceneWillUnload && NewScene == LastActiveScene)
            {
                // Replace instance because old scene is going away immediately
                SetMianWorld(NewWorld);
                return true;
            }

            // Different scene but NOT replacing old => Additive load
            if (NewScene != CurrrentScene)
            {
                SetAdditiveWorld(NewWorld);
                return false; ;
            }
            else
            {
                Debug.LogError($"[WorldRegistry] Duplicate world in SAME scene : {NewWorld.gameObject.name} / {Game.World.gameObject}");
                return false;
            }
        }
        /// <summary>
        /// Called when new scene loaded and during base world valid new world created.
        /// </summary>
        /// <param name="AdditiveWorld">The New WorldClass Refrence in AdditiveScene</param>
        protected virtual void OnAdditiveWorldDetected(World AdditiveWorld)
        {
            UnityEngine.Object.Destroy(AdditiveWorld.gameObject);
        }
        /// <summary>
        /// Starts loading a scene by name using a coroutine.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <returns>
        /// An IEnumerator coroutine for loading the scene,
        /// or null if a load operation is already in progress.
        /// </returns>
        public virtual IEnumerator GetLoadScneCorotine(string sceneName)
        {
            if (isLoading == true)
            {
                Debug.LogWarning($"Can't load Scene:{sceneName} The Bridge is in loading target scene");
                return null;
            }
            return LoadSceneAsync(sceneName);
        }
        /// <summary>
        /// Starts Adding a scene by name using a coroutine.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <returns>
        /// An IEnumerator coroutine for Adding the scene,
        /// or null if a load operation is already in progress.
        /// </returns>
        public virtual IEnumerator GetAddScneCorotine(string sceneName, Action callback)
        {
            if (isLoading == true)
            {
                Debug.LogWarning($"Can't load Scene:{sceneName} The Bridge is in loading target scene");
                return null;
            }
            return AddSceneAsync(callback, sceneName);
        }
        /// <summary>
        /// Starts loading a scene by build index using a coroutine.
        /// </summary>
        /// <param name="sceneIndex">The build index of the scene to load.</param>
        /// <returns>
        /// An IEnumerator coroutine for loading the scene,
        /// or null if a load operation is already in progress.
        /// </returns>
        public virtual IEnumerator GetLoadScneCorotine(int sceneIndex)
        {
            if (isLoading == true)
            {
                Debug.LogWarning($"Can't load Index:{sceneIndex} The Bridge is in loading target scene");
                return null;
            }
            return LoadSceneAsync(string.Empty, sceneIndex);
        }
        /// <summary>
        /// Starts Adding a scene by build index using a coroutine.
        /// </summary>
        /// <param name="sceneIndex">The build index of the scene to load.</param>
        /// <returns>
        /// An IEnumerator coroutine for Adding the scene,
        /// or null if a load operation is already in progress.
        /// </returns>
        public virtual IEnumerator GetAddScneCorotine(int sceneIndex, Action callback)
        {
            if (isLoading == true)
            {
                Debug.LogWarning($"Can't load Index:{sceneIndex} The Bridge is in loading target scene");
                return null;
            }
            return AddSceneAsync(callback, string.Empty, sceneIndex);
        }
        /// <summary>
        /// Starts loading a world configuration using a coroutine.
        /// </summary>
        /// <param name="WorldScene">The worldAsset to load.</param>
        /// <returns>
        /// An IEnumerator coroutine for loading the world,
        /// or null if a load operation is already in progress.
        /// </returns>
        public virtual IEnumerator GetLoadWorldCorotine(WorldAsset WorldScene)
        {
            if (isLoading == true)
            {
                Debug.LogWarning($"Can't load World:{WorldScene} The Bridge is in loading target scene");
                return null;
            }
            return LoadWorldAsync(WorldScene);
        }


        private static void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            LastActiveScene = newScene;
            SceneWillUnload = true;   // old scene will be destroyed next frame
        }
        private float RemapClamped(float value, float inMin, float inMax, float outMin, float outMax)
        {
            // Prevent divide by zero
            if (Mathf.Approximately(inMax, inMin))
            {
                Debug.LogWarning("Input range is zero. Returning outMin.");
                return outMin;
            }

            // Normalize the input value to 0–1 within the input range
            float t = (value - inMin) / (inMax - inMin);

            // Scale and offset to target range
            float mappedValue = t * (outMax - outMin) + outMin;

            // Clamp result to the output range
            return Mathf.Clamp(mappedValue, Mathf.Min(outMin, outMax), Mathf.Max(outMin, outMax));
        }



        // Corotine
        private IEnumerator LoadSceneAsync(string scene, int scneIndex = -1)
        {
            //StartLoading
            isLoading = true;
            SceneLoadingEvent?.Invoke(true);
            float fadingtime = FadeTime;

            //Fading Screen
            if (fadingtime != 0)
            {
                yield return new WaitForSeconds(fadingtime);
            }

            //Loading Scene
            AsyncOperation Load_opertation;
            if (scneIndex > -1)
            {
                Load_opertation = SceneManager.LoadSceneAsync(scneIndex, LoadSceneMode.Single);
            }
            else
            {
                Load_opertation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
            }
            if (Load_opertation == null)
            {
                Debug.LogError("Failed to load scene. AsyncOperation is null.");
                SceneLoadingEvent?.Invoke(false);
                isLoading = false;
                yield break;
            }
            while (!Load_opertation.isDone)
            {
                SceneLoadingProcessEvent?.Invoke(Load_opertation.progress);
                yield return null;
            }
            SceneLoadingProcessEvent?.Invoke(1);

            //Fading Screen
            if (fadingtime != 0)
            {
                yield return new WaitForSeconds(fadingtime);
            }

            //FinishLoading
            SceneLoadingEvent?.Invoke(false);
            isLoading = false;
        }
        private IEnumerator AddSceneAsync(Action callback, string scene, int scneIndex = -1)
        {
            //Loading Scene
            AsyncOperation Load_opertation;
            if (scneIndex > -1)
            {
                Load_opertation = SceneManager.LoadSceneAsync(scneIndex, LoadSceneMode.Additive);
            }
            else
            {
                Load_opertation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            }
            if (Load_opertation == null)
            {
                Debug.LogError("Failed to Add scene. AsyncOperation is null.");
                yield break;
            }
            while (!Load_opertation.isDone)
            {
                // Processing
                yield return null;
            }
            // Process = 1;
            callback?.Invoke();
        }
        private IEnumerator LoadWorldAsync(WorldAsset WS)
        {
            //StartLoading
            isLoading = true;
            SceneLoadingEvent?.Invoke(true);
            float fadingtime = FadeTime;

            //Fading Screen
            if (fadingtime != 0)
            {
                yield return new WaitForSeconds(fadingtime);
            }


            //load Persistance Levels
            AsyncOperation Load_opertation = SceneManager.LoadSceneAsync(WS.Persistent, LoadSceneMode.Single);
            if (Load_opertation == null)
            {
                Debug.LogError("Failed to load scene. AsyncOperation is null.");
                SceneLoadingEvent?.Invoke(false);
                isLoading = false;
                yield break;
            }
            while (!Load_opertation.isDone)
            {
                SceneLoadingProcessEvent?.Invoke(RemapClamped(Load_opertation.progress, 0, 1, 0, 1 / WS.Count + 1));
                SceneLoadingEvent?.Invoke(false);
                isLoading = false;
                yield return null;
            }

            // Load Additive Levels
            for (int i = 0; i < WS.Count; i++)
            {
                AsyncOperation Additive_Load_opertation = SceneManager.LoadSceneAsync(WS[i], LoadSceneMode.Additive);
                if (Additive_Load_opertation == null)
                {
                    Debug.LogError("Failed to load scene. AsyncOperation is null.");
                    SceneLoadingEvent?.Invoke(false);
                    isLoading = false;
                    yield break;
                }
                while (!Additive_Load_opertation.isDone)
                {
                    SceneLoadingProcessEvent?.Invoke(RemapClamped(Load_opertation.progress, 0, 1, 0, 1 / WS.Count + 1 - (i + 1)));
                    yield return null;
                }
            }
            SceneLoadingProcessEvent?.Invoke(1);

            //Fading Screen
            if (fadingtime != 0)
            {
                yield return new WaitForSeconds(fadingtime);
            }

            //FinishLoading
            SceneLoadingEvent?.Invoke(false);
            isLoading = false;
        }


    }
}