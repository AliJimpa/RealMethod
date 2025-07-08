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
        protected override void OnAssetUpdate(UpgradeItem item, bool unlocked)
        {
            if (unlocked)
            {
                Unlocked?.Invoke(item);
            }
        }

    }


}