using UnityEngine;

namespace RealMethod
{
    public abstract class BlackboardAsset : UniqueAsset, IBlackboard
    {
        [System.Serializable]
        private class BB_Vector2_Dictionary : SerializableDictionary<string, Vector2> { }
        [System.Serializable]
        private class BB_Vector3_Dictionary : SerializableDictionary<string, Vector3> { }
        [System.Serializable]
        private class BB_Object_Dictionary : SerializableDictionary<string, Object> { }
        [System.Serializable]
        private class BB_Identity_Dictionary : SerializableDictionary<string, IIdentifier> { }
        [Header("Blackboard")]
        [SerializeField, ReadOnly]
        private StringBoolDictionary boolData;
        [SerializeField, ReadOnly]
        private StringByteDictionary byteData;
        [SerializeField, ReadOnly]
        private StringIntDictionary intData;
        [SerializeField, ReadOnly]
        private StringFloatDictionary floatData;
        [SerializeField, ReadOnly]
        private BB_Vector2_Dictionary vector2Data;
        [SerializeField, ReadOnly]
        private BB_Vector3_Dictionary vector3Data;
        [SerializeField, ReadOnly]
        private BB_Object_Dictionary objectData;
        [SerializeField, ReadOnly]
        private StringAssetObjectDictionary assetData;
        [SerializeField, ReadOnly]
        private BB_Identity_Dictionary idData;


        public IBlackboard Provider => this;
        public System.Action<IBlackboard> OnBlackBoardChanged;
        public System.Action<string> OnValueChanged;



        // Implement IBlackboardPure Interface
        public void SetBool(string name, bool target)
        {
            SetValue(boolData, name, target);
        }
        public bool GetBool(string name)
        {
            return GetValue(boolData, name);
        }
        public void SetByte(string name, byte target)
        {
            SetValue(byteData, name, target);
        }
        public byte GetByte(string name)
        {
            return GetValue(byteData, name);
        }
        public void SetInt(string name, int target)
        {
            SetValue(intData, name, target);
        }
        public int GetInt(string name)
        {
            return GetValue(intData, name);
        }
        public void SetFloat(string name, float target)
        {
            SetValue(floatData, name, target);
        }
        public float GetFloat(string name)
        {
            return GetValue(floatData, name);
        }

        // Implement IBlackboardUnity Interface
        public void SetVector2(string name, Vector2 target)
        {
            SetValue(vector2Data, name, target);
        }
        public Vector2 GetVector2(string name)
        {
            return GetValue(vector2Data, name);
        }
        public void SetVector3(string name, Vector3 target)
        {
            SetValue(vector3Data, name, target);
        }
        public Vector3 GetVector3(string name)
        {
            return GetValue(vector3Data, name);
        }
        public void SetObject(string name, Object target)
        {
            SetValue(objectData, name, target);
        }
        public Object GetObject(string name)
        {
            return GetValue(objectData, name);
        }

        // Implement IBlackboard Interface
        public void SetIdentity(string name, IIdentifier target)
        {
            SetValue(idData, name, target);
        }
        public IIdentifier GetIdentity(string name)
        {
            return GetValue(idData, name);
        }
        public void SetAsset(string name, PrimitiveAsset target)
        {
            SetValue(assetData, name, target);
        }
        public PrimitiveAsset GetAsset(string name)
        {
            return GetValue(assetData, name);
        }



        protected void SetValue<TValue>(SerializableDictionary<string, TValue> dic, string name, TValue value)
        {
            dic.Add(name, value);
            OnBlackBoardChanged?.Invoke(Provider);
            OnValueChanged?.Invoke(name);
        }
        protected TValue GetValue<TValue>(SerializableDictionary<string, TValue> dic, string name)
        {
            if (dic.TryGetValue(name, out TValue val))
            {
                return val;
            }
            else
            {
                Debug.LogWarning($"[{name}]: Variable With name '{name}' didn't find in DataBase");
                return default;
            }
        }


#if UNITY_EDITOR
        public override void OnEditorPlay()
        {
            boolData.Clear();
            byteData.Clear();
            intData.Clear();
            floatData.Clear();
            vector2Data.Clear();
            vector3Data.Clear();
            objectData.Clear();
        }
#endif


    }
}
