using UnityEngine;

namespace RealMethod
{
    public class CoroutineManager : MonoBehaviour, IGameManager
    {


        public MonoBehaviour GetManagerClass()
        {
            return this;
        }
        public void InitiateManager(bool AlwaysLoaded)
        {
            if (Game.TryFindService(out Spawn SpawnServ))
            {
                SpawnServ.BringManager(this);
            }
        }
        public void InitiateService(Service service)
        {
            if (service is Spawn spawnservice)
            {
                spawnservice.BringManager(this);
            }
        }
    }
}