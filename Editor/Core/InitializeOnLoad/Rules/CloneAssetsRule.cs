using System;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class CloneAssetsRule : CompileRule
    {
        protected override void Initilized()
        {

        }
        public override RuleExecutionMode GetRuleMode()
        {
            return RuleExecutionMode.EnteredPlayMode;
        }
        public override void OnStart(RuleExecutionMode mode)
        {
        }
        public override Type GetBaseType()
        {
            return null;
            //return typeof(CloneAsset);
        }
        public override void OnCheck(Type type)
        {
            var assets = Resources.FindObjectsOfTypeAll<PrimitiveAsset>();

            foreach (var asset in assets)
            {
                if (AssetDatabase.Contains(asset))
                {
                    Debug.LogWarning(
                        $"PrimitiveAsset '{asset.name}' is used directly in Play Mode. " +
                        $"A runtime instance or clone should be used instead.",
                        asset);
                }
            }
        }
        public override void OnEnd(RuleExecutionMode mode)
        {
        }
    }
}