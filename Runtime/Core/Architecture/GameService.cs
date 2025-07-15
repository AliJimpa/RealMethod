using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RealMethod
{
    public interface IWorldSync
    {
        bool IntroduceWorld(World world);
    }

    public abstract class GameService : Service, IWorldSync
    {
        // Game Structure
        public Action<World> OnWorldUpdate;
        public Action<World> OnAdditiveWorld;
        public Action<Service> OnServiceCreate;
        // Load Scene 
        public Action<bool> OnSceneLoading;
        public Action<float> OnSceneLoadingProcess;
        public bool IsLoading { get; protected set; }
        public float FadeTime = 0;


        private SceneReference CurrentScne;
        private WorldSceneConfig CurrentWorld;


        // Implement IWorldSync Interface
        // Any World in Awake time call this method
        bool IWorldSync.IntroduceWorld(World world)
        {
            if (Game.World == null)
            {
                OnWorldUpdate?.Invoke(world);
                return true;
            }
            else
            {
                OnNewAdditiveWorld(world);
                OnAdditiveWorld?.Invoke(world);
                return false;
            }
        }


        // Virtual Methods
        public virtual IEnumerator GetLoadScneCorotine(SceneReference TargetScene)
        {
            if (IsLoading == true)
            {
                Debug.LogWarning($"Can't load {TargetScene} The Service is in Loading");
                return null;
            }
            CurrentScne = TargetScene;
            CurrentWorld = null;
            return LoadSceneAsync(TargetScene);
        }
        public virtual IEnumerator GetLoadWorldCorotine(WorldSceneConfig WorldScene)
        {
            if (IsLoading == true)
            {
                Debug.LogWarning($"Can't load {WorldScene} The Service is in Loading");
                return null;
            }
            CurrentScne = null;
            CurrentWorld = WorldScene;
            return LoadWorldAsync(WorldScene);
        }
        public virtual IEnumerator GetReloadSWCorotine()
        {
            if (IsLoading == true)
            {
                Debug.LogWarning($"Can't load CurrentScne/World The Service is in Loading");
                return null;
            }
            if (CurrentScne != null)
            {
                return LoadSceneAsync(CurrentScne);
            }
            if (CurrentWorld != null)
            {
                return LoadWorldAsync(CurrentWorld);
            }
            return null;
        }


        // Abstract Methods
        protected abstract void OnNewAdditiveWorld(World target);

        // Corotine
        private IEnumerator LoadSceneAsync(SceneReference scene)
        {
            //StartLoading
            IsLoading = true;
            OnSceneLoading?.Invoke(true);

            //Fading Screen
            if (FadeTime != 0)
            {
                yield return new WaitForSeconds(FadeTime);
            }

            //Loading Scene
            AsyncOperation Load_opertation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
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
            if (FadeTime != 0)
            {
                yield return new WaitForSeconds(FadeTime);
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

            //Fading Screen
            if (FadeTime != 0)
            {
                yield return new WaitForSeconds(FadeTime);
            }


            //load Persistance Levels
            AsyncOperation Load_opertation = SceneManager.LoadSceneAsync(WS.GetPersistent(), LoadSceneMode.Single);
            if (Load_opertation == null)
            {
                Debug.LogError("Failed to load scene. AsyncOperation is null.");
                OnSceneLoading?.Invoke(false);
                IsLoading = false;
                yield break;
            }
            while (!Load_opertation.isDone)
            {
                OnSceneLoadingProcess?.Invoke(RM_Math.MapRangedClamp(Load_opertation.progress, 0, 1, 0, (1 / WS.AdditiveCount + 1)));
                OnSceneLoading?.Invoke(false);
                IsLoading = false;
                yield return null;
            }

            // Load Additive Levels
            for (int i = 0; i < WS.AdditiveCount; i++)
            {
                AsyncOperation Additive_Load_opertation = SceneManager.LoadSceneAsync(WS.GetAdditive(i), LoadSceneMode.Additive);
                if (Additive_Load_opertation == null)
                {
                    Debug.LogError("Failed to load scene. AsyncOperation is null.");
                    OnSceneLoading?.Invoke(false);
                    IsLoading = false;
                    yield break;
                }
                while (!Additive_Load_opertation.isDone)
                {
                    OnSceneLoadingProcess?.Invoke(RM_Math.MapRangedClamp(Load_opertation.progress, 0, 1, 0, (1 / WS.AdditiveCount + 1 - (i + 1))));
                    yield return null;
                }
            }
            OnSceneLoadingProcess?.Invoke(1);

            //Fading Screen
            if (FadeTime != 0)
            {
                yield return new WaitForSeconds(FadeTime);
            }

            //FinishLoading
            OnSceneLoading?.Invoke(false);
            IsLoading = false;
        }
    }
}