using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace RealMethod
{
    // Abstract Class
    public abstract class PrefabCore
    {
        [SerializeField]
        private GameObject PrefabAsset;  // <-- this name must match
        public GameObject asset => PrefabAsset;
        public string Name => PrefabAsset.name;

        // Public Functions
        public J GetSoftClass<J>() where J : Component
        {
            return PrefabAsset.GetComponent<J>();
        }
        public J[] GetAllSoftClass<J>() where J : Component
        {
            return PrefabAsset.GetComponentsInChildren<J>();
        }
        public bool HasInterface<T>()
        {
            return PrefabAsset.GetComponent<T>() != null;
        }

        // Abstract Methods
        public abstract bool IsValid();
    }
    // Prefab Class
    [System.Serializable]
    public class PrefabCore<T> : PrefabCore where T : Component
    {
        // Prefab Methods
        public override bool IsValid()
        {
            return asset != null && asset.GetComponent<T>() != null;
        }

        public T GetSoftClassTarget()
        {
            return asset.GetComponent<T>();
        }
    }


    // Sample
    [System.Serializable]
    public class Prefab : PrefabCore<Transform>
    {

    }
    [System.Serializable]
    public class UPrefab : PrefabCore<RectTransform>
    {

    }
    [System.Serializable]
    public class UKPrefab : PrefabCore<UIDocument>
    {

    }
    [System.Serializable]
    public class VPrefab : PrefabCore<ParticleSystem>
    {

    }
    [System.Serializable]
    public class VEPrefab : PrefabCore<VisualEffect>
    {

    }
    [System.Serializable]
    public class SPrefab : PrefabCore<AudioBehaviour>
    {

    }


}