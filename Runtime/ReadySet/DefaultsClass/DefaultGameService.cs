using UnityEngine;

namespace RealMethod
{
    public sealed class DefaultGameService : GameService
    {
        protected override void OnStart(object Author)
        {
            Debug.Log("DefaultGameService Started");
        }
        protected override void OnNewWorld()
        {
        }
        protected override void OnNewAdditiveWorld(World target)
        {
            
        }
        protected override void OnEnd(object Author)
        {
            Debug.LogError("DefaultGameService Ended [This should not happent]");
        }
    }
}