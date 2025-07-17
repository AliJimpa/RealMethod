using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace RealMethod
{
    // Abstract Class
    public abstract class PrefabCore
    {
        // Abstract Methods
        public abstract bool IsValid();
        public abstract GameObject GetPrefab();
    }
    public abstract class Prefab : PrefabCore
    {
        [SerializeField]
        protected GameObject PrefabAsset;  // <-- this name must match

        // PrefabCore Method
        public override GameObject GetPrefab()
        {
            return PrefabAsset;
        }

        // Public Functions
        public J GetSoftClass<J>() where J : Component
        {
            return PrefabAsset.GetComponent<J>();
        }
        public J[] GetAllSoftClass<J>() where J : Component
        {
            return PrefabAsset.GetComponentsInChildren<J>();
        }

    }



    [System.Serializable]
    public class Prefab<T> : Prefab where T : Component
    {
        // PrefabCore Methods
        public override bool IsValid()
        {
            return PrefabAsset != null && PrefabAsset.GetComponent<T>() != null;
        }


        public T Instantiate(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (!IsValid())
            {
                Debug.LogError("Trying to instantiate an invalid prefab.");
                return null;
            }

            GameObject instance = Object.Instantiate(PrefabAsset, position, rotation, parent);
            return instance.GetComponent<T>();
        }
    }

    [System.Serializable]
    public class ResourcePrefab : PrefabCore
    {
        [SerializeField]
        protected string resourcePath;
        public string path => resourcePath;

        private GameObject loadedAsset;
        public bool isLoaded => loadedAsset != null;
        private int pointer = 0;

        public System.Action<PrefabCore> Onloaded;

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(resourcePath);
        }
        public override GameObject GetPrefab()
        {
            if (!isLoaded)
            {
                if (!Load())
                {
                    return null;
                }
            }
            pointer++;
            return loadedAsset;
        }

        // Public Functions
        public bool Load()
        {
            if (isLoaded)
            {
                Debug.LogError("That prefab is already loaded");
                return false;
            }

            if (!IsValid())
            {
                Debug.LogError("Prefab path is not set.");
                return false;
            }

            if (loadedAsset == null)
            {
                loadedAsset = Resources.Load<GameObject>(resourcePath);
            }

            if (loadedAsset == null)
            {
                Debug.LogError($"Prefab not found at Resources/{resourcePath}");
                return false;
            }

            Onloaded?.Invoke(this);
            return true;
        }
        public async Task LoadAsync()
        {
            if (isLoaded)
            {
                Debug.LogError("That prefab is already loaded");
                return;
            }

            if (!IsValid())
            {
                Debug.LogError("Invalid resource path.");
                return;
            }

            var request = Resources.LoadAsync<GameObject>(resourcePath);
            while (!request.isDone)
                await Task.Yield(); // Wait until finished

            loadedAsset = request.asset as GameObject;
            if (loadedAsset == null)
            {
                Debug.LogError($"Failed to load prefab at path: {resourcePath}");
                return;
            }

            Onloaded?.Invoke(this);
        }
        public void LoadAsync(MonoBehaviour Owner, System.Action<PrefabCore> loaded)
        {
            if (Owner != null)
            {
                Owner.StartCoroutine(LoadRoutine(loaded));
            }
            else
            {
                Debug.LogError("Owner is not valid!");
            }
        }
        public bool Unload(bool Force = false)
        {
            if (isLoaded)
            {
                if (pointer == 0 || Force)
                {
                    Resources.UnloadAsset(loadedAsset);
                    loadedAsset = null;
                    return true;
                }
                else
                {
                    Debug.LogWarning($"{this} For this prefab [{loadedAsset}] you use ({pointer}) Instance");
                    return false;
                }
            }
            else
            {
                Debug.LogWarning("First Load Prefab!");
                return false;
            }
        }
        public bool Destroy(GameObject instance)
        {
            if (instance != null)
            {
                Destroy(instance);
                pointer--;
                return true;
            }
            else
            {
                return false;
            }
        }

        public J GetSoftClass<J>() where J : Component
        {
            if (loadedAsset != null)
            {
                return loadedAsset.GetComponent<J>();
            }
            else
            {
                Debug.LogWarning("First Load Prefab!");
                return null;
            }
        }
        public J[] GetAllSoftClass<J>() where J : Component
        {
            if (loadedAsset != null)
            {
                return loadedAsset.GetComponentsInChildren<J>();
            }
            else
            {
                Debug.LogWarning("First Load Prefab!");
                return null;
            }
        }

        private IEnumerator LoadRoutine(System.Action<PrefabCore> result)
        {
            if (isLoaded)
            {
                Debug.LogError("That prefab is already loaded");
                yield break;
            }

            ResourceRequest request = Resources.LoadAsync<GameObject>(resourcePath);
            yield return request;

            loadedAsset = request.asset as GameObject;
            if (loadedAsset == null)
            {
                Debug.LogError($"Failed to load prefab at path: {resourcePath}");
                yield break;
            }

            Onloaded?.Invoke(this);
            result?.Invoke(this);
        }
    }



    [System.Serializable]
    public class UPrefab : Prefab<Transform>
    {

    }
}