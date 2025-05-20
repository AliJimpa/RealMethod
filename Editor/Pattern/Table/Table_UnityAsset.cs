using UnityEditor;
using UnityEngine;


namespace RealMethod
{
    public class Table_UnityAsset : AssetPostprocessor
    {
        [InitializeOnLoadMethod]
        private static void OnDoubleClickScriptableObject()
        {
            // Remove previous handler to avoid duplicates
            EditorApplication.projectWindowItemOnGUI -= HandleProjectWindowItemOnGUI;
            EditorApplication.projectWindowItemOnGUI += HandleProjectWindowItemOnGUI;
        }

        private static void HandleProjectWindowItemOnGUI(string guid, Rect rect)
        {
            Event e = Event.current;
            // Only handle left mouse double-clicks inside the asset's rect
            if (rect.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.clickCount == 2 && e.button == 0)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<TableAsset>(assetPath);

                if (asset != null)
                {
                    if (TableViewerWindow.IsOpenWindow())
                    {
                        TableViewerWindow.CloseWindow();
                    }
                    TableViewerWindow.OpenWindow(asset);
                    e.Use(); // Consume the event
                }
            }
        }
    }
    [InitializeOnLoad]
    public static class TableAssetIcon
    {
        static TableAssetIcon()
        {
            // Automatically sets the custom icon on load
            SetAssetIcon();
        }

        public static void SetAssetIcon()
        {
            // Load the custom icon (ensure the path matches your asset's location)
            Texture2D icon = Resources.Load<Texture2D>("Icons/TableAsset");
            //Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.mustard.realmetho/Resources/Icons/TableAsset.png");

            if (icon == null)
            {
                Debug.LogWarning("Custom icon not found. Please ensure the path and file are correct.");
                return;
            }

            // Find all assets of your custom ScriptableObject type
            string[] guids = AssetDatabase.FindAssets("t:TableAsset");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object asset = AssetDatabase.LoadAssetAtPath(path, typeof(TableAsset));

                // Set the icon
                EditorGUIUtility.SetIconForObject(asset, icon);
                Debug.Log($"Icon applied to {asset.name}");
            }

            // Save changes
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
