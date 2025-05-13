using UnityEngine;
using UnityEditor;
using System;
using System.Linq;


namespace RealMethod
{
    //[InitializeOnLoad]
    public static class AutoWorldExecutionOrder
    {
        [MenuItem("Tools/RealMethod/Core/ExecutionOrder")]
        static void ApplyExecutionOrder()
        {
            SetExecutionOrderForDerivedTypes(typeof(World), -18); // ← change base class and order here
        }

        private static void SetExecutionOrderForDerivedTypes(Type baseType, int order)
        {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    try { return assembly.GetTypes(); }
                    catch { return Array.Empty<Type>(); }
                })
                .Where(type =>
                    type.IsClass &&
                    !type.IsAbstract &&
                    baseType.IsAssignableFrom(type) &&
                    typeof(MonoBehaviour).IsAssignableFrom(type)
                );

            MonoScript[] scripts = (MonoScript[])Resources.FindObjectsOfTypeAll(typeof(MonoScript));

            foreach (var type in allTypes)
            {
                MonoScript script = scripts.FirstOrDefault(s => s != null && s.GetClass() == type);
                if (script == null) continue;

                int currentOrder = MonoImporter.GetExecutionOrder(script);
                if (currentOrder != order)
                {
                    MonoImporter.SetExecutionOrder(script, order);
                    Debug.Log($"[AutoOrder] Set execution order of {type.Name} to {order}");
                }
            }



        }
    }
}