using UnityEngine;

namespace RealMethod
{
    public sealed class DefaultGameConfig : GameConfig
    {
        public override void Initialized()
        {
            Debug.Log("DefaultConfig Loaded");
        }
    }
}
