using UnityEngine;

namespace RealMethod
{
    public sealed class DefaultGameBridge : GameBridge
    {
        protected override void OnStart(object Author)
        {
            Debug.Log("DefaultBridge Connected");
        }
        protected override void OnWorldChanging(World Previous, World New)
        {
        }
        protected override void OnEnd(object Author)
        {
            Debug.Log("DefaultBridge Disconnected");
        }


    }
}