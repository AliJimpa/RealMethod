namespace RealMethod.Editor
{
    public class PCGResource_UnityAsset : AssetProcess<PCGResourceConfig, DataAsset>
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
        protected override void DoubleClick(PCGResourceConfig asset)
        {

        }

    }
    public class PCGGeneration_UnityAsset : AssetProcess<PCGGenerationAsset, DataAsset>
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
    public class PCGCash_UnityAsset : AssetProcess<PCGCashAsset, DataAsset>
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