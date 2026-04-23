using System;
using UnityEngine;

namespace RealMethod
{
    public static class Type_Extension
    {
        public static bool IsChild<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }
        public static bool IsChild(this Type type, Type target)
        {
            return target.IsAssignableFrom(type);
        }
        public static object CreateInstance(this Type type)
        {
            if (type.IsChild(typeof(ScriptableObject)))
            {
                return ScriptableObject.CreateInstance(type);
            }
            else
            {
                return Activator.CreateInstance(type);
            }
        }
    }
}