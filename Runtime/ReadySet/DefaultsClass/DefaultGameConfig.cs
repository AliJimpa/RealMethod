using UnityEngine;

namespace RealMethod
{
    public sealed class DefaultGameConfig : GameConfig
    {
        public override void GameStarted(Game Author)
        {
            Debug.Log("DefaultGameConfig Loaded");
        }
    }
}
