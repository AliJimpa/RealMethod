using UnityEngine;

namespace RealMethod
{
    public sealed class DefaultGameSetting : GameSettingAsset
    {
        public override void GameStarted(Game Author)
        {
            Debug.Log("DefaultGameSetting Loaded");
        }
    }
}
