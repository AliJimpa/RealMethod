using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// Base class representing a locatioon for spawn player in scene 
    /// some virtual function in world calls can find all of object that has this class and with these spawing to location
    /// </summary>
    public abstract class PlayerStarter : MonoBehaviour
    {
        [Header("Details")]
        [SerializeField]
        private string posName = "None";
        public string PosName => posName;

#if UNITY_EDITOR
        [Header("Gizmo")]
        [SerializeField]
        private float height = 2f;
        [SerializeField]
        private float radius = 0.5f;
        [SerializeField]
        private bool BottomPivot = false;
        private Vector3 Pivot => BottomPivot ? transform.position + new Vector3(0, height / 2, 0) : transform.position;
#endif

        private bool hasPlayer = false;

        // Unity Method
        private void Awake()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Attempts to mark this spawn point as occupied by a player.
        /// </summary>
        /// <returns>
        /// True if the spawn point was successfully marked as occupied; false if it is already occupied.
        /// </returns>
        public bool TryStartHere()
        {
            if (hasPlayer)
                return false;

            hasPlayer = true;
            return true;
        }
        /// <summary>
        /// Check is player Spawn on this location.
        /// </summary>
        /// <returns>
        /// True is already occupied.
        /// </returns>
        public bool CanStartHere()
        {
            return hasPlayer;
        }


#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            bool blocked = Physics.CheckCapsule(Pivot, transform.position + Vector3.up * height, radius);

            Color c = blocked ? Color.red : Color.cyan;

            RM_Gizmos.Capsule(Pivot, c, height, radius);
            RM_Gizmos.Arrow(Pivot, transform.forward, Color.red);
            RM_Gizmos.Text(PosName, Pivot + (transform.up * (height / 2)) + (Vector3.up * 0.1f), Color.black);
        }
