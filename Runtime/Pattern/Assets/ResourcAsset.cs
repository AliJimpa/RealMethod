using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace RealMethod
{
    public abstract class ResourcAsset : DataAsset
    {
        public System.Action<ResourcAsset> OnloadAsset;

        // Abstract Methods
        public abstract bool IsValid();
        public abstract bool IsLoaded();
        public abstract T GetAsset<T>() where T : Object;
        public abstract bool Load();
        public abstract Task LoadAsync();
        public abstract void LoadAsync<T>(MonoBehaviour Owner, System.Action<T> loaded) where T : Object;
        public abstract bool Unload();
    }
    public abstract class ResourcAsset<T> : ResourcAsset where T : Object
    {
        [ReadOnly]
        public string ResourcePath;
        private string AssetPath => ResourcePath.StartsWith("Resources/") ? ResourcePath.Substring("Resources/".Length) : ResourcePath;
        private T Asset;


        // ResourcAsset Methods
        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(ResourcePath);
        }
        public override bool IsLoaded()
        {
            return Asset != null;
        }
        public override U GetAsset<U>()
        {
            if (!IsLoaded())
            {
                if (!Load())
                {
                    return null;
                }
            }
            return Asset as U;
        }
        public override bool Load()
        {
            if (IsLoaded())
            {
                Debug.LogError("That Asset is already loaded");
                return false;
            }

            if (!IsValid())
            {
                Debug.LogError("Asset path is not valid!");
                return false;
            }

            if (Asset == null)
            {
                Asset = Resources.Load<T>(AssetPath);
                if (Asset == null)
                {
                    Debug.LogError($"Asset not found at Resources/{AssetPath}");
                    return false;
                }
            }
            OnloadAsset?.Invoke(this);
            return true;
        }
        public override async Task LoadAsync()
        {
            if (IsLoaded())
            {
                Debug.LogError("That Asset is already loaded");
                return;
            }

            if (!IsValid())
            {
                Debug.LogError("Asset path is not valid!");
                return;
            }

            var request = Resources.LoadAsync<T>(AssetPath);
            while (!request.isDone)
                await Task.Yield(); // Wait until finished

            Asset = request.asset as T;
            if (Asset == null)
            {
                Debug.LogError($"Asset not found at Resources/{AssetPath}");
                return;
            }

            OnloadAsset?.Invoke(this);
        }
        public override void LoadAsync<U>(MonoBehaviour Owner, System.Action<U> loaded)
        {
            if (Owner != null)
            {
                Owner.StartCoroutine(LoadRoutine(obj => loaded?.Invoke(obj as U)));
            }
            else
            {
                Debug.LogError("Owner is not valid!");
            }
        }
        public override bool Unload()
        {
            if (IsLoaded())
            {
                Resources.UnloadAsset(Asset);
                Asset = null;
                return true;
            }
            return false;
        }

        // IEnumerator Methods
        private IEnumerator LoadRoutine(System.Action<T> result)
        {
            if (IsLoaded())
            {
                Debug.LogError("That Asset is already loaded");
                yield break;
            }

            ResourceRequest request = Resources.LoadAsync<T>(AssetPath);
            yield return request;

            Asset = request.asset as T;
            if (Asset == null)
            {
                Debug.LogError($"Asset not found at Resources/{AssetPath}");
                yield break;
            }

            OnloadAsset?.Invoke(this);
            result?.Invoke(Asset);
        }


#if UNITY_EDITOR
        public override void OnEditorPlay()
        {
            Asset = null;
        }
#endif

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