using UnityEngine;

namespace RealMethod
{
    public sealed class DefaultGameBridge : GameBridge
    {
        protected override void OnStart(object Author)
        {
            Debug.Log("DefaultBridge Started");
        }
        protected override void OnNewWorld()
        {
        }
        protected override void OnEnd(object Author)
        {
            Debug.Log("DefaultBridge Ended");
        }
    }
}