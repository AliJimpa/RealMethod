
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
            Debug.Log($"{NewWorld} Synced to Game");
        }
        protected override void GameClosed()
        {
            Debug.Log("Game Closed");
        }
    }
}
