namespace RealMethod
{
    public interface IInventorySave
    {
        InventoryItemProperty[] LoadInventory(Inventory owner);
        void SaveInventory(Inventory owner, InventoryItemProperty[] Data);
        bool IsExistInventoryData(Inventory owner);
    }
}