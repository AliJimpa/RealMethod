using UnityEngine;

namespace RealMethod
{
    public abstract class BlackboardAsset : UniqueAsset, IBlackboard
    {
        public Blackboard Blackboard;

        // Implement IBlackboard Interface
        public bool HasBool(FName name) => Blackboard.HasValue<bool>(name);
        public void SetBool(FName name, bool target) => Blackboard.SetValue(name, target);
        public bool GetBool(FName name) => Blackboard.GetValue<bool>(name);
        public bool HasInt(FName name) => Blackboard.HasValue<int>(name);
        public void SetInt(FName name, int target) => Blackboard.SetValue(name, target);
        public int GetInt(FName name) => Blackboard.GetValue<int>(name);
        public bool HasFloat(FName name) => Blackboard.HasValue<float>(name);
        public void SetFloat(FName name, float target) => Blackboard.SetValue(name, target);
        public float GetFloat(FName name) => Blackboard.GetValue<float>(name);
        public bool HasString(FName name) => Blackboard.HasValue<string>(name);
        public void SetString(FName name, string target) => Blackboard.SetValue(name, target);
        public string GetString(FName name) => Blackboard.GetValue<string>(name);
        public bool HasVector2(FName name) => Blackboard.HasValue<Vector2>(name);
        public void SetVector2(FName name, Vector2 target) => Blackboard.SetValue(name, target);
        public Vector2 GetVector2(FName name) => Blackboard.GetValue<Vector2>(name);
        public bool HasVector3(FName name) => Blackboard.HasValue<Vector3>(name);
        public void SetVector3(FName name, Vector3 target) => Blackboard.SetValue(name, target);
        public Vector3 GetVector3(FName name) => Blackboard.GetValue<Vector3>(name);
        public bool HasPefab(FName name) => Blackboard.HasValue<Prefab>(name);
        public void SetPrefab(FName name, Prefab target) => Blackboard.SetValue(name, target);
        public Prefab GetPrefab(FName name) => Blackboard.GetValue<Prefab>(name);
        public bool HasAsset(FName name) => Blackboard.HasValue<PrimitiveAsset>(name);
        public void SetAsset(FName name, PrimitiveAsset target) => Blackboard.SetValue(name, target);
        public PrimitiveAsset GetAsset(FName name) => Blackboard.GetValue<PrimitiveAsset>(name);
        public bool HasComponent(FName name) => Blackboard.HasValue<Component>(name);
        public void SetComponent(FName name, Component target) => Blackboard.SetValue(name, target);
        public Component GetComponent(FName name) => Blackboard.GetValue<Component>(name);


        protected override void OnEnable()
        {
            Blackboard.SetSize(GetMaxSize());
        }

        [ContextMenu("Resize")]
        private void OnResetSize()
        {
            Blackboard = new Blackboard(GetMaxSize());
        }

        // Abstract Methods
        protected abstract int GetMaxSize();
    }
}