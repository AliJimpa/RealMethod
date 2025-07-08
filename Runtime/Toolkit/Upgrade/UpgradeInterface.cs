namespace RealMethod
{
    public interface IUpgradeable
    {
        bool Initiate(Upgrade owner, UpgradeConfig config, UpgradeAsset previous);
        void SetUnlock(bool free = false);
        void SetLock();
    }
}