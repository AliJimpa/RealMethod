/// <summary>
/// The Game class is a singleton that manages the core game logic and state.
/// It provides methods for managing game objects, scenes, and services, as well as handling world updates.
/// Note: Ensure that only one instance of this class is active in the scene.
/// VeryImportant: Dont Use RequireComponent in This class.
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RealMethod
{
    /// <remarks>
    /// The Game class is a singleton that manages the core game logic and state.
    /// Note: Dont Use RequireComponent.
    /// </remarks>
    public abstract class Game : MonoBehaviour
    {
        //Static Variables
        private static Game AlternativeInstance;
        public static Game Instance
        {
            get
            {
                if (AlternativeInstance == null)
                {
                    Game[] components = FindObjectsByType<Game>(FindObjectsSortMode.InstanceID);
                    if (components.Length == 1)
                    {
                        AlternativeInstance = components[0];
                        AlternativeInstance.Initialize();
                    }
                    else
                    {
                        if (components.Length == 0)
                        {
                            var emptyObject = new GameObject("GameManager");
                            Type TargetClass = GameManager.Settings.GetGameClass();
                            if (TargetClass != null)
                            {
                                AlternativeInstance = (Game)emptyObject.AddComponent(TargetClass);
                                if (AlternativeInstance == null)
                                {
                                    Debug.LogError($"Component of type {TargetClass} is not assignable from Game.");
                                    AlternativeInstance = emptyObject.AddComponent<Game>();
                                }
                            }
                            else
                            {
                                AlternativeInstance = emptyObject.AddComponent<Game>();
                            }
                            AlternativeInstance.Initialize();
                        }
                        else
                        {
                            Debug.LogError("The 'Game' Class Component should be active in scene & just One");
                        }

                    }
                }
                return AlternativeInstance;
            }
            private set { }
        }
        public static World World;

        //Public Variables
        public Action<World> OnWorldUpdated = delegate { };
        public Timer GameTimer;

        //Private
        private bool IsInLoading = false;
        private GameData gameData;
        private List<Service> Services = new List<Service>();



        // Basic Methods
        public GameObject GetWorldObject()
        {
            return World.gameObject;
        }

        // Add or Remove GameObject in Child 
        public void HoldGameObject(GameObject Target)
        {
            Target.transform.SetParent(this.transform);
        }
        public bool UnHoldGameObject(string GameObjectName, GameObject Target)
        {
            Transform[] Childs = GetComponentsInChildren<Transform>();
            foreach (var item in Childs)
            {
                if (item.gameObject.name == GameObjectName)
                {
                    item.SetParent(Target.transform);
                    return true;
                }
            }
            return false;
        }

        // Manager
        public IGameManager FindManager(string ClassName)
        {
            IGameManager Result = null;
            foreach (var item in GetComponents<IGameManager>())
            {
                if (item.GetType().FullName == ClassName)
                {
                    Result = item;
                    break;
                }
            }
            return Result;
        }
        public T FindManagerClass<T>() where T : class
        {
            foreach (var TargetManager in GetComponents<IGameManager>())
            {
                if (TargetManager.GetManagerClass() is T Result)
                {
                    return Result;
                }
            }
            return null;
        }

        // Call these Virtual Method for Your Custom Game
        protected virtual void Initialize()
        {
            DontDestroyOnLoad(gameObject);

            GameTimer = new Timer();
            //Find All Script that has IGameManagerInterface
            foreach (var Manager in GetComponents<IGameManager>())
            {
                Manager.InitiateManager(true);
            }
        }
        public virtual void OnWorldUpdate(World NewWorld)
        {
            if (!World)
            {
                World = NewWorld;
                OnWorldUpdated?.Invoke(NewWorld);
            }
            else
            {
                Debug.LogWarning($"This World Class ({NewWorld}) Deleted");
                Destroy(NewWorld.gameObject);
            }
        }
        protected virtual void PrintDebug(string message, LogType type)
        {
            switch (type)
            {
                case LogType.Log:
                    Debug.Log(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                default:
                    Debug.LogError("Game: PrintDebug Type is not normal type");
                    break;
            }
        }

        // Statics Functions
        public static GameObject GetGameObject()
        {
            return Instance.gameObject;
        }
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
        public static bool AddService<T>(object Owner) where T : Service, new()
        {
            try
            {
                // Instantiate a new object of the specified type
                T NServ = new T();
                NServ.Initiate(Owner);
                foreach (var item in Instance.GetComponents<IGameManager>())
                {
                    item.InitiateService(NServ);
                }
                Instance.Services.Add(NServ);
                Instance.gameData.OnNewService?.Invoke(NServ);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error instantiating {typeof(T).Name}: {ex.Message}");
                return false;
            }
        }
        public static void OpenScene(SceneReference TargetScene)
        {
            if (Instance.IsInLoading)
                return;

            if (SceneManager.GetActiveScene().buildIndex != SceneManager.GetSceneByPath(TargetScene.ScenePath).buildIndex)
            {
                Instance.StartCoroutine(Instance.LoadSceneAsync(TargetScene));
            }
            else
            {
                Debug.LogWarning("The scene is already loaded.");
            }
        }
        public static void OpenWorld(WorldScene WorldScene)
        {
            if (Instance.IsInLoading)
                return;

            if (SceneManager.GetActiveScene().buildIndex != SceneManager.GetSceneByPath(WorldScene.GetPersistent().ScenePath).buildIndex)
            {
                Instance.StartCoroutine(Instance.LoadWorldAsync(WorldScene));
            }
            else
            {
                Debug.LogWarning("The scene is already loaded.");
            }
        }
        public static void Log(string Message)
        {
            Instance.PrintDebug(Message, LogType.Log);
        }
        public static void LogWarning(string Message)
        {
            Instance.PrintDebug(Message, LogType.Warning);
        }
        public static void LogError(string Message)
        {
            Instance.PrintDebug(Message, LogType.Error);
        }
        public static bool IsValid()
        {
            return AlternativeInstance;
        }
        public static GameData Data()
        {
            if (Instance.gameData != null)
            {
                return Instance.gameData;
            }
            else
            {
                if (GameManager.Settings.GeneralGameData != null)
                {
                    Instance.gameData = GameManager.Settings.GeneralGameData;
                }
                else
                {
                    Instance.gameData = ScriptableObject.CreateInstance<GameData>();
                }
                return Instance.gameData;
            }
        }
        public static void SetGameTime(float scale)
        {
            Time.timeScale = scale;
        }


        // Protected Function
        protected bool CheckComponent<t>(out t TargetRef) where t : MonoBehaviour
        {
            TargetRef = gameObject.GetComponent<t>();
            if (TargetRef == null)
            {
                TargetRef = gameObject.AddComponent<t>();
                Debug.LogWarning($"Component of type {typeof(t).Name} was not found. [A new one has been added].");
                return false;
            }
            else
            {
                return true;
            }
        }
        //Private Function
        private IEnumerator LoadSceneAsync(SceneReference scene)
        {
            //Cheack GameData
            if (gameData == null)
            {
                Debug.LogError("GameData is not Valid");
                yield break;
            }

            //StartLoading
            IsInLoading = true;
            gameData.OnSceneLoading?.Invoke(true);

            //Fading Screen
            if (gameData.FadingTime != 0)
            {
                gameData.OnScreenFade?.Invoke(true);
                yield return new WaitForSeconds(gameData.FadingTime);
            }

            //Loading Scene
            gameData.OnScreenLoading?.Invoke(true);
            AsyncOperation Load_opertation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
            if (Load_opertation == null)
            {
                Debug.LogError("Failed to load scene. AsyncOperation is null.");
                yield break;
            }
            while (!Load_opertation.isDone)
            {
                gameData.OnLoadingProcess?.Invoke(Load_opertation.progress);
                yield return null;
            }
            gameData.OnLoadingProcess?.Invoke(1);
            yield return new WaitForSeconds(0.5f);
            gameData.OnScreenLoading?.Invoke(false);

            //Fading Screen
            if (gameData.FadingTime != 0)
            {
                gameData.OnScreenFade?.Invoke(false);
                yield return new WaitForSeconds(gameData.FadingTime);
            }

            //FinishLoading
            gameData.OnSceneLoading?.Invoke(false);
            IsInLoading = false;
        }
        private IEnumerator LoadWorldAsync(WorldScene WS)
        {
            //Cheack GameData
            if (gameData == null)
            {
                Debug.LogError("GameData is not Valid");
                yield break;
            }

            //StartLoading
            IsInLoading = true;
            gameData.OnSceneLoading?.Invoke(true);

            //Fading Screen
            if (gameData.FadingTime != 0)
            {
                gameData.OnScreenFade?.Invoke(true);
                yield return new WaitForSeconds(gameData.FadingTime);
            }

            //Loading Scene
            gameData.OnScreenLoading?.Invoke(true);

            //load Persistance Levels
            AsyncOperation Load_opertation = SceneManager.LoadSceneAsync(WS.GetPersistent(), LoadSceneMode.Single);
            if (Load_opertation == null)
            {
                Debug.LogError("Failed to load scene. AsyncOperation is null.");
                yield break;
            }
            while (!Load_opertation.isDone)
            {
                gameData.OnLoadingProcess?.Invoke(Mustard.Math.MapRangedClamp(Load_opertation.progress, 0, 1, 0, (1 / WS.GetAdditiveCount() + 1)));
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
                    gameData.OnLoadingProcess?.Invoke(Mustard.Math.MapRangedClamp(Load_opertation.progress, 0, 1, 0, (1 / WS.GetAdditiveCount() + 1 - (i + 1))));
                    yield return null;
                }

            }
            yield return new WaitForSeconds(0.5f);
            gameData.OnScreenLoading?.Invoke(false);

            //Fading Screen
            if (gameData.FadingTime != 0)
            {
                gameData.OnScreenFade?.Invoke(false);
                yield return new WaitForSeconds(gameData.FadingTime);
            }

            //FinishLoading
            gameData.OnSceneLoading?.Invoke(false);
            IsInLoading = false;
        }


    }


    public class GameData : ScriptableObject
    {
        [Header("Srvice")]
        public Action<Service> OnNewService;
        [Header("FadeScreen")]
        public Action<bool> OnScreenFade;
        public float FadingTime = 1;
        [Header("LoadScreen")]
        public Action<bool> OnScreenLoading;
        public Action<float> OnLoadingProcess;
        [Header("Status")]
        public Action<bool> OnSceneLoading;

    }


}