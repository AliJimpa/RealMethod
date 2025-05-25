using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public static GameSettingAsset Setting { get; private set; }
        public static World World { get; private set; }
        public static GameService Service { get; private set; }

        // Private Variable
        private IGameManager[] Managers;
        private Dictionary<string, Service> GameServices = new Dictionary<string, Service>(10);



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
        // Protected Methods
        protected bool NeedComponent<t>(out t TargetRef) where t : MonoBehaviour
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
        // Private Methods
        private void ReplaceWorld(World NewWorld)
        {
            World = NewWorld;
            foreach (var service in GameServices)
            {
                service.Value.WorldUpdated();
            }
            WorldSynced(World);
        }
        // Public Static Methods
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
        public static Service CreateService<T>(string Name, object Author)
        {
            Service newService = Activator.CreateInstance<T>() as Service;
            if (Instance.GameServices.TryAdd(Name, newService))
            {
                Service.OnServiceCreate?.Invoke(newService);
                foreach (var manager in Instance.Managers)
                {
                    manager.InitiateService(newService);
                }
                newService.Start(Author);
                return newService;
            }
            else
            {
                Debug.LogWarning($"A service with the name '{Name}' already exists and will not be replaced.");
                return null;
            }
        }
        public static bool DeleteService(string Name, object Author)
        {
            if (!Instance.GameServices.TryGetValue(Name, out var Result))
                return false;

            Result.End(Author);
            Instance.GameServices.Remove(Name);
            return true;
        }
        public static Service FindService(string Name)
        {
            return Instance.GameServices[Name];
        }
        public static bool TryFindService<T>(string Name, out T Target) where T : Service
        {
            if (Instance.GameServices.TryGetValue(Name, out Service service) && service is T tService)
            {
                Target = tService;
                return true;
            }
            Target = null;
            return false;
        }
        public static void OpenScene(SceneReference TargetScene)
        {
            if (!Service.IsLoading && SceneManager.GetActiveScene().buildIndex != SceneManager.GetSceneByPath(TargetScene).buildIndex)
            {
                Instance.StartCoroutine(Service.LoadSceneAsync(TargetScene));
            }
            else
            {
                Debug.LogWarning("The scene is already loaded.");
            }
        }
        public static void OpenWorld(WorldSceneAsset WorldScene)
        {
            if (!Service.IsLoading && SceneManager.GetActiveScene().buildIndex != SceneManager.GetSceneByPath(WorldScene.GetPersistent()).buildIndex)
            {
                Instance.StartCoroutine(Service.LoadWorldAsync(WorldScene));
            }
            else
            {
                Debug.LogWarning("The scene is already loaded.");
            }
        }
        public static T FindManager<T>() where T : class
        {
            if (World != null)
            {
                return World.GetManager<T>();
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
        // Private Static Methods
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
                    // Call InstanceCreated abstract Method
                    AlternativeInstance.InstanceCreated();
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
                Service.OnWorldUpdate += AlternativeInstance.ReplaceWorld;
                Service.Start(AlternativeInstance);
                // Set Game Setting Asset 
                if (ProjectSettings.GetGameSetting() != null)
                {
                    Setting = ProjectSettings.GetGameSetting();
                }
                else
                {
                    Setting = ScriptableObject.CreateInstance<DefaultGameSetting>();
                }
                Setting.GameStarted(AlternativeInstance);
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
        protected abstract void InstanceCreated();
        protected abstract void GameInitialized();
        protected abstract void WorldSynced(World NewWorld);

    }
}