using System;
using UnityEngine;

namespace RealMethod
{
    public abstract class SoftType
    {
        [SerializeField]
        protected string typeName; // => PropertyRelative
        public Type Type
        {
            get => string.IsNullOrEmpty(typeName) ? null : Type.GetType(typeName);
            set => typeName = value?.AssemblyQualifiedName;
        }



        // Operations
        public static implicit operator Type(SoftType softType)
        {
            return softType?.Type;
        }
        public static bool operator ==(SoftType a, SoftType b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            return a.typeName == b.typeName;
        }
        public static bool operator !=(SoftType a, SoftType b)
        {
            return !(a == b);
        }
        public static bool operator ==(SoftType a, Type b)
        {
            if (a is null || b is null)
                return false;

            return a.typeName == b.AssemblyQualifiedName;
        }
        public static bool operator !=(SoftType a, Type b)
        {
            return !(a == b);
        }
        public static bool operator ==(Type a, SoftType b)
        {
            return b == a;
        }
        public static bool operator !=(Type a, SoftType b)
        {
            return !(b == a);
        }

        // Override Methods
        public override bool Equals(object obj)
        {
            if (obj is SoftType other)
                return typeName == other.typeName;

            return false;
        }
        public override int GetHashCode()
        {
            return typeName?.GetHashCode() ?? 0;
        }
    }

    [Serializable]
    public class SoftType<T> : SoftType where T : class
    {
        // Operations
        public static implicit operator SoftType<T>(Type type)
        {
            if (type != null && !typeof(T).IsAssignableFrom(type))
                throw new InvalidCastException($"{type} is not assignable to {typeof(T)}");

            return new SoftType<T> { typeName = type?.AssemblyQualifiedName };
        }
    }


}