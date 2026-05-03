using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Essential/DefultGame")]
    public sealed class DefultGame : Game
    {
        protected override void OnGameOpen()
        {
            Debug.Log("DefultGame Opened");
        }
        protected override void OnGameInitialized()
        {
            AddService<DebugService>();
            Debug.Log("DefultGame Initialized");
        }
        protected override void OnGameStart()
        {
            Debug.Log("DefultGame Started");
        }
        protected override void OnWorldChanged(World NewWorld)
        {
            Debug.Log($"New world assign: {NewWorld}");
        }
        protected override void OnGameClosed()
        {
            Debug.Log("DefultGame Closed");
        }


    }
}