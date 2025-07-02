using System;
using UnityEngine;

namespace RealMethod
{
    public class InventoryItemProperty
    {
        [SerializeField]
        private string ItemName;
        [SerializeField]
        private InventoryItemAsset ItemAsset;
        public InventoryItemAsset Asset => ItemAsset;
        [SerializeField]
        private int ItemQuantity;
        public int Quantity => ItemQuantity;
        [SerializeField]
        private int ItemCapacity;
        public int Capacity => ItemCapacity;

        public InventoryItemProperty(InventoryItemAsset asset, int quantity, int capacity = 0)
        {
            ItemAsset = asset;
            ItemQuantity = quantity;
            ItemCapacity = capacity;
        }

        public bool Add(int value = 1)
        {
            ItemQuantity = ItemQuantity + value;
            if (ItemQuantity <= Capacity || Capacity == 0)
            {
                return true;
            }
            else
            {
                ItemQuantity = Capacity;
                return false;
            }
        }
        public bool Remove(int value = 1)
        {
            ItemQuantity = ItemQuantity - value;
            if (ItemQuantity <= 0)
            {
                ItemQuantity = 0;
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public abstract class InventoryItemAsset : ItemAsset
    {
        public abstract void PickedUp(Inventory owner, int quantity);
        public abstract void Cahanged(int quantity);
        public abstract void Dropped(Inventory owner);
        public abstract bool CanChange(bool IsAdded);
        public abstract bool CanPickUp(Inventory owner);
        public abstract bool CanDropp(Inventory owner);
    }


}