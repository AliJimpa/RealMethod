#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{

    [CustomPropertyDrawer(typeof(PrefabCore<>), true)]
    public class PrefabDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Get the internal GameObject field from your Prefab<T>
            SerializedProperty prefabGOProp = property.FindPropertyRelative("PrefabAsset");

            if (prefabGOProp == null)
            {
                EditorGUI.LabelField(position, label.text, "Invalid Prefab Field");
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            // Show only GameObjects in the object picker
            Object newObject = EditorGUI.ObjectField(
                position,
                label,
                prefabGOProp.objectReferenceValue,
                typeof(GameObject),
                false
            );

            if (newObject != prefabGOProp.objectReferenceValue)
            {
                if (newObject == null)
                {
                    prefabGOProp.objectReferenceValue = null;
                }
                else if (PrefabUtility.IsPartOfPrefabAsset(newObject))
                {
                    GameObject go = newObject as GameObject;

                    // Get generic type T from Prefab<T>
                    var genericType = GetGenericPrefabType(fieldInfo.FieldType); //fieldInfo.FieldType.GetGenericArguments()[0];
                    if (genericType == null)
                    {
                        EditorGUI.LabelField(position, label.text, "Invalid prefab wrapper.");
                        return;
                    }


                    // Check if prefab has T component
                    if (go.GetComponent(genericType) != null)
                    {
                        prefabGOProp.objectReferenceValue = newObject;
                    }
                    else
                    {
                        Debug.LogWarning($"Selected prefab does not have required component: {genericType.Name}");
                    }
                }
                else
                {
                    Debug.LogWarning("Only prefab assets from the project can be assigned.");
                }
            }

            EditorGUI.EndProperty();
        }


        private System.Type GetGenericPrefabType(System.Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PrefabCore<>))
                {
                    return type.GetGenericArguments()[0];
                }
                type = type.BaseType;
            }
            return null;
        }



    }



}
#endif
