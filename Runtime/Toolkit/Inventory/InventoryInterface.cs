namespace RealMethod
{
    public interface IInventoryStorage
    {
        InventoryItemProperty[] GetItems();
        void CreateItem(InventoryItemProperty item);
        void DestroyItem(string name);
        // amount > 0 : AddQuantity , amount < 0 RemoveqQuantity , amount = 0 ZiroQuantity
        void UpdateQuantity(string name, int amount); 
        void UpdateCapacity(int value);
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