using UnityEditor;
using UnityEngine;

namespace RealMethod
{
    [InitializeOnLoad]
    public static class PCGAssetIcon
    {
        static PCGAssetIcon()
        {
            // Automatically sets the custom icon on load
            SetPCGAssetIcon();
        }

        public static void SetPCGAssetIcon()
        {
            // Load the custom icon (ensure the path matches your asset's location)
            Texture2D icon = Resources.Load<Texture2D>("Icons/Toolkit/PCGResource");

            if (icon == null)
            {
                Debug.LogWarning("Custom icon not found. Please ensure the path and file are correct.");
                return;
            }

            // Find all assets of your custom ScriptableObject type
            string[] guids = AssetDatabase.FindAssets("t:PCGResourceAsset");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object asset = AssetDatabase.LoadAssetAtPath(path, typeof(TableAsset));

                               if (asset != null && icon != null)
{
    // Set the icon
    EditorGUIUtility.SetIconForObject(asset, icon);
}
                Debug.Log($"Icon applied to {asset.name}");
            }
            Debug.Log($"Total assets processed: {guids.Length}");

            // Save changes
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}