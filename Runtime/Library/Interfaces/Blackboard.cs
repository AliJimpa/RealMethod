using UnityEngine;

namespace RealMethod
{
    public interface IBlackboard
    {
        bool HasBool(FName name);
        void SetBool(FName name, bool target);
        bool GetBool(FName name);
        bool HasInt(FName name);
        void SetInt(FName name, int target);
        int GetInt(FName name);
        bool HasFloat(FName name);
        void SetFloat(FName name, float target);
        float GetFloat(FName name);
        bool HasString(FName name);
        void SetString(FName name, string target);
        string GetString(FName name);
        bool HasVector2(FName name);
        void SetVector2(FName name, Vector2 target);
        Vector2 GetVector2(FName name);
        bool HasVector3(FName name);
        void SetVector3(FName name, Vector3 target);
        Vector3 GetVector3(FName name);
        bool HasPefab(FName name);
        void SetPrefab(FName name, Prefab target);
        Prefab GetPrefab(FName name);
        bool HasAsset(FName name);
        void SetAsset(FName name, PrimitiveAsset target);
        PrimitiveAsset GetAsset(FName name);
        bool HasComponent(FName name);
        void SetComponent(FName name, Component target);
        Component GetComponent(FName name);
    }


}