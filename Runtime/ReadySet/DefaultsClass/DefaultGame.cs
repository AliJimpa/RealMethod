
using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Essential/DefultGame")]
    public sealed class DefultGame : Game
    {
        protected override void GameServiceCreated()
        {
        }
        protected override void GameInitialized()
        {
            Debug.Log("DefultGame Initialized");
        }
        protected override void WorldSynced(World NewWorld)
        {
            Debug.Log("New World Synced to Game");
        }
    }
}
