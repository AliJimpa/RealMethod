using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Essential/DefultGame")]
    public sealed class DefultGame : Game
    {
        protected override void OnGameOpen()
        {
           
        }
        protected override void OnGameInitialized()
        {
            AddModule<DebugModule>();
        }
        protected override void OnGameStart()
        {
            
        }
        protected override void OnWorldChanged(World NewWorld)
        {
            
        }
        protected override void OnGameClosed()
        {
            
        }


    }
}