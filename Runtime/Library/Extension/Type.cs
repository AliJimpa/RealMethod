using System;

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

    }
}