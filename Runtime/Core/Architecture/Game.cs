using System;
using System.Collections.Generic;
using UnityEngine;

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
                    Game components = FindFirstObjectByType<Game>();
                    if (components)
                    {
                        AlternativeInstance = components;
                        AlternativeInstance.InitializeClass();
                    }
                    else
                    {
                        var emptyObject = new GameObject("GameManager");
                        ProjectSettingAsset ProjectSettings = Resources.Load<ProjectSettingAsset>("RealMethod/RealMethodSetting");
                        if (ProjectSettings != null)
                        {
                            Type TargetClass = ProjectSettings.GetGameInstanceClass();
                            // Initiate Game Class in Game
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
                            // Set Game Setting Asset 
                            if (ProjectSettings.GetGameSetting() != null)
                            {
                                Setting = ProjectSettings.GetGameSetting();
                            }
                            else
                            {
                                Setting = ScriptableObject.CreateInstance<DefaultGameSetting>();
                            }
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
                            AlternativeInstance.InitializeClass();
                        }
                        else
                        {
                            Debug.LogError("ProjectSettingAsset is missing from Resources folder!");
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
        public IGameManager GetManager(string ClassName)
        {
            foreach (var manger in Managers)
            {
                if (manger.GetType().FullName == ClassName)
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
        private void InitializeClass()
        {
            // Move Self GameObject to DontDestroy
            DontDestroyOnLoad(gameObject);
            // Create Game Service
            Service = new GameService();
            Service.OnWorldUpdate += ReplaceWorld;
            Service.Created(this);
            // Call Initialize abstract Method
            Initialize();
        }
        private void ReplaceWorld(World NewWorld)
        {
            World = NewWorld;
            WorldSynced(World);
        }
        // Static Methods
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
            Instance.GameServices.Add(Name, newService);
            Service.OnServiceCreate?.Invoke(newService);
            foreach (var manager in Instance.Managers)
            {
                manager.InitiateService(newService);
            }
            newService.Created(Author);
            return newService;
        }
        public static bool DeleteService(string Name, object Author)
        {
            if (!Instance.GameServices.TryGetValue(Name, out var Result))
                return false;

            Result.Removed(Author);
            Instance.GameServices.Remove(Name);
            return true;
        }
        public static Service FindService(string Name)
        {
            return Instance.GameServices[Name];
        }
        public static bool FindService(string Name, out Service Target)
        {
            return Instance.GameServices.TryGetValue(Name, out Target);
        }
        public static Service FindService<T>() where T : Service
        {
            foreach (var service in Instance.GameServices.Values)
            {
                if (service is T targetService)
                {
                    return targetService;
                }
            }
            return null;
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
        // Abstract Methods
        protected abstract void Initialize();
        protected abstract void WorldSynced(World NewWorld);






    }
}