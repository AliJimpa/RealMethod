using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Upgrade/Upgrade")]
    public sealed class UpgradeComponent : UpgradeStorage
    {
        [Header("Events")]
        public UnityEvent<bool> Unlocked;

        // Base Upgrade Methods
        protected override void UnlockedItem(IUpgradeItem item)
        {
            Unlocked?.Invoke(true);
        }
        protected override void lockedItem(IUpgradeItem item)
        {
            Unlocked?.Invoke(false);
        }
        


    }


}