using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    /// A bridge that receives notifications before and after the global world changes.
    /// </summary>
    public interface IBridge
    {
        /// <summary>
        /// Called after the new world has been set as the active global world.
        /// </summary>
        void OnWorldChanged(World world);
    }


    /// <summary>
    /// Base class for any gamebridge in the framework.
    /// Provides core functionality for:
    /// <list type="bullet">
    /// <item>World synchronization (main and additive worlds)</item>
    /// <item>world check for single remove new version</item>
    /// <item>Load & Unload Scene reporting</item>
    /// </list>
    /// </summary>
    public abstract class GameBridge : IDisposable, IRelationBridge, ILoadScneBridge, IInspectorInfo
    {
        // Events
        private Action GameReadyEvent;

        private Action<bool> SceneLoadingEvent;
        private Action<float> SceneLoadingProcessEvent;
        private bool isLoading;
        public bool IsHolding { get; private set; } = false;
        protected float FadeTime = 0;
        private Scene CurrrentScene;
        public Scene LastActiveScene;
        public bool SceneWillUnload { get; private set; }

        private readonly Action<World> OnNextWorld;
        private readonly List<WeakReference<IBridge>> bridges = new();



        public GameBridge(Action<World> method)
        {
            OnNextWorld = method ?? throw new ArgumentNullException(nameof(method));
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }



        // Implement IDisposable Interface
        void IDisposable.Dispose()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }
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
        /// Registers a bridge provider using a weak reference.  
        /// Prevents duplicate bindings and ignores null providers.
        /// </summary>
        /// <param name="provider">The bridge instance to bind.</param>
        /// <exception cref="ArgumentNullException">Thrown when provider is null.</exception>
        public void Bind(IBridge provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            // Prevent duplicates
            foreach (var weak in bridges)
            {
                if (weak.TryGetTarget(out IBridge existing) && existing == provider)
                    return; // Already stored
            }

            bridges.Add(new WeakReference<IBridge>(provider));
        }
        /// <summary>
        /// Removes the specified bridge instance from the registry.
        /// </summary>
        /// <param name="obj">The object to unbind (must implement IBridge).</param>
        /// <returns>
        /// True if the bridge was found and removed; otherwise false.
        /// </returns>
        public bool Unbind(object obj)
        {
            if (obj is not IBridge provider)
                return false;

            for (int i = bridges.Count - 1; i >= 0; i--) // reverse-safe removal
            {
                var weak = bridges[i];

                if (!weak.TryGetTarget(out IBridge bridge) || bridge == provider)
                {
                    bridges.RemoveAt(i);
                    return true;
                }
            }

            return false;
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






        /// <summary>
        /// Call this when you want to define new World class to game 
        /// </summary>
        /// <param name="world">The world instance will set as main world for game.</param>
        protected void SetMianWorld(World world)
        {
            CurrrentScene = world.gameObject.scene;
            OnNextWorld.Invoke(world);
            NotifyBridges(world);
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




        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            LastActiveScene = newScene;
            SceneWillUnload = true;   // old scene will be destroyed next frame
        }
        /// <summary>
        /// Calls the bridge method on all active bridge instances.
        /// Invalid or collected references are removed automatically.
        /// </summary>
        private void NotifyBridges(World Newworld)
        {
            for (int i = bridges.Count - 1; i >= 0; i--)
            {
                if (bridges[i].TryGetTarget(out IBridge bridge))
                {
                    bridge.OnWorldChanged(Newworld);
                }
                else
                {
                    bridges.RemoveAt(i);
                }
            }
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





#if UNITY_EDITOR
        // Implement IInspectorInfo Interface
        string IInspectorInfo.GetInfo()
        {
            return $"IsLoading ({isLoading}) , FadeTime ({FadeTime}) , {GetInspectorInfo()}";
        }
        protected virtual string GetInspectorInfo()
        {
            return null;
        }
#endif



    }
}