using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public abstract class PlayerStarter : MonoBehaviour
    {
        [Header("Setting")]
        [SerializeField]
        private string posName = "None";
        public string PosName => posName;
    }

    /// <summary>
    /// Base class representing a game world / scene context.
    /// Manages the player GameObject, locates and initializes child game managers,
    /// and forwards service updates to managers. Concrete worlds should implement
    /// <see cref="WorldBegin"/> and <see cref="WorldEnd"/> to perform
    /// world-specific initialization and cleanup.
    /// </summary>
    public abstract class World : MonoBehaviour
    {
        [Header("Setting")]
        [SerializeField]
        private Prefab DefualtPlayer;


        private IGameManager[] Managers;
        private GameObject PlayerObject;



        /// <summary>
        /// Unity callback invoked when the script instance is being loaded.
        /// Connects the world to the game service, binds service callbacks,
        /// initializes child managers and locates or creates the player object,
        /// then calls <see cref="WorldBegin"/> for world-specific initialization.
        /// </summary>
        private void Awake()
        {
            //Connect to Game Service
            IMethodSync SyncProvider = Game.Service;
            if (SyncProvider.IntroduceWorld(this))
            {
                SyncProvider.BindSideWorldAdd(Notify_OnAdditiveWorldInitiate);
                SyncProvider.BindServicesUpdated(Notify_OnServicesUpdated);
            }
            else
            {
                enabled = false;
                return;
            }

            // Get All Managers
            List<IGameManager> CashManagers = new List<IGameManager>(10);
            foreach (var manager in GetComponentsInChildren<IGameManager>()) // Self Mangers
            {
                manager.InitiateManager(false);
                CashManagers.Add(manager);
            }
            Managers = new IGameManager[CashManagers.Count];
            Managers = CashManagers.ToArray();

            //Find Player or Create newone
            var scneplayer = GetPlayerInScene();
            if (scneplayer == null)
            {
                var starters = FindObjectsByType<PlayerStarter>(FindObjectsSortMode.InstanceID);
                Transform SpawnPoint = SelectSpawnPoint(starters);
                PlayerObject = SpawnPlayer(DefualtPlayer, SpawnPoint);
            }
            else
            {
                PlayerObject = scneplayer;
            }

            WorldBegin();
        }

        /// <summary>
        /// Unity callback invoked when the object is being destroyed.
        /// Unbinds previously bound service callbacks to avoid dangling references.
        /// </summary>
        private void OnDestroy()
        {
            IMethodSync SyncProvider = Game.Service;
            SyncProvider.UnbindSideWorldAdd();
            SyncProvider.UnbindServicesUpdated();
            WorldEnd();
        }


        /// <summary>
        /// Returns a manager instance of the requested type if one exists on this world.
        /// </summary>
        /// <typeparam name="T">The manager type to search for.</typeparam>
        /// <returns>The manager instance cast to <typeparamref name="T"/>, or null if not found.</returns>
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
        /// <summary>
        /// Returns a manager whose associated GameObject has the specified name.
        /// </summary>
        /// <param name="ObjectName">The name of the manager's GameObject to find.</param>
        /// <returns>The matching <see cref="IGameManager"/>, or null if none match.</returns>
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
        /// <summary>
        /// Returns the primary player <see cref="GameObject"/> for this world.
        /// </summary>
        /// <param name="index">Player index when supporting multiple players (currently unused).</param>
        /// <returns>The player GameObject reference, or null if none is present.</returns>
        public GameObject GetPlayerObject(byte index = 0)
        {
            return PlayerObject;
        }
        /// <summary>
        /// Gets a component of type <typeparamref name="T"/> from the player GameObject.
        /// </summary>
        /// <typeparam name="T">Component type to retrieve (must derive from <see cref="MonoBehaviour"/>).</typeparam>
        /// <param name="index">Player index when supporting multiple players (currently unused).</param>
        /// <returns>The component instance if found; otherwise null.</returns>
        public T GetPlayerComponent<T>(byte index = 0) where T : MonoBehaviour
        {
            return PlayerObject.GetComponent<T>();
        }
        /// <summary>
        /// Gets a component or class of type <typeparamref name="T"/> from the player GameObject.
        /// This is a generic helper that uses Unity's <c>GetComponent</c> and returns the result
        /// as a reference type.
        /// </summary>
        /// <typeparam name="T">The class type to retrieve.</typeparam>
        /// <param name="index">Player index when supporting multiple players (currently unused).</param>
        /// <returns>The requested class instance, or null if not found.</returns>
        public T GetPlayerClass<T>(byte index = 0) where T : class
        {
            return PlayerObject.GetComponent<T>();
        }
        /// <summary>
        /// Retrieves all components of type <typeparamref name="T"/> attached to the player GameObject.
        /// </summary>
        /// <typeparam name="T">Component type to retrieve.</typeparam>
        /// <param name="index">Player index when supporting multiple players (currently unused).</param>
        /// <returns>Array of components if found; otherwise null and a warning is logged.</returns>
        public T[] GetPlayerComponents<T>(byte index = 0) where T : MonoBehaviour
        {
            T[] components = PlayerObject.GetComponents<T>();
            if (components.Length > 0)
            {
                return components;
            }
            Debug.LogWarning($"No components of type {typeof(T).Name} found on {PlayerObject.name} or its children.");
            return null;
        }
        /// <summary>
        /// Retrieves all components of type <typeparamref name="T"/> from the player GameObject and its children.
        /// </summary>
        /// <typeparam name="T">Component type to retrieve.</typeparam>
        /// <param name="index">Player index when supporting multiple players (currently unused).</param>
        /// <returns>Array of components if found; otherwise null and a warning is logged.</returns>
        public T[] GetPlayerComponentsInChilderen<T>(byte index = 0) where T : MonoBehaviour
        {
            T[] components = PlayerObject.GetComponentsInChildren<T>();
            if (components.Length > 0)
            {
                return components;
            }
            Debug.LogWarning($"No components of type {typeof(T).Name} found on {PlayerObject.name} or its children.");
            return null;
        }
        /// <summary>
        /// Retrieves the first component of type <typeparamref name="T"/> found on the player GameObject or its children.
        /// </summary>
        /// <typeparam name="T">Component type to retrieve.</typeparam>
        /// <param name="index">Player index when supporting multiple players (currently unused).</param>
        /// <returns>The component instance if found; otherwise null.</returns>
        public T GetPlayerComponentsInChildren<T>(byte index = 0) where T : MonoBehaviour
        {
            return PlayerObject.GetComponentInChildren<T>();
        }


        /// <summary>
        /// Spawns the player character at the specified spawn point.
        /// </summary>
        /// <param name="playerPrefab">
        /// The player prefab to instantiate. If invalid, a fallback empty Player GameObject is created.
        /// </param>
        /// <param name="spawnPoint">
        /// The transform representing the position and rotation where the player should spawn.
        /// </param>
        /// <returns>
        /// The spawned player GameObject.
        /// </returns>
        protected virtual GameObject SpawnPlayer(Prefab playerPrefab, Transform spawnPoint)
        {
            if (playerPrefab.IsValid())
            {
                Transform player = Instantiate(playerPrefab.GetSoftClassTarget(), spawnPoint.position, spawnPoint.rotation);
                player.SendMessage("OnSpawn", this, SendMessageOptions.DontRequireReceiver);
                return player.gameObject;
            }
            else
            {
                GameObject player = new GameObject("Player");
                player.tag = "Player";
                player.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
                return player;
            }
        }
        /// <summary>
        /// Searches the current scene for an existing player GameObject.
        /// </summary>
        /// <returns>
        /// The first GameObject found with the "Player" tag, or null if none exists.
        /// </returns>
        protected virtual GameObject GetPlayerInScene()
        {
            return GameObject.FindGameObjectWithTag("Player");
        }
        /// <summary>
        /// Selects a spawn point from the available PlayerStarter components.
        /// </summary>
        /// <param name="starters">
        /// An array of PlayerStarter objects that define possible spawn locations.
        /// </param>
        /// <returns>
        /// The transform of the selected spawn point. If none are available,
        /// the current object's transform at the world origin is used.
        /// </returns>
        protected virtual Transform SelectSpawnPoint(PlayerStarter[] starters)
        {
            if (starters.Length > 0)
            {
                int index = Random.Range(0, starters.Length);
                return starters[index].transform;
            }
            else
            {
                transform.position = Vector3.zero;
                return transform;
            }
        }
        /// <summary>
        /// Called when an additive world GameObject is added to this world.
        /// Default behaviour is to destroy the provided object; override to implement custom handling.
        /// </summary>
        /// <param name="WorldObject">The additive world GameObject that was added.</param>
        protected virtual void OnAdditiveWorldAdded(GameObject WorldObject)
        {
            Destroy(WorldObject);
        }


        /// <summary>
        /// Internal callback invoked by the service when an additive world is initiated.
        /// Extracts the world GameObject, destroys the <see cref="World"/> component instance,
        /// and forwards the object to <see cref="OnAdditiveWorldAdded"/>.
        /// </summary>
        /// <param name="TargetWorld">The additive <see cref="World"/> instance that was initiated.</param>
        private void Notify_OnAdditiveWorldInitiate(World TargetWorld)
        {
            GameObject worldObject = TargetWorld.gameObject;
            Destroy(TargetWorld);
            OnAdditiveWorldAdded(worldObject);
        }
        /// <summary>
        /// Internal callback invoked when services are updated. Forwards the service update
        /// to all managers so they can resolve or react to the change.
        /// </summary>
        /// <param name="NewService">The service that changed.</param>
        /// <param name="stage">The stage or phase of the service update.</param>
        private void Notify_OnServicesUpdated(Service NewService, bool stage)
        {
            if (Managers != null)
            {
                foreach (var manager in Managers)
                {
                    manager.ResolveService(NewService, stage);
                }
            }
        }


        /// <summary>
        /// Called after the world is initialized in the context of this scene.
        /// If this world is the main world, this runs normally.
        /// If this world is loaded additively, it runs in additive context.
        /// Use this to set up scene-specific systems, not global main-world logic.
        /// </summary>
        protected abstract void WorldBegin();
        /// <summary>
        /// Called when the world is being destroyed to perform cleanup of world-specific state.
        /// Implementations should release resources and unregister any world-specific hooks here.
        /// </summary>
        protected abstract void WorldEnd();
    }
}