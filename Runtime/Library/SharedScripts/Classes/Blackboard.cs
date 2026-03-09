using System;
using UnityEngine;

namespace RealMethod
{
    [Serializable]
    public struct Blackboard
    {
        private enum ValueType
        {
            None = 0,
            Bool = 1,
            Int = 2,
            Float = 3,
            String = 4,
            Vector2 = 5,
            Vector3 = 6,
            Prefab = 7,
            Asset = 8,
            Component = 10,
        }
        [Serializable]
        private struct ValuePointer
        {
            [field: SerializeField]
            public Hash128 Name { get; private set; }
            [field: SerializeField]
            public ValueType Type { get; private set; }
            [field: SerializeField]
            public int Index { get; private set; }
            public bool IsValid => Type != ValueType.None;

            public ValuePointer(FName Vname, ValueType Vtype, int Vindex = -1)
            {
                if (Vtype != ValueType.None)
                {
                    Name = Hash128.Compute(Vname);
                    Type = Vtype;
                    Index = Vindex;
                }
                else
                {
                    Debug.LogWarning("Value didnt store in Blackboard,Type is None");
                    Name = Hash128.Compute(string.Empty);
                    Type = ValueType.None;
                    Index = -1;
                }
            }
            public void Clear()
            {
                Name = Hash128.Compute(string.Empty);
                Type = ValueType.None;
                Index = -1;
            }
        }

        [SerializeField, ReadOnly]
        private ValuePointer[] Variables;
        [SerializeField, ReadOnly]
        private bool[] Boolean;
        [SerializeField, ReadOnly]
        private int[] Intiger;
        [SerializeField, ReadOnly]
        private float[] Float;
        [SerializeField, ReadOnly]
        private string[] String;
        [SerializeField, ReadOnly]
        private Vector2[] Vector2;
        [SerializeField, ReadOnly]
        private Vector3[] Vector3;
        [SerializeField, ReadOnly]
        private Prefab[] Prefab;
        [SerializeField, ReadOnly]
        private PrimitiveAsset[] Asset;
        [SerializeField, ReadOnly]
        private Component[] Component;
        public int Length
        {
            get
            {
                int result = 0;
                foreach (var item in Variables)
                {
                    if (item.IsValid)
                    {
                        result++;
                    }
                }
                return result;
            }
        }
        public int MaxSize
        {
            get
            {
                return Boolean.Length;
            }
        }

        public Blackboard(int size = 10)
        {
            int TypesCount = Enum.GetValues(typeof(ValueType)).Length - 1;
            Variables = new ValuePointer[size * TypesCount];
            Boolean = new bool[size];
            Intiger = new int[size];
            Float = new float[size];
            String = new string[size];
            Vector2 = new Vector2[size];
            Vector3 = new Vector3[size];
            Prefab = new Prefab[size];
            Asset = new PrimitiveAsset[size];
            Component = new Component[size];
        }

