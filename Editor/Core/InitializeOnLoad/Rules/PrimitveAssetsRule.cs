using System;
using UnityEngine;
using UnityEditor;

namespace RealMethod.Editor
{
    public class PrimitveAssetsRule : CompileRule
    {
        private PrimitiveAsset[] assets = null;

        protected override void Initilized()
        {
            assets = Resources.FindObjectsOfTypeAll<PrimitiveAsset>();
        }
        public override void OnStartCheck(RuleExecutionMode mode)
        {
            if ((int)mode < 2)
                return;

            PlayModeStateChange CurrentMode = (PlayModeStateChange)((int)mode - 2);
            foreach (var asset in assets)
            {
                if (mode == RuleExecutionMode.EnteredPlayMode)
                {
                    if (asset is not DataAsset)
                        asset.Invoke(GameMessage.AssetPermission);

                    if (AssetDatabase.Contains(asset))
                    {
                        if (asset is InstanceAsset)
                        {
                            Debug.LogWarning($"InstanceAsset '{asset.name}' is used directly in Play Mode. " +
                            $"A runtime instanceAsset should be used instead.Create() at runtime",
                            asset);
                        }
                    }
                }

                if (asset.AutoReset(CurrentMode))
                {
                    asset.Invoke(GameMessage.Reset);
                }
            }
        }
        public override bool CanCheck(RuleExecutionMode mode, Type type)
        {
            return false;
        }
        public override void OnCheck(Type type)
        {

        }

    }
}