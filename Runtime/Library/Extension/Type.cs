using System;
using System.Reflection;
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
        public static MethodInfo GetMethodInHierarchy(this Type type, string methodName, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
        {
            while (type != null)
            {
                var method = type.GetMethod(methodName, flags);
                if (method != null)
                    return method;

                type = type.BaseType;
            }

            return null;
        }
        public static bool HasImplementInterface<T>(this Type type)
        {
            var interfaces = type.GetInterfaces();
            foreach (var i in interfaces)
            {
                // Only interfaces derived from Interface
                if (typeof(T).IsAssignableFrom(i))
                    return true;
            }
            return false;
        }

    }
}