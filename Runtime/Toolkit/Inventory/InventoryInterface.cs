namespace RealMethod
{
    public interface IInventorySave
    {
        InventoryItemProperty[] ReadInventoryData(Inventory owner);
        void WriteInventoryData(Inventory owner, InventoryItemProperty[] Data);
        bool IsExistInventoryData(Inventory owner);
    }

    public interface IInventoryItem
    {
        void PickedUp(Inventory owner, int quantity);
        void Cahanged(int quantity);
        void Dropped(Inventory owner);
        bool CanChange(bool IsAdded);
        bool CanPickUp(Inventory owner);
        bool CanDropp(Inventory owner);
    }
}