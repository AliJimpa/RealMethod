
namespace RealMethod
{
    public interface IBlackboardPure
    {
        void SetBool(string name, bool target);
        bool GetBool(string name);
        void SetByte(string name, byte target);
        byte GetByte(string name);
        void SetInt(string name, int target);
        int GetInt(string name);
        void SetFloat(string name, float target);
        float GetFloat(string name);
    }
    public interface IBlackboardUnity : IBlackboardPure
    {
        void SetVector2(string name, UnityEngine.Vector2 target);
        UnityEngine.Vector2 GetVector2(string name);
        void SetVector3(string name, UnityEngine.Vector3 target);
        UnityEngine.Vector3 GetVector3(string name);
        void SetObject(string name, UnityEngine.Object target);
        UnityEngine.Object GetObject(string name);
    }
    public interface IBlackboard : IBlackboardUnity
    {
        void SetIdentity(string name, IIdentifier target);
        IIdentifier GetIdentity(string name);
        void SetAsset(string name, PrimitiveAsset target);
        PrimitiveAsset GetAsset(string name);

    }
}