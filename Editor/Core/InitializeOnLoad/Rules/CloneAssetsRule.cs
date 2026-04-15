using System;

namespace RealMethod.Editor
{
    public class CloneAssetsRule : CompileRule
    {
        protected override void Initilized()
        {

        }
        public override bool CanCheck(RuleExecutionMode mode, Type type)
        {
            return false;
            // if (mode == RuleExecutionMode.AfterCompilation)
            // {
            //     return typeof(Service);
            // }
            // else
            // {
            //     return null;
            // }
        }
        public override void OnCheck(Type type)
        {
            // var assets = Resources.FindObjectsOfTypeAll<CloneAsset>();
            // foreach (var asset in assets)
            // {
            //     if (AssetDatabase.Contains(asset))
            //     {
            //         Debug.LogWarning(
            //             $"PrimitiveAsset '{asset.name}' is used directly in Play Mode. " +
            //             $"A runtime instance or clone should be used instead.",
            //             asset);
            //     }
            // }
        }
    }
}