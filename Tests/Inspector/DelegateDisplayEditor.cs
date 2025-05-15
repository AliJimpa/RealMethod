using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace RealMethod
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class DelegateDisplayEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MonoBehaviour targetMono = (MonoBehaviour)target;
            Type type = targetMono.GetType();

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var fields = type.GetFields(flags);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Assigned Delegates", EditorStyles.boldLabel);

            bool anyShown = false;

            foreach (var field in fields)
            {
                // Check if it's a delegate or derived from MulticastDelegate (includes Action, Func, etc.)
                if (typeof(MulticastDelegate).IsAssignableFrom(field.FieldType.BaseType))
                {
                    var del = field.GetValue(targetMono) as Delegate;

                    if (del != null)
                    {
                        Delegate[] invocationList = del.GetInvocationList();

                        foreach (var d in invocationList)
                        {
                            string methodName = d.Method?.Name ?? "<null>";
                            string targetName = d.Target?.GetType().Name ?? "<no target>";

                            EditorGUILayout.LabelField($"{field.Name}", $"{methodName} ({targetName})");
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField($"{field.Name}", "<null>");
                    }

                    anyShown = true;
                }
            }

            if (!anyShown)
            {
                EditorGUILayout.LabelField("No delegates found.");
            }

            Debug.Log("sdsdsd");
        }


    }
}