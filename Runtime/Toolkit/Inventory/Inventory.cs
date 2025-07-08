using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{

    public class InventoryItemProperty
    {
        [SerializeField]
        private string ItemName;
        [SerializeField]
        private ItemAsset ItemAsset;
        public ItemAsset Asset => ItemAsset;
        [SerializeField]
        private int ItemQuantity;
        public int Quantity => ItemQuantity;
        [SerializeField]
        private int ItemCapacity;
        public int Capacity => ItemCapacity;

        public InventoryItemProperty(ItemAsset asset, int quantity, int capacity = 0)
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


    public abstract class Inventory : MonoBehaviour
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
            Broadcast,
            ActionAndMessage,
        }

        [Header("Setting")]
        [SerializeField, Tooltip("Zero means infinit")]
        private int _capacity = 0;
        public int Capacity => _capacity;
        public bool IsEnoughCapacity => _capacity > 0 ? Items.Count < _capacity : true;
        [SerializeField]
        private BehaviorType Behavior;
        [SerializeField]
        private ItemAsset[] DefaultItem;
        public bool IsLoading { get; protected set; }

        // Actions
        public Action<ItemAsset, int> OnItemAdded;
        public Action<ItemAsset, int> OnItemUpdated;
        public Action OnItemRemoved;

        // Private Variable
        private Hictionary<InventoryItemProperty> Items = new Hictionary<InventoryItemProperty>(5);
        public int Count => Items.Count;


        public ItemAsset this[string itemName]
        {
            get => Items[itemName].Asset;
        }

        // Unity Methods
        protected virtual void Awake()
        {
            IsLoading = false;
            if (UseDefaultItem())
            {
                if (DefaultItem != null)
                {
                    foreach (var item in DefaultItem)
                    {
                        AddItem(item);
                    }
                }
            }
        }

        // public methods
        public int GetQuantity(string itemName)
        {
            if (Items.ContainsKey(itemName))
            {
                return Items[itemName].Quantity;
            }
            else
            {
                Debug.LogWarning("Ther isnt any item with this Name [Be sure to use ItemTitle]");
                return -1;
            }
        }
        public int GetQuantity(ItemAsset item)
        {
            if (GetInterface(item) != null)
            {
                return GetQuantity(item.Title);
            }
            else
            {
                return -1;
            }
        }
        public bool IsValidItem(string itemName)
        {
            return Items.ContainsKey(itemName);
        }
        public bool IsValidItem(ItemAsset item)
        {
            if (GetInterface(item) != null)
            {
                return Items.ContainsKey(item.Title);
            }
            else
            {
                return false;
            }
        }
        public bool AddNewItem(ItemAsset item, int Quantity, int ItemCapacity)
        {
            IInventoryItem invitem = GetInterface(item);
            if (invitem == null)
                return false;

            if (!Items.ContainsKey(item.Title))
            {
                if (invitem.CanPickUp(this))
                {
                    Items.Add(item.Title, new InventoryItemProperty(item, Quantity, ItemCapacity));
                    SendInventoryMessage(ItemState.Create, item, Quantity);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Debug.LogWarning($"Item with this Name {item.Title} already there");
                return false;
            }
        }
        public bool AddItem(ItemAsset item, int Quantity = 1)
        {
            IInventoryItem invitem = GetInterface(item);
            if (invitem == null)
                return false;

            if (Items.Count < _capacity || _capacity == 0)
            {
                if (Items.ContainsKey(item.Title))
                {
                    if (invitem.CanChange(true))
                    {
                        Items[item.Title].Add(Quantity);
                        SendInventoryMessage(ItemState.Update, item, Quantity);
                    }
                }
                else
                {
                    if (invitem.CanPickUp(this))
                    {
                        Items.Add(item.Title, new InventoryItemProperty(item, Quantity));
                        SendInventoryMessage(ItemState.Create, item, Quantity);
                    }
                }
                return true;
            }
            else
            {
                Debug.LogWarning("There is not enough space.");
                return false;
            }
        }
        public bool RemoveItem(string itemName, int Quantity = 1)
        {
            InventoryItemProperty target;
            if (Items.TryGetValue(itemName, out target))
            {
                IInventoryItem invitem = GetInterface(target.Asset);
                if (invitem == null)
                    return false;

                if (invitem.CanChange(true))
                {
                    if (target.Remove(Quantity))
                    {
                        SendInventoryMessage(ItemState.Update, target.Asset, target.Quantity);
                        return true;
                    }
                    else
                    {
                        if (invitem.CanDropp(this))
                        {
                            if (Items.Remove(itemName))
                            {
                                SendInventoryMessage(ItemState.Delete, null, 0);
                            }
                            else
                            {
                                Debug.LogError($"Can't Remove Item With this Name {itemName} [Be sure to use ItemTitle]");
                            }
                            return true;
                        }
                        else
                        {
                            SendInventoryMessage(ItemState.Update, target.Asset, 0);
                            return true;
                        }

                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Debug.LogWarning($"Ther isnt any Item with this Name {itemName} [Be sure to use ItemTitle]");
                return false;
            }
        }
        public bool DeleteItem(string itemName)
        {
            if (Items.ContainsKey(itemName))
            {
                bool Result = Items.Remove(itemName);
                if (Result)
                {
                    SendInventoryMessage(ItemState.Delete, null, 0);
                }
                return Result;
            }
            else
            {
                Debug.LogWarning($"Ther isnt any Item With this Name {itemName} [Be sure to use ItemTitle]");
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
        protected InventoryItemProperty[] GetCopyItemsProperty()
        {
            var values = Items.GetValues();
            var copy = new InventoryItemProperty[Items.Count];
            values.CopyTo(copy, 0);
            return copy;
        }
        public void Clear()
        {
            Items.Clear();
        }

        // private Methods
        private IInventoryItem GetInterface(ItemAsset item)
        {
            if (item is IInventoryItem InvItem)
            {
                return InvItem;
            }
            else
            {
                Debug.LogWarning("Your item can use for Inventory [IInventoryItem interface not implemented]");
                return null;
            }
        }
        private void SendInventoryMessage(ItemState state, ItemAsset target, int quantity)
        {
            IInventoryItem invitem = GetInterface(target);
            switch (state)
            {
                case ItemState.Create:
                    invitem.PickedUp(this, quantity);
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
                        case BehaviorType.ActionAndMessage:
                            OnItemAdded?.Invoke(target, quantity);
                            gameObject.SendMessage("OnItemAdded", target, SendMessageOptions.RequireReceiver);
                            break;
                        default:
                            break;
                    }
                    AddItem(target);
                    break;
                case ItemState.Update:
                    invitem.Cahanged(quantity);
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
                        case BehaviorType.ActionAndMessage:
                            OnItemUpdated?.Invoke(target, quantity);
                            gameObject.SendMessage("OnItemUpdated", target, SendMessageOptions.RequireReceiver);
                            break;
                        default:
                            break;
                    }
                    UpdateItem(target, quantity);
                    break;
                case ItemState.Delete:
                    invitem.Dropped(this);
                    switch (Behavior)
                    {
                        case BehaviorType.Action:
                            OnItemRemoved?.Invoke();
                            break;
                        case BehaviorType.SendMessage:
                            gameObject.SendMessage("OnItemRemoved", SendMessageOptions.RequireReceiver);
                            break;
                        case BehaviorType.Broadcast:
                            gameObject.BroadcastMessage("OnItemRemoved", SendMessageOptions.RequireReceiver);
                            break;
                        case BehaviorType.ActionAndMessage:
                            OnItemRemoved?.Invoke();
                            gameObject.SendMessage("OnItemRemoved", SendMessageOptions.RequireReceiver);
                            break;
                        default:
                            break;
                    }
                    RemoveItem();
                    break;
            }
        }

        // Abstract Methods 
        public abstract bool ReadFromSaveFile(SaveFile file);
        public abstract bool WriteToSaveFile(SaveFile file);
        protected abstract bool UseDefaultItem();
        protected abstract void AddItem(ItemAsset target);
        protected abstract void UpdateItem(ItemAsset target, int Quantity);
        protected abstract void RemoveItem();

    }
    public abstract class InventoryStorage : Inventory
    {
        [Header("Save")]
        [SerializeField]
        private bool ReadOnAwake = false;
        [SerializeField]
        private SaveFile _SaveFile;
        public SaveFile File => _SaveFile;


        // override Methods
        public sealed override bool ReadFromSaveFile(SaveFile file)
        {
            if (file is IInventorySave Inventoryfile)
            {
                if (Inventoryfile.IsExistInventoryData(this))
                {
                    IsLoading = true;
                    Clear();
                    foreach (var property in Inventoryfile.ReadInventoryData(this))
                    {
                        AddItem(property.Asset, property.Quantity);
                    }
                    IsLoading = false;
                    OnReaded();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Debug.LogWarning("IInventorySave Interface not implemented in Savefile. ReadFile Failed");
                return false;
            }
        }
        public sealed override bool WriteToSaveFile(SaveFile file)
        {
            if (file is IInventorySave Inventoryfile)
            {
                Inventoryfile.WriteInventoryData(this, GetCopyItemsProperty());
                OnWrited();
                return true;
            }
            else
            {
                Debug.LogWarning("IInventorySave Interface not implemented in Savefile. WriteFile Failed");
                return false;
            }
        }
        protected sealed override bool UseDefaultItem()
        {
            if (ReadOnAwake)
            {
                if (_SaveFile)
                {
                    return !ReadFromSaveFile(_SaveFile);
                }
                else
                {
                    Debug.LogError("SaveFile Is Not Valid for ReadFile");
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        // Abstract Methods 
        protected abstract void OnReaded();
        protected abstract void OnWrited();
    }

}
