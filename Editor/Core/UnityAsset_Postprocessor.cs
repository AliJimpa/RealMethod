using UnityEditor;
using UnityEngine;

namespace RealMethod
{
    //[InitializeOnLoad]
    public class UnityAsset_Postprocessor : AssetPostprocessor
    {
        static AssetHandeler[] AssetList = new AssetHandeler[5] {
            new WorldScene_UnityAsset(),
            new Table_UnityAsset(),
            new PCGResource_UnityAsset(),
            new PCGGeneration_UnityAsset(),
            new PCGCash_UnityAsset() };

        // static UnityAsset_Postprocessor()
        // {
        //     // Use a unique key for your project/session
        //     if (!SessionState.GetBool("RealMethod.EditorStartupOnce", false))
        //     {
        //         SessionState.SetBool("RealMethod.EditorStartupOnce", true);
        //         StartEditor();
        //     }
        // }

        private static void StartEditor()
        {
        }
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (AssetList == null)
            {
                return;
            }

            foreach (var asset in AssetList)
            {
                foreach (string assetPath in importedAssets)
                {
                    asset.OnAssetImported(assetPath);
                }
                foreach (string assetPath in deletedAssets)
                {
                    asset.OnAssetDeleted(assetPath);
                }
                for (int i = 0; i < movedAssets.Length; i++)
                {
                    asset.OnAssetMoved(movedAssets[i], movedFromAssetPaths[i]);
                }
            }

        }
    }



    public abstract class AssetHandeler
{
    public AssetHandeler()
    {
        Initialized();
    }
    protected abstract void Initialized();
    public abstract void OnAssetImported(string AssetPath);
    public abstract void OnAssetDeleted(string AssetPath);
    public abstract void OnAssetMoved(string AssetPath, string FromPath);

    protected abstract string GetIconPath();
    protected bool TryLoadAsset<T>(string assetPath, out T asset) where T : Object
    {
        asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        return asset != null;
    }
}
public abstract class AssetHandeler<T, J> : AssetHandeler where T : Object
{
    public override void OnAssetImported(string AssetPath)
    {
        T loadedAsset;
        if (TryLoadAsset(AssetPath, out loadedAsset))
        {
            if (loadedAsset.GetType() == typeof(J))
            {
                // Load your icon from Resources (adjust the path as needed)
                Texture2D icon = Resources.Load<Texture2D>(GetIconPath());
                if (icon != null)
                {
                    EditorGUIUtility.SetIconForObject(loadedAsset, icon);
                }
                else
                {
                    Debug.LogWarning("Custom icon not found. Please ensure the path and file are correct.");
                }
            }
        }
        else
        {
            Debug.LogWarning("Cant Load ");
        }
    }
}

}