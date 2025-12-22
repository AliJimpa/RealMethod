
using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Essential/DefultGame")]
    public sealed class DefultGame : Game
    {
        protected override void GameInitialized()
        {
            Debug.Log("DefultGame Initialized");
        }
        protected override void GameStarted()
        {
            Debug.Log("DefultGame Started");
        }
        protected override void OnWorldChanged(World NewWorld)
        {
            Debug.Log($"DefultGame Synced to {NewWorld}");
        }
        protected override void OnGameClosed()
        {
            Debug.Log("DefultGame Closed");
        }

    }
}
