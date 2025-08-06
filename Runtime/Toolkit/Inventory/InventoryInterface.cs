namespace RealMethod
{
    public interface IInventoryStorage : IStorage
    {
        InventoryItemProperty[] GetItems();
        void CreateItem(InventoryItemProperty item);
        void DestroyItem(IInventoryItem item);
        // amount > 0 : AddQuantity , amount < 0 RemoveqQuantity , amount = 0 ZiroQuantity
        void UpdateQuantity(IInventoryItem item, int amount);
        void UpdateCapacity(IInventoryItem item, int value);
    }

    public interface IInventoryItem : IItem , IResource
    {
        void PickedUp(Inventory owner, int quantity);
        void Cahanged(int quantity);
        void Dropped(Inventory owner);
        bool CanChange(bool IsAdded);
        bool CanPickUp(Inventory owner);
        bool CanDropp(Inventory owner);
    }
}