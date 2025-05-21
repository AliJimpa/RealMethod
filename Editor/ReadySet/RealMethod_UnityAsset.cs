using UnityEditor;

namespace RealMethod
{
    public class Table_UnityAsset : AssetHandeler<TableAsset, DataAsset>
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

    }
    public class WorldScene_UnityAsset : AssetHandeler<WorldSceneAsset, DataAsset>
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
    }
    public class Game_UnityAsset : AssetHandeler<MonoScript, Game>
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
    }

}