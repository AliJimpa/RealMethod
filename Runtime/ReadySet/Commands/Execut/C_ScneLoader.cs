using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Command/ScneLoader")]
    public sealed class C_ScneLoader : Command
    {
        private enum LoadMethod
        {
            SceneIndex = 0,
            SceneName = 1,
            SceneAsset = 2,
            WorldAsset = 3,
        }
        [Header("Assets")]
        [SerializeField]
        private LoadMethod method = LoadMethod.SceneAsset;
        [SerializeField, ShowInInspectorByEnum("method", 0)]
        private int sceneIndex;
        [SerializeField, ShowInInspectorByEnum("method", 1)]
        private string sceneName;
        [SerializeField, ShowInInspectorByEnum("method", 2)]
        private SceneReference sceneAsset;
        [SerializeField, ShowInInspectorByEnum("method", 3)]
        private WorldSceneConfig worldAsset;
        [Header("Setting")]
        [SerializeField, ShowInInspectorByEnum("method", 0, 1)]
        private bool IsAsync = false;
        [SerializeField, ShowInInspectorByEnum("method", 0, 1)]
        private LoadSceneMode LoadType = LoadSceneMode.Single;

        public Action OnAsyncSceneLoaded;



        // ExecutCommand Methods
        protected override bool OnInitiate(UnityEngine.Object author, UnityEngine.Object owner)
        {
            return true;
        }
        protected override bool CanExecute(object Owner)
        {
            return enabled;
        }
        protected override void Execute(object Owner)
        {
            switch (method)
            {
                case LoadMethod.SceneIndex:
                    if (IsAsync)
                    {
                        SceneManager.LoadScene(sceneIndex, LoadType);
                    }
                    else
                    {
                        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneIndex, LoadType);
                        asyncOp.completed += (op) =>
                        {
                            OnAsyncSceneLoaded?.Invoke();
                        };
                    }
                    break;
                case LoadMethod.SceneName:
                    if (IsAsync)
                    {
                        SceneManager.LoadScene(sceneName, LoadType);
                    }
                    else
                    {
                        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadType);
                        asyncOp.completed += (op) =>
                        {
                            OnAsyncSceneLoaded?.Invoke();
                        };
                    }
                    SceneManager.LoadScene(sceneName, LoadType);
                    break;
                case LoadMethod.SceneAsset:
                    Game.OpenScene(sceneAsset);
                    break;
                case LoadMethod.WorldAsset:
                    Game.OpenWorld(worldAsset);
                    break;
                default:
                    Debug.LogError("Unkown Method for Loading Scene");
                    break;
            }
        }


        // Public Function
        public void LoadScene()
        {
            Execute(null);
        }


    }
}