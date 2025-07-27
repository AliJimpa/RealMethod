using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public static class RM_ScriptableObj
    {
        /// <summary>
        /// Creates a new ScriptableObject instance, saves it as an asset in the project, and returns the created asset.
        /// </summary>
        /// <typeparam name="T">The type of ScriptableObject to create.</typeparam>
        /// <param name="path">The path where the asset should be saved (relative to the Assets folder).</param>
        /// <returns>The created ScriptableObject asset.</returns>
        public static T CreateAndSaveAsset<T>(string path) where T : ScriptableObject
        {
            // Create an instance of the ScriptableObject
            T asset = ScriptableObject.CreateInstance<T>();

            // Ensure the directory exists
            string directory = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            // Save the asset
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            // Focus on the newly created asset in the Project window
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            Debug.Log($"ScriptableObject of type {typeof(T).Name} created and saved at: {path}");

            // Return the created asset
            return asset;
        }
    }
}