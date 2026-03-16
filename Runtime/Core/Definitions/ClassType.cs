using System;
using UnityEngine;

namespace RealMethod
{
    public abstract class ClassType
    {
        [SerializeField]
        protected string ClassName;

        protected Type GetClassType()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(ClassName);
                if (type != null)
                    return type;
            }
            return null;
        }
    }

    [Serializable]
    public class ClassType<T> : ClassType where T : class
    {
        // Operations
        public static implicit operator Type(ClassType<T> classType)
        {
            return classType.GetClassType();
        }
        public static implicit operator ClassType<T>(Type type)
        {
            return new ClassType<T> { ClassName = type.FullName };
        }
    }


}