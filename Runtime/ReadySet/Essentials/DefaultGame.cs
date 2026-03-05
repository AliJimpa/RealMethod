
using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Essential/DefultGame")]
    public sealed class DefultGame : Game
    {
        protected override void OnGameInitialized()
        {
            Debug.Log("DefultGame Initialized");
        }
        protected override void OnGameStarted()
        {
            Debug.Log("DefultGame Started");
        }
        protected override void OnWorldChanged(World NewWorld)
        {
            Debug.Log($"DefultGame.World Change to {NewWorld}");
        }
        protected override void OnGameClosed()
        {
            Debug.Log("DefultGame Closed");
        }

    }
}
