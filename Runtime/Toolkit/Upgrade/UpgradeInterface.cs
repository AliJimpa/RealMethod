namespace RealMethod
{
    public interface IUpgradeItem
    {
        string Label { get; }
        bool IsUnlocked { get; }
        string ConfigLabel { get; }
        void Identify(UpgradeMapConfig map, int mapIndex, int itemIndex, int various);
        void Sync(bool status);

        bool Prerequisites(bool cost);
        void Unlock(bool cost);
        void Lock();

        IUpgradeItem[] GetNextAvailables();
        void AddNextAvailables(IUpgradeItem items);
        void OnPreviousItem(IUpgradeItem items);
    }
    public interface IUpgradeConfig
    {
        IUpgradeItem[] GenerateItems(Upgrade owner);
        IUpgradeItem GetStartItem();
    }

    public interface IUpgradeStorage : IStorage
    {
        void UnlockItem(IUpgradeItem item);
        void LockItem(IUpgradeItem item);
        void AddAvailableItem(IUpgradeItem item);
        void RemoveAvalibelItem(IUpgradeItem item);
        string[] GetAvailableItems();
        string[] GetUnlockItems();
    }
}