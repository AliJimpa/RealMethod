using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class UnityAsset_Postprocessor : AssetPostprocessor
    {
        static AssetHandeler[] AssetList = new AssetHandeler[5] {
            new WorldScene_UnityAsset(),
            new Table_UnityAsset(),
            new PCGResource_UnityAsset(),
            new PCGGeneration_UnityAsset(),
            new PCGCash_UnityAsset() };

        // private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        // {
        //     if (AssetList == null)
        //     {
        //         return;
        //     }

        //     foreach (var asset in AssetList)
        //     {
        //         foreach (string assetPath in importedAssets)
        //         {
        //             asset.OnAssetImported(assetPath);
        //         }
        //         foreach (string assetPath in deletedAssets)
        //         {
        //             asset.OnAssetDeleted(assetPath);
        //         }
        //         for (int i = 0; i < movedAssets.Length; i++)
        //         {
        //             asset.OnAssetMoved(movedAssets[i], movedFromAssetPaths[i]);
        //         }
        //     }
        // }

        [InitializeOnLoadMethod]
        private static void OnDoubleClickScriptableObject()
        {
            EditorApplication.projectWindowItemOnGUI += (guid, rect) =>
            {
                Event e = Event.current;
                if (e.type == EventType.MouseDown && e.clickCount == 2)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    foreach (var asset in AssetList)
                    {
                        asset.OnAssetClick(assetPath, e);
                    }
                }
            };
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
        public abstract void OnAssetClick(string AssetPath, Event e);
        protected abstract string GetIconPath();
    }

    public abstract class AssetHandeler<T, J> : AssetHandeler where T : Object
    {
        protected abstract void DoubleClick(T asset);

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
        public override void OnAssetClick(string AssetPath, Event e)
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(AssetPath);

            if (asset != null)
            {
                DoubleClick(asset);
                e.Use(); // Consume the event
            }
        }

        protected bool TryLoadAsset<K>(string assetPath, out K asset) where K : Object
        {
            asset = AssetDatabase.LoadAssetAtPath<K>(assetPath);
            return asset != null;
        }
    }




}