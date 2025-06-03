using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Manager/HUDManager")]
    public sealed class HUDManager : UIManager
    {
        public override void InitiateService(Service newService)
        {

        }
        public override bool IsMaster()
        {
            return true;
        }
    }
}