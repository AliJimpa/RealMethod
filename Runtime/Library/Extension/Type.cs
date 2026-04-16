using System;

namespace RealMethod
{
    public static class Type_Extension
    {
        public static bool IsChild<T>(this Type type) where T : Type
        {
            return type.IsAssignableFrom(typeof(T));
        }
        public static bool IsChild(this Type type, Type target)
        {
            return type.IsAssignableFrom(target);
        }
        public static bool IsChild<T>(this SoftType soft) where T : Type
        {
            return soft.Type.IsAssignableFrom(typeof(T));
        }
        public static bool IsChild(this SoftType soft, SoftType target)
        {
            return soft.Type.IsAssignableFrom(target);
        }
    }
}