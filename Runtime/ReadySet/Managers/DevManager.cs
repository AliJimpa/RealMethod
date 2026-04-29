#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Reflection;
using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Manager/DeveloperManager")]
    public class DevManager : DeveloperManager
    {
        // DeveloperManager Methods
        public override void InitiateManager(bool alwaysLoaded)
        {
        }
        public override void ResolveService(Service service, bool active)
        {
        }
    }
}
#endif