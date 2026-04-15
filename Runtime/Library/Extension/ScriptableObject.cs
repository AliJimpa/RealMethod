using System.Reflection;
using UnityEngine;

namespace RealMethod
{
    public static class ScriptableObject_Extension
    {
        public static void Invoke(this ScriptableObject so, string methodName)
        {
            so.Invoke(methodName, null);
        }
        public static void Invoke(this ScriptableObject so, string methodName, object[] parameters)
        {
            var method = so.GetType().GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (method != null)
            {
                method.Invoke(so, parameters);
            }
            else
            {
                Debug.LogError($"Method '{methodName}' not found.");
            }
        }
    
    }
}