namespace RealMethod.Editor
{
    public class PCGResource_UnityAsset : AssetHandeler<PCGResourceAsset, DataAsset>
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
            return "Icons/Toolkit/PCGResource";
        }
        protected override void DoubleClick(PCGResourceAsset asset)
        {

        }

    }
    public class PCGGeneration_UnityAsset : AssetHandeler<PCGGenerationAsset, DataAsset>
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
            return "Icons/Toolkit/PCG_Generation";
        }
        protected override void DoubleClick(PCGGenerationAsset asset)
        {

        }

    }
    public class PCGCash_UnityAsset : AssetHandeler<PCGCashAsset, DataAsset>
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
            return "Icons/Toolkit/PCGCash";
        }
        protected override void DoubleClick(PCGCashAsset asset)
        {

        }

    }
}