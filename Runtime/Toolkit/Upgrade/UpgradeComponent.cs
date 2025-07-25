using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Upgrade/Upgrade")]
    public sealed class UpgradeComponent : UpgradeStorage
    {
        [Header("Events")]
        public UnityEvent<UpgradeItem> Unlocked;

        // Base Upgrade Methods
        protected override void OnLockedAsset(UpgradeItem item)
        {
        }
        protected override void OnUnlockedAsset(UpgradeItem item)
        {
            Unlocked?.Invoke(item);
        }
    }


}