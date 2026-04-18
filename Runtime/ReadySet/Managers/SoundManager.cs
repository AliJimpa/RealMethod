using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Manager/SoundManager")]
    public sealed class SoundManager : AudioManager
    {
        // AudioManager Methods
        public override void ResolveService(Service service, bool active)
        {
            base.ResolveService(service, active);
        }
    }
}