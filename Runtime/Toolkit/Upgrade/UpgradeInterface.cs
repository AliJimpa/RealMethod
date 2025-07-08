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
        void Initiate(Upgrade Owner, string[] StartList);
        bool SwapToUnAvalibal(string target);
        bool SwapToAvalibal(string target);
        bool IsUnAvalibal(string target);
        bool IsAvalabel(string target);
    }
}