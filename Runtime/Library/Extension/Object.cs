using UnityEngine;

namespace RealMethod
{
    public static class Object_Extension
    {
        public static bool NullChecker(this Object Obj, string Name)
        {
            if (Obj)
            {
                return true;
            }
            else
            {
                Debug.LogError($"Target Object [{Name}] Is Not Valid");
                return false;
            }
        }
        public static T Cast<T>(this object obj) where T : class
        {
            return (T)obj;
        }
        public static bool TryCast<T>(this object obj, out T result) where T : class
        {
            if (obj is T refrence)
            {
                result = refrence;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
        public static SoftType<T> GetSoftType<T>(this object obj) where T : System.Type
        {
            return obj.GetType();
        }
        public static bool TryGetType<T>(this object obj, out SoftType<T> result) where T : System.Type
        {
            System.Type targetType = obj.GetType();
            if (targetType.IsAssignableFrom(typeof(T)))
            {
                result = obj.GetType();
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }
}