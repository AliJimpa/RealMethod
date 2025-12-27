using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RealMethod
{
    /// <summary>
    /// Defines synchronization hooks used internally to coordinate
    /// worlds and services during runtime initialization.
    /// </summary>
    public interface IMethodSync
    {
        /// <summary>
        /// Introduces a newly created world to the system.
        /// </summary>
        /// <param name="world">The world instance being introduced.</param>
        /// <returns>
        /// True if the world is treated as the main world;
        /// false if it is considered a side/additive world.
        /// </returns>
        bool IntroduceWorld(World world);
        /// <summary>
        /// Binds a callback invoked when the main world is added.
        /// </summary>
        void BindMainWorldAdd(Action<World> func);
        /// <summary>
        /// Unbinds the main world added callback.
        /// </summary>
        void UnbindMainWorldAdd();
        /// <summary>
        /// Binds a callback invoked when a side (additive) world is added.
        /// </summary>
        void BindSideWorldAdd(Action<World> func);
        /// <summary>
        /// Unbinds the side world added callback.
        /// </summary>
        void UnbindSideWorldAdd();
        /// <summary>
        /// Notifies the system that a service has been created.
        /// </summary>
        void ServiceCreated(Service service);
        /// <summary>
        /// Notifies the system that a service has been removed.
        /// </summary>
        void ServiceRemoved(Service service);
        /// <summary>
        /// Binds a callback invoked when services are added or removed.
        /// </summary>
        /// <param name="func">
        /// Callback receiving the service instance and its active state.
        /// </param>
        void BindServicesUpdated(Action<Service, bool> func);
        /// <summary>
        /// Unbinds the service update callback.
        /// </summary>
        void UnbindServicesUpdated();
    }


    /// <summary>
    /// Base class for any game services in the framework.
    /// Provides core functionality for:
    /// <list type="bullet">
    /// <item>World synchronization (main and additive worlds)</item>
    /// <item>Service lifecycle notifications (created/removed)</item>
    /// <item>Scene and world loading with progress reporting</item>
    /// </list>
    /// Inherits from <see cref="Service"/> and implements <see cref="IMethodSync"/>
    /// to integrate with the game's internal world and service management system.
    /// </summary>
    public abstract class GameService : Service, IMethodSync
    {
        /// <summary>
        /// Event invoked when a scene or world starts or finishes loading.
        /// The boolean parameter is true when loading starts and false when loading ends.
        /// </summary>
        public event Action<bool> OnSceneLoading;
        /// <summary>
        /// Event invoked during scene or world loading to report progress.
        /// The float parameter represents the loading progress from 0 (start) to 1 (complete).
        /// </summary>
        public event Action<float> OnSceneLoadingProcess;
        /// <summary>
        /// Indicates whether a scene or world load operation is currently in progress.
        /// </summary>
        public bool IsLoading { get; protected set; }

        // Game Structure
        private Action<World> MainWorldEvent;
        private Action<World> SideWorldEvent;
        private Action<Service, bool> ServiceEvents;



        // Implement IMethodSync Interface
        bool IMethodSync.IntroduceWorld(World world)
        {
            if (Game.World == null)
            {
                MainWorldEvent?.Invoke(world);
                return true;
            }
            else
            {
                SideWorldEvent?.Invoke(world);
                return false;
            }
        }
        void IMethodSync.BindMainWorldAdd(Action<World> func)
        {
            if (MainWorldEvent != null)
            {
                Debug.LogWarning("BindMainWorldAdd is already binded this interface is internal didnt use in another script or your game");
                return;
            }
            MainWorldEvent = func;
        }
        void IMethodSync.UnbindMainWorldAdd()
        {
            MainWorldEvent = null;
        }
        void IMethodSync.BindSideWorldAdd(Action<World> func)
        {
            if (SideWorldEvent != null)
            {
                Debug.LogWarning("BindSideWorldAdd is already binded this interface is internal didnt use in another script or your game");
                return;
            }
            SideWorldEvent = func;
        }
        void IMethodSync.UnbindSideWorldAdd()
        {
            SideWorldEvent = null;
        }
        void IMethodSync.ServiceCreated(Service service)
        {
            ServiceEvents.Invoke(service, true);
        }
        void IMethodSync.ServiceRemoved(Service service)
        {
            ServiceEvents.Invoke(service, false);
        }
        void IMethodSync.BindServicesUpdated(Action<Service, bool> func)
        {
            if (ServiceEvents != null)
            {
                Debug.LogWarning("BindServicesUpdated is already binded this interface is internal didnt use in another script or your game");
                return;
            }
            ServiceEvents = func;
        }
        void IMethodSync.UnbindServicesUpdated()
        {
            ServiceEvents = null;
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
            if (IsLoading == true)
            {
                Debug.LogWarning($"Can't load Scene:{sceneName} The Service is in Loading");
                return null;
            }
            return LoadSceneAsync(sceneName);
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
            if (IsLoading == true)
            {
                Debug.LogWarning($"Can't load Index:{sceneIndex} The Service is in Loading");
                return null;
            }
            return LoadSceneAsync(string.Empty, sceneIndex);
        }
        /// <summary>
        /// Starts loading a world configuration using a coroutine.
        /// </summary>
        /// <param name="WorldScene">The world scene configuration to load.</param>
        /// <returns>
        /// An IEnumerator coroutine for loading the world,
        /// or null if a load operation is already in progress.
        /// </returns>
        public virtual IEnumerator GetLoadWorldCorotine(WorldSceneConfig WorldScene)
        {
            if (IsLoading == true)
            {
                Debug.LogWarning($"Can't load World:{WorldScene} The Service is in Loading");
                return null;
            }
            return LoadWorldAsync(WorldScene);
        }


        // Corotine
        private IEnumerator LoadSceneAsync(string scene, int scneIndex = -1)
        {
            //StartLoading
            IsLoading = true;
            OnSceneLoading?.Invoke(true);
            float fadingtime = Game.Config.FadeTime;

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
                OnSceneLoading?.Invoke(false);
                IsLoading = false;
                yield break;
            }
            while (!Load_opertation.isDone)
            {
                OnSceneLoadingProcess?.Invoke(Load_opertation.progress);
                yield return null;
            }
            OnSceneLoadingProcess?.Invoke(1);

            //Fading Screen
            if (fadingtime != 0)
            {
                yield return new WaitForSeconds(fadingtime);
            }

            //FinishLoading
            OnSceneLoading?.Invoke(false);
            IsLoading = false;
        }
        private IEnumerator LoadWorldAsync(WorldSceneConfig WS)
        {
            //StartLoading
            IsLoading = true;
            OnSceneLoading?.Invoke(true);
            float fadingtime = Game.Config.FadeTime;

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
                OnSceneLoading?.Invoke(false);
                IsLoading = false;
                yield break;
            }
            while (!Load_opertation.isDone)
            {
                OnSceneLoadingProcess?.Invoke(Clamped(Load_opertation.progress, 0, 1, 0, 1 / WS.Count + 1));
                OnSceneLoading?.Invoke(false);
                IsLoading = false;
                yield return null;
            }

            // Load Additive Levels
            for (int i = 0; i < WS.Count; i++)
            {
                AsyncOperation Additive_Load_opertation = SceneManager.LoadSceneAsync(WS[i], LoadSceneMode.Additive);
                if (Additive_Load_opertation == null)
                {
                    Debug.LogError("Failed to load scene. AsyncOperation is null.");
                    OnSceneLoading?.Invoke(false);
                    IsLoading = false;
                    yield break;
                }
                while (!Additive_Load_opertation.isDone)
                {
                    OnSceneLoadingProcess?.Invoke(Clamped(Load_opertation.progress, 0, 1, 0, 1 / WS.Count + 1 - (i + 1)));
                    yield return null;
                }
            }
            OnSceneLoadingProcess?.Invoke(1);

            //Fading Screen
            if (fadingtime != 0)
            {
                yield return new WaitForSeconds(fadingtime);
            }

            //FinishLoading
            OnSceneLoading?.Invoke(false);
            IsLoading = false;
        }


        // Private Functions
        private float Clamped(float value, float inMin, float inMax, float outMin, float outMax)
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



#if UNITY_EDITOR
        [InitializeOnEnterPlayMode] // Runs when entering Play Mode in Editor
        private static void EditorPlayModeInit()
        {
            var assets = Resources.FindObjectsOfTypeAll<PrimitiveAsset>();
            foreach (var asset in assets)
            {
                if (asset.IsProjectAsset())
                {
                    asset.OnEditorPlay();
                }
            }
        }
#endif


    }
}