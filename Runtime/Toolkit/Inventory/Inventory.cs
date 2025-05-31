using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Inventory/Inventory")]
    public class Inventory : MonoBehaviour
    {
        private enum ItemState
        {
            Create,
            Update,
            Delete
        }
        private enum BehaviorType
        {
            None,
            Action,
            SendMessage,
            Broadcast
        }
        private class ItemPack
        {
            public ItemAsset Asset { get; private set; }
            public int Quantity { get; private set; }
            public int Capacity { get; private set; }

            public ItemPack(ItemAsset asset, int quantity, int capacity = 0)
            {
                Asset = asset;
                Quantity = quantity;
                Capacity = capacity;
            }

            public bool Add(int value = 1)
            {
                Quantity = Quantity + value;
                if (Quantity <= Capacity || Capacity == 0)
                {
                    return true;
                }
                else
                {
                    Quantity = Capacity;
                    return false;
                }
            }
            public bool Remove(int value = 1)
            {
                Quantity = Quantity - value;
                if (Quantity <= 0)
                {
                    Quantity = 0;
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [Header("Setting")]
        [SerializeField, Tooltip("Zero means No Limit")]
        private int _capacity = 0;
        public int Capacity => _capacity;
        public bool IsEnoughCapacity => _capacity > 0 ? Items.Count < _capacity : true;
        [SerializeField]
        private BehaviorType Behavior;
        [SerializeField]
        private ItemAsset[] DefaultItem;
        public int Count => Items.Count;
        public Action<ItemAsset, int> OnItemAdded;
        public Action<ItemAsset, int> OnItemUpdated;
        public Action OnItemRemove;

        private HashedKeyItem<ItemPack> Items = new HashedKeyItem<ItemPack>(5);


        private void Awake()
        {
            if (DefaultItem != null)
            {
                foreach (var item in DefaultItem)
                {
                    AddItem(item);
                }
            }
        }


        public ItemAsset this[string Name]
        {
            get => Items[Name].Asset;
        }


        public int GetQuantity(string Name)
        {
            if (Items.IsValid(Name))
            {
                return Items[Name].Quantity;
            }
            else
            {
                Debug.LogWarning("Ther isnt any item with this Name");
                return -1;
            }
        }
        public int GetQuantity(ItemAsset asset)
        {
            return GetQuantity(asset.Name);
        }
        public bool IsValidItem(string Name)
        {
            return Items.IsValid(Name);
        }
        public bool CreateNewItem(ItemAsset item, int Quantity, int ItemCapacity)
        {
            if (!Items.IsValid(item.Name))
            {
                Items.AddItem(item.Name, new ItemPack(item, Quantity, ItemCapacity));
                SendInventoryMessage(ItemState.Create, item, Quantity);
                return true;
            }
            else
            {
                Debug.LogWarning($"Item with this Name {item.Name} already there");
                return false;
            }
        }
        public bool AddItem(ItemAsset item, int Quantity = 1)
        {
            if (Items.Count < _capacity || _capacity == 0)
            {
                if (Items.IsValid(item.Name))
                {
                    Items[item.Name].Add(Quantity);
                    SendInventoryMessage(ItemState.Update, item, Quantity);
                }
                else
                {
                    Items.AddItem(item.Name, new ItemPack(item, Quantity));
                    SendInventoryMessage(ItemState.Create, item, Quantity);
                }
                return true;
            }
            else
            {
                Debug.LogWarning("There is not enough space.");
                return false;
            }
        }
        public bool RemoveItem(string Name, int Quantity = 1)
        {
            ItemPack target;
            if (Items.TryGetItem(Name, out target))
            {
                if (target.Remove(Quantity))
                {
                    SendInventoryMessage(ItemState.Update, target.Asset, target.Quantity);
                    return true;
                }
                else
                {
                    bool Result = Items.RemoveItem(Name);
                    if (Result)
                    {
                        SendInventoryMessage(ItemState.Delete, null, 0);
                    }
                    return Result;
                }
            }
            else
            {
                Debug.LogWarning($"Ther isnt any Item with this Name {Name}");
                return false;
            }
        }
        public bool DeleteItem(string Name)
        {
            if (Items.IsValid(Name))
            {
                bool Result = Items.RemoveItem(Name);
                if (Result)
                {
                    SendInventoryMessage(ItemState.Delete, null, 0);
                }
                return Result;
            }
            else
            {
                Debug.LogWarning($"Ther isnt any Item With this Name {Name}");
                return false;
            }
        }
        public T[] CopyItemsByClass<T>() where T : ItemAsset
        {
            List<T> Result = new List<T>();
            foreach (var pack in Items.GetValues())
            {
                if (pack.Asset is T finditem)
                {
                    Result.Add(finditem);
                }
            }
            return Result.ToArray();
        }
        public T CopyItemByClass<T>() where T : ItemAsset
        {
            foreach (var pack in Items.GetValues())
            {
                if (pack.Asset is T finditem)
                {
                    return finditem;
                }
            }
            return null;
        }

        private void SendInventoryMessage(ItemState state, ItemAsset target, int quantity)
        {

            switch (state)
            {
                case ItemState.Create:
                    target.PickedUp(this, quantity);
                    switch (Behavior)
                    {
                        case BehaviorType.Action:
                            OnItemAdded?.Invoke(target, quantity);
                            break;
                        case BehaviorType.SendMessage:
                            gameObject.SendMessage("OnItemAdded", target, SendMessageOptions.RequireReceiver);
                            break;
                        case BehaviorType.Broadcast:
                            gameObject.BroadcastMessage("OnItemAdded", target, SendMessageOptions.RequireReceiver);
                            break;
                        default:
                            break;
                    }
                    break;
                case ItemState.Update:
                    target.Cahanged(quantity);
                    switch (Behavior)
                    {
                        case BehaviorType.Action:
                            OnItemUpdated?.Invoke(target, quantity);
                            break;
                        case BehaviorType.SendMessage:
                            gameObject.SendMessage("OnItemUpdated", target, SendMessageOptions.RequireReceiver);
                            break;
                        case BehaviorType.Broadcast:
                            gameObject.BroadcastMessage("OnItemUpdated", target, SendMessageOptions.RequireReceiver);
                            break;
                        default:
                            break;
                    }
                    break;
                case ItemState.Delete:
                    target.Dropped(this);
                    switch (Behavior)
                    {
                        case BehaviorType.Action:
                            OnItemRemove?.Invoke();
                            break;
                        case BehaviorType.SendMessage:
                            gameObject.SendMessage("OnItemRemove", SendMessageOptions.RequireReceiver);
                            break;
                        case BehaviorType.Broadcast:
                            gameObject.BroadcastMessage("OnItemRemove", SendMessageOptions.RequireReceiver);
                            break;
                        default:
                            break;
                    }
                    break;
            }
        }

    }

    public abstract class ItemAsset : DataAsset
    {
        [Header("Basic")]
        [SerializeField]
        protected string _name;
        public string Name => _name;
        [SerializeField]
        protected Texture2D _icon;
        public Texture2D Icon => _icon;

        public abstract void PickedUp(Inventory owner, int quantity);
        public abstract void Cahanged(int quantity);
        public abstract void Dropped(Inventory owner);
        public abstract bool CanChange(bool IsAdded);
    }

    public abstract class ItemAsset<T> : ItemAsset where T : Enum
    {
        [Header("Category")]
        [SerializeField]
        protected T Type;
    }


}
