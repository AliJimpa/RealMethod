using UnityEngine;

namespace RealMethod
{
    public abstract class BlackboardAsset : UniqueAsset, IBlackboard
    {
        public Blackboard Blackboard;

        // Implement IBlackboard Interface
        public bool HasBool(Name16 name) => Blackboard.HasValue<bool>(name);
        public void SetBool(Name16 name, bool target) => Blackboard.SetValue(name, target);
        public bool GetBool(Name16 name) => Blackboard.GetValue<bool>(name);
        public bool HasInt(Name16 name) => Blackboard.HasValue<int>(name);
        public void SetInt(Name16 name, int target) => Blackboard.SetValue(name, target);
        public int GetInt(Name16 name) => Blackboard.GetValue<int>(name);
        public bool HasFloat(Name16 name) => Blackboard.HasValue<float>(name);
        public void SetFloat(Name16 name, float target) => Blackboard.SetValue(name, target);
        public float GetFloat(Name16 name) => Blackboard.GetValue<float>(name);
        public bool HasString(Name16 name) => Blackboard.HasValue<string>(name);
        public void SetString(Name16 name, string target) => Blackboard.SetValue(name, target);
        public string GetString(Name16 name) => Blackboard.GetValue<string>(name);
        public bool HasVector2(Name16 name) => Blackboard.HasValue<Vector2>(name);
        public void SetVector2(Name16 name, Vector2 target) => Blackboard.SetValue(name, target);
        public Vector2 GetVector2(Name16 name) => Blackboard.GetValue<Vector2>(name);
        public bool HasVector3(Name16 name) => Blackboard.HasValue<Vector3>(name);
        public void SetVector3(Name16 name, Vector3 target) => Blackboard.SetValue(name, target);
        public Vector3 GetVector3(Name16 name) => Blackboard.GetValue<Vector3>(name);
        public bool HasPefab(Name16 name) => Blackboard.HasValue<Prefab>(name);
        public void SetPrefab(Name16 name, Prefab target) => Blackboard.SetValue(name, target);
        public Prefab GetPrefab(Name16 name) => Blackboard.GetValue<Prefab>(name);
        public bool HasAsset(Name16 name) => Blackboard.HasValue<PrimitiveAsset>(name);
        public void SetAsset(Name16 name, PrimitiveAsset target) => Blackboard.SetValue(name, target);
        public PrimitiveAsset GetAsset(Name16 name) => Blackboard.GetValue<PrimitiveAsset>(name);
        public bool HasComponent(Name16 name) => Blackboard.HasValue<Component>(name);
        public void SetComponent(Name16 name, Component target) => Blackboard.SetValue(name, target);
        public Component GetComponent(Name16 name) => Blackboard.GetValue<Component>(name);


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