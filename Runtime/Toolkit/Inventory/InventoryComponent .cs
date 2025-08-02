using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Inventory/Inventory")]
    public sealed class InventoryComponent : InventoryStorage
    {
        protected override void ItemAdded(IInventoryItem target)
        {
        }
        protected override void ItemUpdated(IInventoryItem target, int Quantity)
        {
        }
        protected override void ItemRemoved()
        {
        }
    }
}
