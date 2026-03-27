using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace RealMethod
{
    // Abstract Class
    public abstract class PrefabCore : IIdentifier
    {
        [SerializeField]
        protected GameObject PrefabAsset;  // <-- this name must match

        // Implement IIdentidier Interface
        public Name16 NameID => PrefabAsset != null ? PrefabAsset.name : "Empty";

        // Public Functions
        public J GetSoftComponent<J>() where J : Component
        {
            return PrefabAsset.GetComponent<J>();
        }
        public J[] GetSoftComponentsInChildren<J>() where J : Component
        {
            return PrefabAsset.GetComponentsInChildren<J>();
        }
        public bool HasInterface<T>()
        {
            return PrefabAsset.GetComponent<T>() != null;
        }

        // Override Functions
        public override int GetHashCode() => PrefabAsset != null ? PrefabAsset.GetHashCode() : 0;

        // Operator
        public static implicit operator GameObject(PrefabCore prefab)
        {
            return prefab.PrefabAsset;
        }

        // Abstract Methods
        public abstract bool IsValid();
        public abstract System.Type GetMainType(); // <--- added
    }
    // Prefab Class
    [System.Serializable]
    public class PrefabCore<T> : PrefabCore where T : Component
    {
        // PrefabCore Methods
        public override bool IsValid()
        {
            return PrefabAsset != null && PrefabAsset.GetComponent<T>() != null;
        }
        public override System.Type GetMainType() => typeof(T); // <--- implemented

        // Public Method
        public T GetMainComponent()
        {
            return PrefabAsset.GetComponent<T>();
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