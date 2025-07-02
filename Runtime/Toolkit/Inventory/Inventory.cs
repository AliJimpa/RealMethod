using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
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
        [SerializeField]
        private BehaviorType Behavior;
        [SerializeField]
        private InventoryItemAsset[] DefaultItem;

        public bool IsLoading { get; protected set; }
        public bool IsEnoughCapacity => _capacity > 0 ? Items.Count < _capacity : true;
        public int Count => Items.Count;
        public Action<InventoryItemAsset, int> OnItemAdded;
        public Action<InventoryItemAsset, int> OnItemUpdated;
        public Action OnItemRemoved;

        protected Hictionary<InventoryItemProperty> Items = new Hictionary<InventoryItemProperty>(5);


        public InventoryItemAsset this[string itemname]
        {
            get => Items[itemname].Asset;
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
        public int GetQuantity(string itemname)
        {
            if (Items.ContainsKey(itemname))
            {
                return Items[itemname].Quantity;
            }
            else
            {
                Debug.LogWarning("Ther isnt any item with this Name [Be sure to use ItemName or Object.name]");
                return -1;
            }
        }
        public int GetQuantity(InventoryItemAsset asset)
        {
            return GetQuantity(asset.name);
        }
        public bool IsValidItem(string itemname)
        {
            return Items.ContainsKey(itemname);
        }
        public bool IsValidItem(InventoryItemAsset asset)
        {
            return Items.ContainsKey(asset.name);
        }
        public bool AddNewItem(InventoryItemAsset item, int Quantity, int ItemCapacity)
        {
            if (!Items.ContainsKey(item.name))
            {
                if (item.CanPickUp(this))
                {
                    Items.Add(item.name, new InventoryItemProperty(item, Quantity, ItemCapacity));
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
                Debug.LogWarning($"Item with this Name {item.name} already there");
                return false;
            }
        }
        public bool AddItem(InventoryItemAsset item, int Quantity = 1)
        {
            if (Items.Count < _capacity || _capacity == 0)
            {
                if (Items.ContainsKey(item.name))
                {
                    if (item.CanChange(true))
                    {
                        Items[item.name].Add(Quantity);
                        SendInventoryMessage(ItemState.Update, item, Quantity);
                    }
                }
                else
                {
                    if (item.CanPickUp(this))
                    {
                        Items.Add(item.name, new InventoryItemProperty(item, Quantity));
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
        public bool RemoveItem(string itemname, int Quantity = 1)
        {
            InventoryItemProperty target;
            if (Items.TryGetValue(itemname, out target))
            {
                if (target.Asset.CanChange(true))
                {
                    if (target.Remove(Quantity))
                    {
                        SendInventoryMessage(ItemState.Update, target.Asset, target.Quantity);
                        return true;
                    }
                    else
                    {
                        if (target.Asset.CanDropp(this))
                        {
                            if (Items.Remove(itemname))
                            {
                                SendInventoryMessage(ItemState.Delete, null, 0);
                            }
                            else
                            {
                                Debug.LogError($"Can't Remove Item With this Name {itemname} [Be sure to use ItemName or Object.name]");
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
                Debug.LogWarning($"Ther isnt any Item with this Name {itemname} [Be sure to use ItemName or Object.name]");
                return false;
            }
        }
        public bool DeleteItem(string itemname)
        {
            if (Items.ContainsKey(itemname))
            {
                bool Result = Items.Remove(itemname);
                if (Result)
                {
                    SendInventoryMessage(ItemState.Delete, null, 0);
                }
                return Result;
            }
            else
            {
                Debug.LogWarning($"Ther isnt any Item With this Name {itemname} [Be sure to use ItemName or Object.name]");
                return false;
            }
        }
        public T[] CopyItemsByClass<T>() where T : InventoryItemAsset
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
        public T CopyItemByClass<T>() where T : InventoryItemAsset
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
        public void Clear()
        {
            Items.Clear();
        }

        // private Methods
        private void SendInventoryMessage(ItemState state, InventoryItemAsset target, int quantity)
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
                    target.Dropped(this);
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
        public abstract bool Load();
        public abstract bool Save();
        protected abstract bool UseDefaultItem();
        protected abstract void AddItem(InventoryItemAsset target);
        protected abstract void UpdateItem(InventoryItemAsset target, int Quantity);
        protected abstract void RemoveItem();

    }
    public abstract class InventoryStorage : Inventory
    {
        [Header("Save")]
        [SerializeField]
        private bool LoadOnAwake = false;
        [SerializeField]
        private SaveFile _SaveFile;

        public SaveFile SaveSlot => _SaveFile;

        // Protected Methods
        protected void SetSaveFile(SaveFile NewFile)
        {
            _SaveFile = NewFile;
        }

        // override Methods
        public sealed override bool Load()
        {
            if (_SaveFile is IInventorySave File)
            {
                if (File.IsExistInventoryData(this))
                {
                    IsLoading = true;
                    Clear();
                    foreach (var property in File.LoadInventory(this))
                    {
                        AddItem(property.Asset, property.Quantity);
                    }
                    IsLoading = false;
                    OnLoaded();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Debug.LogWarning("IInventoryData not implemented in Savefile. Load Failed");
                return false;
            }
        }
        public sealed override bool Save()
        {
            if (_SaveFile)
            {
                if (_SaveFile is IInventorySave File)
                {
                    File.SaveInventory(this, Items.GetValues());
                    OnSaved();
                    return true;
                }
                else
                {
                    Debug.LogWarning("IInventoryData not implemented in Savefile. Load Failed");
                    return false;
                }
            }
            else
            {
                Debug.LogError($"Save File is Not Valid in Inventory: {gameObject.name}");
                return false;
            }
        }
        protected sealed override bool UseDefaultItem()
        {
            if (LoadOnAwake)
            {
                if (_SaveFile)
                {
                    return !Load();
                }
                else
                {
                    DataManager Data = Game.FindManager<DataManager>();
                    if (Data)
                    {
                        Data.LoadFile(_SaveFile);
                        return !Load();
                    }
                    else
                    {
                        Debug.LogError("Cant Find 'DataManager' for load");
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }
        }

        // Abstract Methods 
        protected abstract void OnLoaded();
        protected abstract void OnSaved();
    }


}
