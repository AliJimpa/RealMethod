#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Manager/DeveloperManager")]
    public class DevManager : DeveloperManager
    {

        // DeveloperManager Methods
        protected override void InitiateManager(bool alwaysLoaded)
        {
        }
        protected override void ResolveService(Service service)
        {
        }
    }
}
#endif