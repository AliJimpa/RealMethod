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
    }
}