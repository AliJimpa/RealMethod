using UnityEngine;

namespace RealMethod
{
    public sealed class DefaultGameService : GameService
    {
        protected override void OnStart(object Author)
        {
            Debug.Log("DefaultService Started");
        }
        protected override void OnNewWorld()
        {
        }
        protected override void OnEnd(object Author)
        {
            Debug.Log("DefaultService Ended");
        }
    }
}