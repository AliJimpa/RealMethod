using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public abstract class World : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField]
        private bool IsPlayerInScene = true;
        [SerializeField]
        [ConditionalHide("IsPlayerInScene", true, false)]
        [TagSelector]
        private string PlayerTag = "Player";
        [SerializeField]
        [ConditionalHide("IsPlayerInScene", true, true)]
        private GameObject DefualtPlayer;
        [SerializeField]
        [ConditionalHide("IsPlayerInScene", true, true)]
        private Transform SpawnPoint;
        [Space(5)]
        [SerializeField]
        protected GameObject[] ExtraObject;

        // Private Variable
        private IGameManager[] Managers;
        private GameObject PlayerObject;

        // Base Method
        private void Awake()
        {
            // ForInitateGame
            Game.IsValid(true);

            //Connect to Game Service
            if (Game.Service.Worldprovider.IntroduceWorld(this))
            {
                Game.Service.OnAdditiveWorld += AdditiveWorld;
                Game.Service.OnServiceCreate += NewServiceCreated;
            }
            else
            {
                return;
            }

            //Find Player or Create newone
            if (!InitiatePlayer(ref PlayerObject))
            {
                return;
            }

            // Get All Managers
            List<IGameManager> CashManagers = new List<IGameManager>(10);
            foreach (var manager in GetComponentsInChildren<IGameManager>()) // Self Mangers
            {
                manager.InitiateManager(false);
                CashManagers.Add(manager);
            }
            foreach (GameObject obj in ExtraObject) // Extera Object Manager
            {
                if (CheckExteraObject(obj))
                {
                    foreach (var manager in obj.GetComponentsInChildren<IGameManager>())
                    {
                        manager.InitiateManager(false);
                        CashManagers.Add(manager);
                    }
                }
            }
            Managers = new IGameManager[CashManagers.Count];
            Managers = CashManagers.ToArray();

            AwakeWorld();
        }
        private void OnDestroy()
        {
            Game.Service.OnAdditiveWorld -= AdditiveWorld;
            Game.Service.OnServiceCreate -= NewServiceCreated;
        }
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
        public GameObject GetPlayerObject(byte index = 0)
        {
            return PlayerObject;
        }
        public T[] GetComponentsInPlayer<T>() where T : MonoBehaviour
        {
            T[] components = PlayerObject.GetComponentsInChildren<T>();
            if (components.Length > 0)
            {
                return components;
            }
            Debug.LogWarning($"No components of type {typeof(T).Name} found on {PlayerObject.name} or its children.");
            return null;
        }
        public T GetComponentInPlayer<T>() where T : MonoBehaviour
        {
            return PlayerObject.GetComponentInChildren<T>();
        }
        public GameObject FindExteraObject(string ObjectName)
        {
            GameObject Result = null;
            foreach (var obj in ExtraObject)
            {
                if (obj.name == ObjectName)
                {
                    Result = obj;
                }
            }
            return Result;
        }
        public T GetComponentInExteraObject<T>(string ObjectName) where T : MonoBehaviour
        {
            GameObject TargetObject = FindExteraObject(ObjectName);
            if (TargetObject)
            {
                T TargetComponent = TargetObject.GetComponent<T>();
                if (TargetComponent)
                {
                    return TargetComponent;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        // Virtual Methods
        protected virtual bool InitiatePlayer(ref GameObject player)
        {
            if (IsPlayerInScene)
            {
                player = GameObject.FindGameObjectWithTag(PlayerTag);
                if (!player)
                {
                    Debug.LogError("PlayerGameObject Cant Find in Scene");
                    return false;
                }
                return true;
            }
            else
            {
                if (DefualtPlayer != null)
                {
                    if (SpawnPoint)
                    {
                        player = Instantiate(DefualtPlayer, SpawnPoint.position, SpawnPoint.rotation);
                    }
                    else
                    {
                        player = Instantiate(DefualtPlayer, transform.position, Quaternion.identity);
                    }
                    return true;
                }
                else
                {
                    player = new GameObject("Player");
                    player.tag = "Player";
                    return true;
                }

            }
        }
        protected virtual void AdditiveWorld(World TargetWorld)
        {
            Debug.LogWarning($"This World Class ({TargetWorld}) Deleted");
            Destroy(TargetWorld);
        }
        protected virtual bool CheckExteraObject(GameObject GObj)
        {
            if (GObj != null)
            {
                return true;
            }
            else
            {
                Debug.LogWarning($"Ther ExterObject isn't valid");
                return false;
            }
        }
        // Private Methods
        private void NewServiceCreated(Service NewService)
        {
            if (Managers != null)
            {
                foreach (var manager in Managers)
                {
                    manager.InitiateService(NewService);
                }
            }
        }
        // Abstract Methods
        protected abstract void AwakeWorld();
        protected abstract void DestroyWorld();
    }
}