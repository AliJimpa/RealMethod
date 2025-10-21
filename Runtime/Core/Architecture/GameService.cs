using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RealMethod
{
    public interface IWorldSync
    {
        bool IntroduceWorld(World world);
    }

    public abstract class GameService : Service, IWorldSync
    {
        public IWorldSync Worldprovider => this;
        // Game Structure
        public Action<World> OnWorldUpdate;
        public Action<World> OnAdditiveWorld;
        public Action<Service> OnServiceCreated;
        public Action<Service> OnServiceRemoved;
        // Load Scene 
        public Action<bool> OnSceneLoading;
        public Action<float> OnSceneLoadingProcess;
        public bool IsLoading { get; protected set; }


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

        // public Methods
        public virtual IEnumerator GetLoadScneCorotine(string sceneName)
        {
            if (IsLoading == true)
            {
                Debug.LogWarning($"Can't load Scene:{sceneName} The Service is in Loading");
                return null;
            }
            return LoadSceneAsync(sceneName);
        }
        public virtual IEnumerator GetLoadScneCorotine(int sceneIndex)
        {
            if (IsLoading == true)
            {
                Debug.LogWarning($"Can't load Index:{sceneIndex} The Service is in Loading");
                return null;
            }
            return LoadSceneAsync(string.Empty, sceneIndex);
        }
        public virtual IEnumerator GetLoadWorldCorotine(WorldSceneConfig WorldScene)
        {
            if (IsLoading == true)
            {
                Debug.LogWarning($"Can't load World:{WorldScene} The Service is in Loading");
                return null;
            }
            return LoadWorldAsync(WorldScene);
        }

        // Abstract Methods
        protected abstract void OnNewAdditiveWorld(World target);

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
                OnSceneLoadingProcess?.Invoke(RM_Math.Map.RemapClamped(Load_opertation.progress, 0, 1, 0, 1 / WS.Count + 1));
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
                    OnSceneLoadingProcess?.Invoke(RM_Math.Map.RemapClamped(Load_opertation.progress, 0, 1, 0, 1 / WS.Count + 1 - (i + 1)));
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


#if UNITY_EDITOR
        // Runs when entering Play Mode in Editor
        [InitializeOnEnterPlayMode]
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