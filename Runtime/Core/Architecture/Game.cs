using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace RealMethod
{
    public abstract class Game : MonoBehaviour
    {
        // Private Static Variables
        private static Game AlternativeInstance;

        // Public Static Variables
        public static Game Instance
        {
            get
            {
                if (AlternativeInstance == null)
                {
                    // Find Game In Scene
                    Game[] components = FindObjectsByType<Game>(FindObjectsSortMode.InstanceID);
                    if (components.Length == 1)
                    {
                        AlternativeInstance = components[0];
                        InitializeGame(false);
                    }
                    else
                    {
                        if (components.Length == 0)
                        {
                            InitializeGame();
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
        public static GameConfig Config { get; private set; }
        public static World World { get; private set; }
        public static GameService Service { get; private set; }

        // Private Variable
        private IGameManager[] Managers;
        private List<Service> GameServices;




        // Public Metthods
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
        // Protected Functions
        protected T NeedManager<T>() where T : MonoBehaviour
        {
            T Result = GetManager<T>();
            if (Result == null)
            {
                Result = gameObject.AddComponent<T>();
                Debug.LogWarning($"Manager of type {typeof(T).Name} was not found. [A new one has been added].");

            }
            return Result;
        }
        // Private Functions
        private void ReplaceWorld(World NewWorld)
        {
            World = NewWorld;
            foreach (var service in GameServices)
            {
                service.WorldUpdated();
            }
            WorldSynced(World);
        }
        // Public Static Functions
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
        public static T AddService<T>(object author) where T : Service, new()
        {
            // Check if a service of this type already exists
            if (Instance.GameServices.Any(s => s.GetType() == typeof(T)))
            {
                Debug.LogWarning($"Service of type {typeof(T).Name} already exists.");
                return null;
            }

            T newService = new T();
            Instance.GameServices.Add(newService);

            Service.OnServiceCreate?.Invoke(newService);
            if (Instance.Managers != null)
            {
                foreach (var manager in Instance.Managers)
                {
                    manager.InitiateService(newService);
                }
            }
            newService.Start(author);

            return newService;
        }
        public static bool RemoveService<T>(object author) where T : Service
        {
            var service = Instance.GameServices.FirstOrDefault(s => s.GetType() == typeof(T));
            if (service != null)
            {
                service.End(author);
                Instance.GameServices.Remove(service);
                return true;
            }

            Debug.LogWarning($"Service of type {typeof(T).Name} not found to remove.");
            return false;
        }
        public static T GetService<T>() where T : Service
        {
            return Instance.GameServices.OfType<T>().FirstOrDefault();
        }
        public static Service GetService(Type type)
        {
            if (!typeof(Service).IsAssignableFrom(type))
            {
                Debug.LogError($"Type {type.Name} is not a valid Service.");
                return null;
            }

            return Instance.GameServices.FirstOrDefault(s => s.GetType() == type);
        }
        public static bool TryFindService<T>(out T service) where T : Service
        {
            service = Instance.GameServices.OfType<T>().FirstOrDefault();
            return service != null;
        }
        public static Coroutine OpenScene(SceneReference TargetScene)
        {
            if (!Service.IsLoading && SceneManager.GetActiveScene().buildIndex != SceneManager.GetSceneByPath(TargetScene).buildIndex)
            {
                return Instance.StartCoroutine(Service.GetLoadScneCorotine(TargetScene));
            }
            else
            {
                Debug.LogWarning("The scene is already loaded.");
                return null;
            }
        }
        public static Coroutine OpenWorld(WorldSceneConfig WorldScene)
        {
            if (!Service.IsLoading && SceneManager.GetActiveScene().buildIndex != SceneManager.GetSceneByPath(WorldScene.GetPersistent()).buildIndex)
            {
                return Instance.StartCoroutine(Service.GetLoadWorldCorotine(WorldScene));
            }
            else
            {
                Debug.LogWarning("The scene is already loaded.");
                return null;
            }
        }
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
        public static void HoldGameObject(GameObject Target)
        {
            Target.transform.SetParent(Instance.transform);
        }
        public static bool UnHoldGameObject(string GameObjectName, GameObject Target)
        {
            Transform[] Childs = Instance.GetComponentsInChildren<Transform>();
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
        public static bool IsValid(bool Initiate = false)
        {
            if (Initiate)
            {
                return Instance;
            }
            else
            {
                return AlternativeInstance;
            }
        }
        // Private Static Functions
        private static void InitializeGame(bool CreateInstance = true)
        {
            // Load Project Setting
            ProjectSettingAsset ProjectSettings = Resources.Load<ProjectSettingAsset>("RealMethod/RealMethodSetting");

            if (ProjectSettings != null)
            {
                // Initiate Game Class in Game
                if (CreateInstance)
                {
                    var emptyObject = new GameObject("GameManager");
                    Type TargetClass = ProjectSettings.GetGameInstanceClass();
                    if (TargetClass != null)
                    {
                        AlternativeInstance = (Game)emptyObject.AddComponent(TargetClass);
                        if (AlternativeInstance == null)
                        {
                            Debug.LogError($"Component of type {TargetClass} is not assignable from Game.");
                            AlternativeInstance = emptyObject.AddComponent<DefultGame>();
                        }
                    }
                    else
                    {
                        AlternativeInstance = emptyObject.AddComponent<DefultGame>();
                    }
                }
                // Create Game Service
                Type targetService = ProjectSettings.GetGameServiceClass();
                if (targetService != null)
                {
                    if (typeof(Service).IsAssignableFrom(targetService))
                    {
                        try
                        {
                            Service = (GameService)Activator.CreateInstance(targetService);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Failed to instantiate {targetService}: {ex.Message}");
                            Service = new DefaultGameService();
                        }
                    }
                    else
                    {
                        Debug.LogError($"Type {targetService} is not assignable to Service.");
                        Service = new DefaultGameService();
                    }
                }
                else
                {
                    Service = new DefaultGameService();
                }
                AlternativeInstance.GameServices = new List<Service>(3);
                Service.OnWorldUpdate += AlternativeInstance.ReplaceWorld;
                Service.Start(AlternativeInstance);
                // CreateStartService Abstract
                AlternativeInstance.GameServiceCreated();
                // Set Game Setting Asset 
                if (ProjectSettings.GetGameConfig() != null)
                {
                    Config = ProjectSettings.GetGameConfig();
                }
                else
                {
                    Config = ScriptableObject.CreateInstance<DefaultGameConfig>();
                }
                Config.GameStarted(AlternativeInstance);
                // Initiate GamePrefab & Managers
                List<IGameManager> CashManagers = new List<IGameManager>(5);
                foreach (var obj in ProjectSettings.GetGamePrefabs())
                {
                    if (obj != null)
                    {
                        GameObject newobj = Instantiate(obj);
                        foreach (var manager in newobj.GetComponents<IGameManager>())
                        {
                            manager.InitiateManager(true);
                            CashManagers.Add(manager);
                        }
                        DontDestroyOnLoad(newobj);
                    }
                }
                AlternativeInstance.Managers = new IGameManager[CashManagers.Count];
                AlternativeInstance.Managers = CashManagers.ToArray();
                // Unload Project Setting
                Resources.UnloadAsset(ProjectSettings);

                // Move Self GameObject to DontDestroy
                DontDestroyOnLoad(AlternativeInstance.gameObject);
                // Call Initialize abstract Method
                AlternativeInstance.GameInitialized();
            }
            else
            {
                Debug.LogError("ProjectSettingAsset is missing from Resources folder!");
            }
        }
        // Abstract Methods
        protected abstract void GameServiceCreated();
        protected abstract void GameInitialized();
        protected abstract void WorldSynced(World NewWorld);

    }
}