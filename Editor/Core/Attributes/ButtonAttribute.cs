using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace RealMethod
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class ButtonAttribute : Attribute { }

    [CustomEditor(typeof(MonoBehaviour), true)]
    public class ButtonDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var targetType = target.GetType();
            var methods = targetType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<ButtonAttribute>() != null && m.GetParameters().Length == 0);

            foreach (var method in methods)
            {
                if (GUILayout.Button(method.Name))
                {
                    method.Invoke(target, null);
                }
            }
        }
    }

}


// [Button]
// private void DoSomething()
// {
//     Debug.Log("DoSomething called from Inspector!");
// }