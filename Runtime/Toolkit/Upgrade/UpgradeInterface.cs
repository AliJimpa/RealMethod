using System.Collections.Generic;

namespace RealMethod
{
    public interface IUpgradeable
    {
        bool Initiate(Upgrade owner, UpgradeConfig config, UpgradeAsset previous);
        void SetUnlock(bool free = false);
        void SetLock();
    }

    public interface IUpgradeStorage
    {
        void Initiate(Upgrade owner, UpgradeAsset[] list);
        bool SwapToUnAvalibal(UpgradeAsset target);
        bool SwapToAvalibal(UpgradeAsset target);
        bool IsUnAvalibal(UpgradeAsset target);
        bool IsAvalabel(UpgradeAsset target);
    }
}