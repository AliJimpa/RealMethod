#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Reflection;
using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Manager/DeveloperManager")]
    public class DevManager : DeveloperManager
    {
        // DeveloperManager Methods
        public override void InitiateManager(Scope owner)
        {
        }
    }
}
#endif