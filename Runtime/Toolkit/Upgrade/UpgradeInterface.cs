using System.Collections.Generic;

namespace RealMethod
{
    public interface IUpgradeable
    {
        bool Initiate(Upgrade owner, UpgradeConfig config, UpgradeItem previous);
        void SetUnlock(bool free = false);
        void SetLock();
    }

    public interface IUpgradeStorage
    {
        void CreateNewItems(UpgradeItem[] list);
        bool SwapToUnAvalibal(UpgradeItem target);
        bool SwapToAvalibal(UpgradeItem target);
        bool IsUnAvalibal(UpgradeItem target);
        bool IsAvalabel(UpgradeItem target);
    }
}