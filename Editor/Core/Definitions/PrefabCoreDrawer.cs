using UnityEditor;
using UnityEngine;
using System;

namespace RealMethod.Editor
{
    [CustomPropertyDrawer(typeof(PrefabCore), true)]
    public class PrefabCore_Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty prefabGOProp = property.FindPropertyRelative("PrefabAsset");

            if (prefabGOProp == null)
            {
                EditorGUI.LabelField(position, label.text, "Cannot serialize generic prefab");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            // Determine label text
            // string displayLabel = prefabGOProp.objectReferenceValue != null ? label.text: $"{label.text} (Select a prefab...)";

            // Draw the ObjectField for GameObject
            UnityEngine.Object newObject = EditorGUI.ObjectField(
                position,
                label,
                prefabGOProp.objectReferenceValue,
                typeof(GameObject),
                false
            );

            // Only update if changed
            if (newObject != prefabGOProp.objectReferenceValue)
            {
                if (newObject == null)
                {
                    prefabGOProp.objectReferenceValue = null;
                }
                else if (PrefabUtility.IsPartOfPrefabAsset(newObject))
                {
                    // Get the concrete PrefabCore instance
                    PrefabCore targetPrefab = fieldInfo.GetValue(property.serializedObject.targetObject) as PrefabCore;

                    if (targetPrefab == null)
                    {
                        Debug.LogWarning("Invalid prefab wrapper.");
                        return;
                    }

                    var targetClass = targetPrefab.GetTargetClass();
                    GameObject go = newObject as GameObject;

                    // Type-check: the prefab must have the required component
                    if (targetClass != null && go.GetComponent(targetClass) != null)
                    {
                        prefabGOProp.objectReferenceValue = newObject;
                    }
                    else
                    {
                        Debug.LogWarning($"Selected prefab does not have required component: {targetClass?.Name}");
                    }
                }
                else
                {
                    Debug.LogWarning("Only prefab assets from the project can be assigned.");
                }
            }

            // --- Set GUI color to blue to indicate change ---
            Color prevColor = GUI.color;
            GUI.color = Color.cyan; // light blue
            EditorGUI.ObjectField(position, label, prefabGOProp.objectReferenceValue, typeof(GameObject), false);
            GUI.color = prevColor;
            EditorGUI.EndProperty();
        }




        // --- Utility to get the actual object instance from SerializedProperty ---
        // private object GetTargetObjectOfProperty(SerializedProperty prop)
        // {
        //     if (prop == null) return null;

        //     string path = prop.propertyPath.Replace(".Array.data[", "[");
        //     object obj = prop.serializedObject.targetObject;
        //     string[] elements = path.Split('.');

        //     foreach (string element in elements)
        //     {
        //         if (element.Contains("["))
        //         {
        //             string elementName = element.Substring(0, element.IndexOf("["));
        //             int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
        //             obj = GetValue(obj, elementName, index);
        //         }
        //         else
        //         {
        //             obj = GetValue(obj, element);
        //         }
        //     }
        //     return obj;
        // }
        // private object GetValue(object source, string name)
        // {
        //     if (source == null) return null;
        //     var type = source.GetType();
        //     var f = type.GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        //     if (f == null) return null;
        //     return f.GetValue(source);
        // }
        // private object GetValue(object source, string name, int index)
        // {
        //     var enumerable = GetValue(source, name) as System.Collections.IEnumerable;
        //     if (enumerable == null) return null;
        //     var enm = enumerable.GetEnumerator();
        //     for (int i = 0; i <= index; i++)
        //     {
        //         if (!enm.MoveNext()) return null;
        //     }
        //     return enm.Current;
        // }
    }
}
