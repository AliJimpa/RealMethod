using UnityEditor;
using UnityEngine;

namespace RealMethod
{
    // Renamed to avoid conflict and inherit from ScriptableObject for Unity asset compatibility
    public class WorldSceneAssetEditorHelper : ScriptableObject
    {
        [InitializeOnLoadMethod]
        private static void OnDoubleClickScriptableObject()
        {
            EditorApplication.projectWindowItemOnGUI += (guid, rect) =>
            {
                Event e = Event.current;
                if (e.type == EventType.MouseDown && e.clickCount == 2)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    var asset = AssetDatabase.LoadAssetAtPath<WorldSceneAsset>(assetPath);

                    if (asset != null)
                    {
                        // Implement OnAssetClick or handle as needed
                        asset.OnAssetClick();
                        e.Use(); // Consume the event
                    }
                }
            };
        }
    }

    [InitializeOnLoad]
    public static class WorldSceneIcon
    {
        static WorldSceneIcon()
        {
            // Automatically sets the custom icon on load
            SetWorldSceneIcon();
        }

        public static void SetWorldSceneIcon()
        {
            // Load the custom icon (ensure the path matches your asset's location)
            Texture2D icon = Resources.Load<Texture2D>("Icons/Core/WorldSceneAsset");
            if (icon == null)
            {
                Debug.LogWarning("Custom icon not found. Please ensure the path and file are correct.");
                return;
            }

            // Find all assets of your custom ScriptableObject type
            string[] guids = AssetDatabase.FindAssets("t:WorldSceneAsset");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object asset = AssetDatabase.LoadAssetAtPath(path, typeof(WorldSceneAsset));

                if (asset != null && icon != null)
{
    // Set the icon
    EditorGUIUtility.SetIconForObject(asset, icon);
}
            }

            // Save changes
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}

