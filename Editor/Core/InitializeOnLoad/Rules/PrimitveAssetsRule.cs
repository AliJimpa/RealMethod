using System;
using UnityEngine;
using UnityEditor;

namespace RealMethod.Editor
{
    public class PrimitveAssetsRule : CompileRule
    {
        private PrimitiveAsset[] assets = null;
        private int state = 0;

        protected override void Initilized()
        {
            assets = Resources.FindObjectsOfTypeAll<PrimitiveAsset>();
        }
        public override bool CanCheck(RuleExecutionMode mode, Type type)
        {
            if ((int)mode < 2)
                return false;


            state = (int)mode - 2;
            return true;
        }
        public override void OnCheck(Type type)
        {
            foreach (var asset in assets)
            {
                if (asset.AutoReset((PlayModeStateChange)state))
                {
                    asset.Invoke("Reset");
                }
            }
        }


    }
}