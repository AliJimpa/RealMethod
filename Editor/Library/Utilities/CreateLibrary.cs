using UnityEditor;
using UnityEngine;
using System.IO;

namespace RealMethod.Editor
{
    public static class RM_Create
    {
        public static void GameObject<T>(string Name = "GameObject") where T : Component
        {
            GameObject instance = new GameObject(Name);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(instance, "Create " + instance.name);
            instance.AddComponent<T>();
            // Select the newly created instance
            Selection.activeObject = instance;
        }
        public static T Asset<T>(string path) where T : PrimitiveAsset
        {
            // Create an instance of the PrimitiveAsset
            T asset = ScriptableObject.CreateInstance<T>();

            // Ensure the directory exists
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Save the asset
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            // Focus on the newly created asset in the Project window
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            Debug.Log($"PrimitiveAsset of type {typeof(T).Name} created and saved at: {path}");

            // Return the created asset
            return asset;
        }
    }
}

