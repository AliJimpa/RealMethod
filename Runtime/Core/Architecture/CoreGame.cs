using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public abstract class CoreGame : MonoBehaviour
    {
        //Private Static Variables
        private static CoreGame AlternativeInstance;

        // Public Static Variables
        public static CoreGame Instance
        {
            get
            {
                if (AlternativeInstance == null)
                {
                    // Find Game In Scene
                    CoreGame components = FindFirstObjectByType<CoreGame>();
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
                                AlternativeInstance = (CoreGame)emptyObject.AddComponent(TargetClass);
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
        public static CoreWorld World { get; private set; }

        // Private Variable
        private IGameManager[] Managers;
        private Dictionary<string, Service> GameServices = new Dictionary<string, Service>(10);



        //Public Metthods
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


        // Private Methods
        private void InitializeClass()
        {
            // Move Self GameObject to DontDestroy
            DontDestroyOnLoad(gameObject);
            // Create Game Service
            GameService MyService = new GameService();
            Instance.GameServices.Add("Game", MyService);
            MyService.OnWorldUpdat += ReplaceWorld;
            MyService.Created(this);
            // Call Initialize abstract Method
            Initialize();
        }
        private void ReplaceWorld(CoreWorld NewWorld)
        {
            World = NewWorld;
        }


        // Static Methods
        // ---Service---
        public static Service CreateService<T>(string Name, object Author)
        {
            Service newService = Activator.CreateInstance<T>() as Service;
            Instance.GameServices.Add(Name, newService);
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
        // ---Manager---
        public static T FindManager<T>() where T : class
        {
            if (World != null)
            {
                return World.GetManager<T>();
            }
            return Instance.GetManager<T>();
        }

        // Abstract Methods
        protected abstract void Initialize();






    }
}