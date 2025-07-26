using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Manager/SoundManager")]
    public sealed class SoundManager : AudioManager
    {
        // AudioManager Methods
        protected override bool CanBringtoSpawn()
        {
            return true;
        }
    }
}