using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public abstract class CoreWorld : MonoBehaviour
    {
        [Header("World")]
        [SerializeField]
        protected GameObject[] ExteraObject;


        private IGameManager[] Managers;


        private void Awake()
        {
            GameService Gameserv = (GameService)CoreGame.FindService("Game");
            Gameserv.IntroduceWorld(this, AdditiveWorld);

            List<IGameManager> CashManagers = new List<IGameManager>(10);
            foreach (var manager in GetComponentsInChildren<IGameManager>())
            {
                manager.InitiateManager(false);
                CashManagers.Add(manager);
            }
            foreach (GameObject Item in ExteraObject)
            {
                foreach (var manager in Item.GetComponentsInChildren<IGameManager>())
                {
                    manager.InitiateManager(false);
                    CashManagers.Add(manager);
                }
            }
            Managers = new IGameManager[CashManagers.Count];
            Managers = CashManagers.ToArray();
        }


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



        // Virtual Methods
        protected virtual void AdditiveWorld(CoreWorld TargetWorld)
        {
            Debug.LogWarning($"This World Class ({TargetWorld}) Deleted");
            Destroy(TargetWorld);
        }




    }
}