using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class UnityAsset_Postprocessor : AssetPostprocessor
    {
        private static AssetProcess[] AssetList = new AssetProcess[5]
        {
            new WorldScene_UnityAsset(),
            new Table_UnityAsset(),
            new PCGResource_UnityAsset(),
            new PCGGeneration_UnityAsset(),
            new PCGCash_UnityAsset()
        };

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
    }
}