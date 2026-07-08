using UnityEngine;

namespace RealMethod
{
    public interface IBlackboard
    {
        bool HasBool(Name16 name);
        void SetBool(Name16 name, bool target);
        bool GetBool(Name16 name);
        bool HasInt(Name16 name);
        void SetInt(Name16 name, int target);
        int GetInt(Name16 name);
        bool HasFloat(Name16 name);
        void SetFloat(Name16 name, float target);
        float GetFloat(Name16 name);
        bool HasString(Name16 name);
        void SetString(Name16 name, string target);
        string GetString(Name16 name);
        bool HasVector2(Name16 name);
        void SetVector2(Name16 name, Vector2 target);
        Vector2 GetVector2(Name16 name);
        bool HasVector3(Name16 name);
        void SetVector3(Name16 name, Vector3 target);
        Vector3 GetVector3(Name16 name);
        bool HasPefab(Name16 name);
        void SetPrefab(Name16 name, Prefab target);
        Prefab GetPrefab(Name16 name);
        bool HasAsset(Name16 name);
        void SetAsset(Name16 name, PrimitiveAsset target);
        PrimitiveAsset GetAsset(Name16 name);
        bool HasComponent(Name16 name);
        void SetComponent(Name16 name, Component target);
        Component GetComponent(Name16 name);
    }


}