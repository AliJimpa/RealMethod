using UnityEngine;

namespace RealMethod
{
    public abstract class ResourcAsset : DataAsset
    {
        [SerializeField]
        protected string resourcePath;
        public string path => resourcePath;

        private GameObject loadedAsset;
        public bool isLoaded => loadedAsset != null;
        private int pointer = 0;

        //public System.Action<PrefabCore> Onloaded;

        // public override bool IsValid()
        // {
        //     return !string.IsNullOrEmpty(resourcePath);
        // }
        // public override GameObject GetPrefab()
        // {
        //     if (!isLoaded)
        //     {
        //         if (!Load())
        //         {
        //             return null;
        //         }
        //     }
        //     pointer++;
        //     return loadedAsset;
        // }

        // // Public Functions
        // public bool Load()
        // {
        //     if (isLoaded)
        //     {
        //         Debug.LogError("That prefab is already loaded");
        //         return false;
        //     }

        //     if (!IsValid())
        //     {
        //         Debug.LogError("Prefab path is not set.");
        //         return false;
        //     }

        //     if (loadedAsset == null)
        //     {
        //         loadedAsset = Resources.Load<GameObject>(resourcePath);
        //     }

        //     if (loadedAsset == null)
        //     {
        //         Debug.LogError($"Prefab not found at Resources/{resourcePath}");
        //         return false;
        //     }

        //     Onloaded?.Invoke(this);
        //     return true;
        // }
        // public async Task LoadAsync()
        // {
        //     if (isLoaded)
        //     {
        //         Debug.LogError("That prefab is already loaded");
        //         return;
        //     }

        //     if (!IsValid())
        //     {
        //         Debug.LogError("Invalid resource path.");
        //         return;
        //     }

        //     var request = Resources.LoadAsync<GameObject>(resourcePath);
        //     while (!request.isDone)
        //         await Task.Yield(); // Wait until finished

        //     loadedAsset = request.asset as GameObject;
        //     if (loadedAsset == null)
        //     {
        //         Debug.LogError($"Failed to load prefab at path: {resourcePath}");
        //         return;
        //     }

        //     Onloaded?.Invoke(this);
        // }
        // public void LoadAsync(MonoBehaviour Owner, System.Action<PrefabCore> loaded)
        // {
        //     if (Owner != null)
        //     {
        //         Owner.StartCoroutine(LoadRoutine(loaded));
        //     }
        //     else
        //     {
        //         Debug.LogError("Owner is not valid!");
        //     }
        // }
        // public bool Unload(bool Force = false)
        // {
        //     if (isLoaded)
        //     {
        //         if (pointer == 0 || Force)
        //         {
        //             Resources.UnloadAsset(loadedAsset);
        //             loadedAsset = null;
        //             return true;
        //         }
        //         else
        //         {
        //             Debug.LogWarning($"{this} For this prefab [{loadedAsset}] you use ({pointer}) Instance");
        //             return false;
        //         }
        //     }
        //     else
        //     {
        //         Debug.LogWarning("First Load Prefab!");
        //         return false;
        //     }
        // }
        // public bool Destroy(GameObject instance)
        // {
        //     if (instance != null)
        //     {
        //         Destroy(instance);
        //         pointer--;
        //         return true;
        //     }
        //     else
        //     {
        //         return false;
        //     }
        // }

        // public J GetSoftClass<J>() where J : Component
        // {
        //     if (loadedAsset != null)
        //     {
        //         return loadedAsset.GetComponent<J>();
        //     }
        //     else
        //     {
        //         Debug.LogWarning("First Load Prefab!");
        //         return null;
        //     }
        // }
        // public J[] GetAllSoftClass<J>() where J : Component
        // {
        //     if (loadedAsset != null)
        //     {
        //         return loadedAsset.GetComponentsInChildren<J>();
        //     }
        //     else
        //     {
        //         Debug.LogWarning("First Load Prefab!");
        //         return null;
        //     }
        // }

        // private IEnumerator LoadRoutine(System.Action<PrefabCore> result)
        // {
        //     if (isLoaded)
        //     {
        //         Debug.LogError("That prefab is already loaded");
        //         yield break;
        //     }

        //     ResourceRequest request = Resources.LoadAsync<GameObject>(resourcePath);
        //     yield return request;

        //     loadedAsset = request.asset as GameObject;
        //     if (loadedAsset == null)
        //     {
        //         Debug.LogError($"Failed to load prefab at path: {resourcePath}");
        //         yield break;
        //     }

        //     Onloaded?.Invoke(this);
        //     result?.Invoke(this);
        // }

    }


    
    // [System.Serializable]
    // public class AddressablePrefab<T> where T : Component
    // {
    //     [SerializeField] private AssetReferenceGameObject prefabRef;

    //     private GameObject loadedPrefab;

    //     public bool IsValid => prefabRef != null;

    //     public AsyncOperationHandle<GameObject> LoadAsync()
    //     {
    //         if (!IsValid)
    //         {
    //             Debug.LogError("Invalid prefab reference.");
    //             return default;
    //         }

    //         return prefabRef.LoadAssetAsync<GameObject>();
    //     }

    //     public async System.Threading.Tasks.Task<T> InstantiateAsync(Vector3 position, Quaternion rotation, Transform parent = null)
    //     {
    //         if (!IsValid)
    //         {
    //             Debug.LogError("Invalid prefab reference.");
    //             return null;
    //         }

    //         var handle = Addressables.InstantiateAsync(prefabRef, position, rotation, parent);
    //         await handle.Task;

    //         if (handle.Status != AsyncOperationStatus.Succeeded)
    //         {
    //             Debug.LogError("Failed to instantiate addressable prefab.");
    //             return null;
    //         }

    //         return handle.Result.GetComponent<T>();
    //     }

    //     public void Release()
    //     {
    //         if (loadedPrefab != null)
    //         {
    //             Addressables.ReleaseInstance(loadedPrefab);
    //             loadedPrefab = null;
    //         }
    //     }

    //     public AssetReferenceGameObject Reference => prefabRef;
    // }




}