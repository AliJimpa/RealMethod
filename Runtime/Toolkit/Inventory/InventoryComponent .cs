using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Inventory/Inventory")]
    public sealed class InventoryComponent : InventoryStorage
    {
        protected override void ItemAdded(ItemAsset target)
        {
        }
        protected override void ItemUpdated(ItemAsset target, int Quantity)
        {
        }
        protected override void ItemRemoved()
        {
        }
    }
}
