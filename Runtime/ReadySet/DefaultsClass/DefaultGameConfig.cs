using UnityEngine;

namespace RealMethod
{
    public sealed class DefaultGameConfig : GameConfig
    {
        public override void Initialized(Game Author)
        {
            Debug.Log("DefaultConfig Loaded");
        }
    }
}
