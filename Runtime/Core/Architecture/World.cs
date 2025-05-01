using UnityEngine;
using System.Collections.Generic;

[DefaultExecutionOrder(-1)]
public abstract class World : MonoBehaviour
{
    [Header("World")]
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
    private bool AdditiveWorld = false;
    [SerializeField]
    protected List<GameObject> ExteraObject = new List<GameObject>();
    protected List<IGameManager> Managers = new List<IGameManager>();
    private GameObject PlayerObject;


    //Internal Methods
    protected virtual void Awake()
    {
        World TopWorld = Game.World;
        if (TopWorld)
        {
            if (AdditiveWorld)
            {
                TopWorld.ExteraObject.Add(this.gameObject);
                foreach (var item in GetComponentsInChildren<IGameManager>())
                {
                    TopWorld.Managers.Add(item);
                    item.InitiateManager(false);
                }
                foreach (GameObject Item in ExteraObject)
                {
                    foreach (var Manager in Item.GetComponentsInChildren<IGameManager>())
                    {
                        TopWorld.Managers.Add(Manager);
                        Manager.InitiateManager(false);
                    }
                }
                gameObject.name = "AdditiveWorld_" + TopWorld.ExteraObject.Count.ToString();
                OnAdditiveWorldActive(TopWorld);
            }
            else
            {
                foreach (GameObject Item in ExteraObject)
                {
                    foreach (var Manager in Item.GetComponentsInChildren<IGameManager>())
                    {
                        TopWorld.Managers.Add(Manager);
                        Manager.InitiateManager(false);
                    }
                }
                Destroy(gameObject);
            }
        }
        else
        {
            InitiateWorld();
        }

    }

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private void InitiateWorld()
    {
        //Introduce Self to GameInstance
        Game.Instance.OnWorldUpdate(this);
        Game.Data().OnNewService += OnServiceCreated;

        //Find Player or Create newone
        DetectPlayer();

        //Find All Script that has IGameManagerInterface
        foreach (var item in GetComponentsInChildren<IGameManager>())
        {
            Managers.Add(item);
        }

        //Process Extera Objects
        foreach (GameObject obj in ExteraObject)
        {
            IGameManager Manager = obj.GetComponent<IGameManager>();
            if (Manager != null)
            {
                Managers.Add(Manager);
            }
            CheckExteraObject(obj);
        }

        //InitiateManagers
        foreach (var item in Managers)
        {
            item.InitiateManager(false);
        }
    }

    // Extera Object Methods
    public GameObject FindExteraObject(string ObjectName)
    {
        GameObject Result = null;
        foreach (var obj in ExteraObject)
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

    // Player Method
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

    //Manager Method
    public IGameManager FindManager(string ClassName)
    {
        IGameManager Result = null;
        foreach (var item in Managers)
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
        foreach (var Mg in Managers)
        {
            if (Mg.GetManagerClass() is T Result)
            {
                return Result;
            }
        }
        return null;
    }

    //virtual Method
    protected virtual bool DetectPlayer()
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
    protected virtual void CheckExteraObject(GameObject Obj)
    {
    }
    protected virtual void OnAdditiveWorldActive(World TopWorld)
    {
        Destroy(this);
    }

    //Functions
    protected GameObject AddGameObject(GameObject Prefab)
    {
        GameObject SpawnedObject = Instantiate(Prefab, transform.position, Quaternion.identity);
        foreach (var item in SpawnedObject.GetComponentsInChildren<IGameManager>())
        {
            Managers.Add(item);
            item.InitiateManager(false);
        }
        SpawnedObject.transform.SetParent(this.transform);
        return SpawnedObject;
    }
    private void OnServiceCreated(Service NewServ)
    {
        foreach (var item in Managers)
        {
            item.InitiateService(NewServ);
        }
    }


}