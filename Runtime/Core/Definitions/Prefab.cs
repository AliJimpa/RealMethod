using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace RealMethod
{
    // Abstract Class
    public abstract class PrefabCore : IIdentifier
    {
        [SerializeField]
        private GameObject PrefabAsset;  // <-- this name must match
        public GameObject asset => PrefabAsset;
        public string NameID => PrefabAsset != null ? PrefabAsset.name : "Empty";

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
        public abstract System.Type GetTargetClass(); // <--- added
    }
    // Prefab Class
    [System.Serializable]
    public class PrefabCore<T> : PrefabCore where T : Component
    {
        // PrefabCore Methods
        public override bool IsValid()
        {
            return asset != null && asset.GetComponent<T>() != null;
        }
        public override System.Type GetTargetClass() => typeof(T); // <--- implemented

        // Public Method
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
    public class PSPrefab : PrefabCore<ParticleSystem>
    {

    }
    [System.Serializable]
    public class VEPrefab : PrefabCore<VisualEffect>
    {

    }
    [System.Serializable]
    public class APrefab : PrefabCore<AudioBehaviour>
    {

    }


}