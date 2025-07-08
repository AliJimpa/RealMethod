using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RealMethod
{
    public abstract class GameService : Service
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

        // Any World in Awake time call this method
        public bool IntroduceWorld(World world)
        {
            if (Game.World == null)
            {
                OnWorldUpdate?.Invoke(world);
                NewWorld(world);
                return true;
            }
            else
            {
                OnAdditiveWorld?.Invoke(world);
                NewAdditiveWorld(world);
                return false;
            }
        }

        // Virtual Methods
        public virtual IEnumerator GetLoadScneCorotine(SceneReference TargetScene)
        {
            return LoadSceneAsync(TargetScene);
        }
        public virtual IEnumerator GetLoadWorldCorotine(WorldSceneAsset WorldScene)
        {
            return LoadWorldAsync(WorldScene);
        }

        // Abstract Methods
        protected abstract void NewWorld(World target);
        protected abstract void NewAdditiveWorld(World target);

        // Corotine
        private IEnumerator LoadSceneAsync(SceneReference scene)
        {
            if (IsLoading == true)
            {
                Debug.LogWarning($"Can't load {scene} The Service is in Loadin");
                yield break;
            }

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
            IsLoading = false;
            OnSceneLoading?.Invoke(false);
        }
        private IEnumerator LoadWorldAsync(WorldSceneAsset WS)
        {
            if (IsLoading == true)
            {
                Debug.LogWarning($"Can't load {WS} The Service is in Loadin");
                yield break;
            }

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
                yield break;
            }
            while (!Load_opertation.isDone)
            {
                OnSceneLoadingProcess?.Invoke(RM_Math.MapRangedClamp(Load_opertation.progress, 0, 1, 0, (1 / WS.GetAdditiveCount() + 1)));
                yield return null;
            }

            // Load Additive Levels
            for (int i = 0; i < WS.GetAdditiveCount(); i++)
            {
                AsyncOperation Additive_Load_opertation = SceneManager.LoadSceneAsync(WS.GetAdditive(i), LoadSceneMode.Additive);
                if (Additive_Load_opertation == null)
                {
                    Debug.LogError("Failed to load scene. AsyncOperation is null.");
                    yield break;
                }
                while (!Additive_Load_opertation.isDone)
                {
                    OnSceneLoadingProcess?.Invoke(RM_Math.MapRangedClamp(Load_opertation.progress, 0, 1, 0, (1 / WS.GetAdditiveCount() + 1 - (i + 1))));
                    yield return null;
                }

            }

            //Fading Screen
            if (FadeTime != 0)
            {
                yield return new WaitForSeconds(FadeTime);
            }

            //FinishLoading
            IsLoading = false;
            OnSceneLoading?.Invoke(false);
        }

    }
}