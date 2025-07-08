using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Inventory/Inventory")]
    public sealed class InventoryComponent : InventoryStorage
    {
        protected override void AddItem(ItemAsset target)
        {
        }
        protected override void UpdateItem(ItemAsset target, int Quantity)
        {
        }
        protected override void RemoveItem()
        {
        }
    }
}
