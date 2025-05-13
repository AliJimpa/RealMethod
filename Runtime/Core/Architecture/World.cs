using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public abstract class World : MonoBehaviour
    {
        [Header("Core")]
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
        [SerializeField]
        protected GameObject[] ExteraObject;

        // Private Variable
        private IGameManager[] Managers;
        private GameObject PlayerObject;


        private void Awake()
        {
            // ForInitateGame
            Game.IsValid(true);

            //Connect to Game Service
            Game.Service.OnServiceCreate += NewServiceCreated;
            Game.Service.IntroduceWorld(this, AdditiveWorld);

            //Find Player or Create newone
            if (!InitiatePlayer())
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
            foreach (GameObject obj in ExteraObject) // Extera Object Manager
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
        public GameObject GetPlayerObject()
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
        // Virtual Methods
        protected virtual bool InitiatePlayer()
        {
            if (IsPlayerInScene)
            {
                PlayerObject = GameObject.FindGameObjectWithTag(PlayerTag);
                if (!PlayerObject)
                {
                    Debug.LogError("PlayerGameObject Cant Find in Scne");
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
                        PlayerObject = Instantiate(DefualtPlayer, SpawnPoint.position, SpawnPoint.rotation);
                        Destroy(SpawnPoint.gameObject);
                    }
                    else
                    {
                        PlayerObject = Instantiate(DefualtPlayer, this.transform);
                    }
                    return true;
                }
                else
                {
                    PlayerObject = new GameObject("Player");
                    if (SpawnPoint)
                        Destroy(SpawnPoint.gameObject);
                    return true;
                }

            }
        }
        protected virtual void AdditiveWorld(World TargetWorld)
        {
            Debug.LogWarning($"This World Class ({TargetWorld}) Deleted");
            Destroy(TargetWorld);
        }
        // Private Methods
        private void NewServiceCreated(Service NewService)
        {
            foreach (var manager in Managers)
            {
                manager.InitiateService(NewService);
            }
        }
        // Abstract Methods
        protected abstract bool CheckExteraObject(GameObject GObj);

    }
}