using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Manager/HUDManager")]
    public sealed class HUDManager : UIManager
    {
        public override void InitiateService(Service newService)
        {
            if (newService is Spawn spawnservice)
            {
                spawnservice.BringManager(this);
            }
        }
        public override bool IsMaster()
        {
            return true;
        }
    }
}