#endif
    }

    /// <summary>
    /// Base class representing a game world / scene context.
    /// Manages the player GameObject, locates and initializes child game managers,
    /// Concrete worlds should implement
    /// <see cref="WorldBegin"/> and <see cref="WorldEnd"/> to perform
    /// world-specific initialization and cleanup.
    /// </summary>
    public abstract class World : Scope
    {
        [Header("Setting")]
        [SerializeField]
        private Prefab DefaultPlayer;
        /// <summary>
        /// This action called every time your game ready to play after load Scene & setup RealMethod
        /// you can enshure that your game and world do anything and player can ready to play game
        /// when you change scene after world initiate this evet invoke again.
        /// </summary>
        public static event System.Action OnReady;


        private GameObject PlayerObject;


        /// <summary>
        /// Unity callback invoked when the script instance is being loaded.
        /// initializes child managers and locates or creates the player object,
        /// then calls <see cref="WorldBegin"/> for world-specific initialization.
        /// </summary>
        private void Awake()
        {
            //Connect to Game With Bridge
            IWorldBridge SyncProvider = Game.Bridge;
            if (!SyncProvider.RegisterWorld(this))
            {
                return;
            }

            // Get All Managers
            OpenScope(new GameObject[1] { gameObject });

            // Find Player or Create newone
            var scenePlayer = GetPlayerInScene();
            if (scenePlayer == null)
            {
                var starters = FindObjectsByType<PlayerStarter>(FindObjectsSortMode.InstanceID);
                Transform SpawnPoint = SelectSpawnPoint(starters);
                PlayerObject = SpawnPlayer(DefaultPlayer, SpawnPoint);
            }
            else
            {
                PlayerObject = scenePlayer;
            }

            // Reset Location of World
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;

            WorldBegin();
            OnReady?.Invoke();
        }
#if UNITY_EDITOR
        private void OnEnable()
        {

        }
        private void OnDisable()
        {
        }
        private void LateUpdate()
        {
            if (transform.position != Vector3.zero)
            {
                Debug.LogWarning($"{gameObject.name} position should not be changed!");
                transform.position = Vector3.zero;
            }
            if (transform.rotation != Quaternion.identity)
            {
                Debug.LogWarning($"{gameObject.name} rotation should not be changed!");
                transform.rotation = Quaternion.identity;
            }
        }
        private void Update()
        {

        }
        private void FixedUpdate()
        {

        }
        public sealed override IInspectorInfo[] GetAllInfo()
        {
            return null;
        }
#endif
        /// <summary>
        /// Unity callback invoked when the object is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            CloseScope();
            WorldEnd();
        }



        /// <summary>
        /// Returns the primary player <see cref="GameObject"/> for this world.
        /// </summary>
        /// <param name="index">Player index when supporting multiple players (currently unused).</param>
        /// <returns>The player GameObject reference, or null if none is present.</returns>
        public GameObject GetPlayer(byte index = 0)
        {
            return PlayerObject;
        }
        /// <summary>
        /// Gets a component of type <typeparamref name="T"/> from the player GameObject.
        /// </summary>
        /// <typeparam name="T">Component type to retrieve (must derive from <see cref="MonoBehaviour"/>).</typeparam>
        /// <param name="index">Player index when supporting multiple players (currently unused).</param>
        /// <returns>The component instance if found; otherwise null.</returns>
        public T GetPlayerComponent<T>(byte index = 0) where T : Component
        {
            return GetPlayer(index).GetComponent<T>();
        }
        /// <summary>
        /// Retrieves all components of type <typeparamref name="T"/> attached to the player GameObject.
        /// </summary>
        /// <typeparam name="T">Component type to retrieve.</typeparam>
        /// <param name="index">Player index when supporting multiple players (currently unused).</param>
        /// <returns>Array of components if found; otherwise null and a warning is logged.</returns>
        public T[] GetPlayerComponents<T>(byte index = 0) where T : Component
        {
            T[] components = GetPlayer(index).GetComponents<T>();
            if (components.Length > 0)
            {
                return components;
            }
            Debug.LogWarning($"No components of type {typeof(T).Name} found on {GetPlayer(index).name} or its children.");
            return null;
        }
        /// <summary>
        /// Retrieves all components of type <typeparamref name="T"/> from the player GameObject and its children.
        /// </summary>
        /// <typeparam name="T">Component type to retrieve.</typeparam>
        /// <param name="index">Player index when supporting multiple players (currently unused).</param>
        /// <returns>Array of components if found; otherwise null and a warning is logged.</returns>
        public T[] GetPlayerComponentsInChilderen<T>(byte index = 0) where T : Component
        {
            T[] components = GetPlayer(index).GetComponentsInChildren<T>();
            if (components.Length > 0)
            {
                return components;
            }
            Debug.LogWarning($"No components of type {typeof(T).Name} found on {GetPlayer(index).name} or its children.");
            return null;
        }
        /// <summary>
        /// Retrieves the first component of type <typeparamref name="T"/> found on the player GameObject or its children.
        /// </summary>
        /// <typeparam name="T">Component type to retrieve.</typeparam>
        /// <param name="index">Player index when supporting multiple players (currently unused).</param>
        /// <returns>The component instance if found; otherwise null.</returns>
        public T GetPlayerComponentsInChildren<T>(byte index = 0) where T : Component
        {
            return GetPlayer(index).GetComponentInChildren<T>();
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
                Transform player = Instantiate(playerPrefab.GetMainComponent(), spawnPoint.position, spawnPoint.rotation);
                player.SendMessage("OnSpawn", this, SendMessageOptions.DontRequireReceiver);
                return player.gameObject;
            }
            else
            {
                GameObject PlayerObject;
                ProjectSettingAsset ProjectSettings = RM_Framework.LoadProjectSetting();
                if (ProjectSettings != null)
                {
                    GameObject CameraObj = ProjectSettings.GetSpectator();
                    if (CameraObj)
                    {
                        PlayerObject = Instantiate(CameraObj, spawnPoint.position, spawnPoint.rotation);
                    }
                    else
                    {
                        PlayerObject = CreateDefaultSpectator(spawnPoint);
                    }
                }
                else
                {
                    PlayerObject = CreateDefaultSpectator(spawnPoint);
                }
                RM_Framework.UnloadProjectSetting();
                return PlayerObject;
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
                if (!starters[index].CanStartHere())
                {
                    return starters[index].transform;
                }
                foreach (var start in starters)
                {
                    if (!start.CanStartHere())
                    {
                        if (start.TryStartHere())
                        {
                            return start.transform;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            transform.position = Vector3.zero;
            return transform;
        }

        /// <summary>
        /// Create Emoty GameOpbject and requerment component for
        /// making Spectator
        /// </summary>
        /// <param name="spawnPoint"></param>
        /// <returns></returns>
        private GameObject CreateDefaultSpectator(Transform spawnPoint)
        {
            GameObject NewPlayer = new GameObject("Player");
            NewPlayer.tag = "Player";
            NewPlayer.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            NewPlayer.AddComponent<Camera>();
            NewPlayer.AddComponent<AudioListener>();
            NewPlayer.AddComponent<DefaultSpectator>();
            return NewPlayer;
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