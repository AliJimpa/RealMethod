using UnityEditor;

namespace RealMethod.Editor
{
    public class Table_UnityAsset : AssetProcess<TableAsset, DataAsset>
    {
        protected override void Initialized()
        {

        }
        public override void OnAssetDeleted(string AssetPath)
        {

        }
        public override void OnAssetMoved(string AssetPath, string FromPath)
        {

        }
        protected override string GetIconPath()
        {
            return "Icons/Pattern/TableAsset";
        }

        protected override void DoubleClick(TableAsset asset)
        {
            TableViewerWindow.OpenWindow(asset);
        }
    }
    public class WorldScene_UnityAsset : AssetProcess<WorldSceneConfig, DataAsset>
    {
        protected override void Initialized()
        {

        }
        public override void OnAssetDeleted(string AssetPath)
        {

        }
        public override void OnAssetMoved(string AssetPath, string FromPath)
        {

        }
        protected override string GetIconPath()
        {
            return "Icons/Core/WorldSceneAsset";
        }
        protected override void DoubleClick(WorldSceneConfig asset)
        {
            asset.OnAssetClick();
        }
    }
    public class Game_UnityAsset : AssetProcess<MonoScript, Game>
    {
        protected override void Initialized()
        {

        }
        public override void OnAssetDeleted(string AssetPath)
        {

        }
        public override void OnAssetMoved(string AssetPath, string FromPath)
        {

        }
        protected override string GetIconPath()
        {
            return "Icons/Core/GameClass";
        }
        protected override void DoubleClick(MonoScript asset)
        {
            
        }
    }

}