        public bool HasValue<T>(FName VariableName)
        {
            return TryFindPointer<T>(VariableName, out ValuePointer outher);
        }
        public T GetValue<T>(FName VariableName)
        {
            if (TryFindPointer<T>(VariableName, out ValuePointer item))
            {
                object result = item.Type switch
                {
                    ValueType.Bool => Boolean[item.Index],
                    ValueType.Int => Intiger[item.Index],
                    ValueType.Float => Float[item.Index],
                    ValueType.String => String[item.Index],
                    ValueType.Vector2 => Vector2[item.Index],
                    ValueType.Vector3 => Vector3[item.Index],
                    ValueType.Prefab => Prefab[item.Index],
                    ValueType.Asset => Asset[item.Index],
                    ValueType.Component => Component[item.Index],
                    _ => null
                };

                if (result is T finalValue)
                    return finalValue;

                throw new InvalidCastException($"Variable '{VariableName}' is type {item.Type} but requested {typeof(T)}");
            }

            Debug.LogWarning($"Variable '{VariableName}' not found.");
            return default;
        }
        public void SetValue<T>(FName VariableName, T value)
        {
            if (TryFindPointer<T>(VariableName, out ValuePointer item))
            {
                SetValue(value, item.Index);
            }
            else
            {
                CreateValue(VariableName, value);
            }
        }
        public bool CreateValue<T>(FName VariableName, T DefaultValue)
        {
            ValueType TargetType = ConvertType<T>();
            int TypeIndex = GetLengthByType(TargetType);
            if (TypeIndex < MaxSize - 1)
            {
                if (SetValue(DefaultValue, TypeIndex))
                {
                    Variables[Length] = new ValuePointer(VariableName, TargetType, TypeIndex);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Debug.LogWarning($"Blackboard for Variable of {typeof(T)} is full");
                return false;
            }
        }
        public bool RemoveValue<T>(FName VariableName)
        {
            if (TryFindPointer<T>(VariableName, out ValuePointer item))
            {
                item.Clear();
                return true;
            }
            return false;
        }
        public void SetSize(int size)
        {
            int TypesCount = Enum.GetValues(typeof(ValueType)).Length - 1;
            if (Variables == null || Variables.Length == 0) Variables = new ValuePointer[size * TypesCount];
            if (Boolean == null || Boolean.Length == 0) Boolean = new bool[size];
            if (Intiger == null || Intiger.Length == 0) Intiger = new int[size];
            if (Float == null || Float.Length == 0) Float = new float[size];
            if (String == null || String.Length == 0) String = new string[size];
            if (Vector2 == null || Vector2.Length == 0) Vector2 = new Vector2[size];
            if (Vector3 == null || Vector3.Length == 0) Vector3 = new Vector3[size];
            if (Prefab == null || Prefab.Length == 0) Prefab = new Prefab[size];
            if (Asset == null || Asset.Length == 0) Asset = new PrimitiveAsset[size];
            if (Component == null || Component.Length == 0) Component = new Component[size];
        }
        public void Clear()
        {
            SetSize(MaxSize);
        }

        private bool TryFindPointer<T>(FName VariableName, out ValuePointer pointer)
        {
            Hash128 TargetName = Hash128.Compute(VariableName);
            ValueType TargetType = ConvertType<T>();
            foreach (var item in Variables)
            {
                if (item.Type == TargetType)
                {
                    if (item.Name == TargetName)
                    {
                        pointer = item;
                        return true;
                    }
                }
            }
            pointer = new ValuePointer();
            return false;
        }
        private int GetLengthByType(ValueType TargetType)
        {
            int result = 0;
            foreach (var item in Variables)
            {
                if (item.Type == TargetType)
                {
                    if (item.Index > result)
                    {
                        result = item.Index;
                    }
                    else
                    {
                        if (item.Index == result)
                            result++;
                    }
                }
            }
            return result;
        }
        private ValueType ConvertType<T>()
        {
            if (typeof(T) == typeof(bool)) { return ValueType.Bool; }
            else if (typeof(T) == typeof(int)) { return ValueType.Int; }
            else if (typeof(T) == typeof(float)) { return ValueType.Float; }
            else if (typeof(T) == typeof(string)) { return ValueType.String; }
            else if (typeof(T) == typeof(Vector2)) { return ValueType.Vector2; }
            else if (typeof(T) == typeof(Vector3)) { return ValueType.Vector3; }
            else if (typeof(T) == typeof(Prefab)) { return ValueType.Prefab; }
            else if (typeof(T) == typeof(PrimitiveAsset)) { return ValueType.Asset; }
            else if (typeof(T) == typeof(Component)) { return ValueType.Component; }
            else { return ValueType.None; }
        }
        private bool SetValue<T>(T value, int index)
        {
            if (index >= MaxSize)
            {
                Debug.LogError($"For this index {index} of {typeof(T)} didn't have any slot");
                return false;
            }
            switch (value)
            {
                case bool v:
                    Boolean[index] = v;
                    return true;
                case int v:
                    Intiger[index] = v;
                    return true;
                case float v:
                    Float[index] = v;
                    return true;
                case string v:
                    String[index] = v;
                    return true;
                case Vector2 v:
                    Vector2[index] = v;
                    return true;
                case Vector3 v:
                    Vector3[index] = v;
                    return true;
                case Prefab v:
                    Prefab[index] = v;
                    return true;
                case PrimitiveAsset v:
                    Asset[index] = v;
                    return true;
                case Component v:
                    Component[index] = v;
                    return true;
                default:
                    Debug.LogError($"Type {typeof(T)} is not supported.");
                    return false;
            }
        }
    }

}