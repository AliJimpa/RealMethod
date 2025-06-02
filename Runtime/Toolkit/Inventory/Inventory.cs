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
            Broadcast
        }

        [Header("Setting")]
        [SerializeField, Tooltip("Zero means No Limit")]
        private int _capacity = 0;
        [SerializeField]
        private BehaviorType Behavior;
        [SerializeField]
        private ItemAsset[] DefaultItem;

        public int Capacity => _capacity;
        public bool IsEnoughCapacity => _capacity > 0 ? Items.Count < _capacity : true;
        public int Count => Items.Count;
        public Action<ItemAsset, int> OnItemAdded;
        public Action<ItemAsset, int> OnItemUpdated;
        public Action OnItemRemove;

        protected HashedKeyItem<ItemProperty> Items = new HashedKeyItem<ItemProperty>(5);


        public ItemAsset this[string Name]
        {
            get => Items[Name].Asset;
        }

        private void Awake()
        {
            if (AddDefaultItem())
            {
                if (DefaultItem != null)
                {
                    foreach (var item in DefaultItem)
                    {
                        AddItem(item);
                    }
                }
            }

            PostAwake();
        }

        // public methods
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
                if (item.CanPickUp(this))
                {
                    Items.AddItem(item.Name, new ItemProperty(item, Quantity, ItemCapacity));
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
                    if (item.CanChange(true))
                    {
                        Items[item.Name].Add(Quantity);
                        SendInventoryMessage(ItemState.Update, item, Quantity);
                    }
                }
                else
                {
                    if (item.CanPickUp(this))
                    {
                        Items.AddItem(item.Name, new ItemProperty(item, Quantity));
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
        public bool RemoveItem(string Name, int Quantity = 1)
        {
            ItemProperty target;
            if (Items.TryGetItem(Name, out target))
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
                            if (Items.RemoveItem(Name))
                            {
                                SendInventoryMessage(ItemState.Delete, null, 0);
                            }
                            else
                            {
                                Debug.LogError($"Can't Remove Item With this Name {Name}");
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
        public void Clear()
        {
            Items.Clear();
        }

        // private Methods
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
                    RemoveItem();
                    break;
            }
        }

        // Abstract Methods 
        protected abstract void PostAwake();
        protected abstract bool AddDefaultItem();
        protected abstract void AddItem(ItemAsset target);
        protected abstract void UpdateItem(ItemAsset target, int Quantity);
        protected abstract void RemoveItem();
        public abstract bool Load();
        public abstract bool Save();
    }
    public abstract class InventoryStorage : Inventory
    {
        [Header("Save")]
        [SerializeField]
        private bool LoadOnAwake = false;
        [SerializeField]
        protected SaveFile SaveSlot;

        // override Methods
        public sealed override bool Load()
        {
            if (SaveSlot is IInventoryData File)
            {
                if (File.IsExistInventoryData(this))
                {
                    Clear();
                    foreach (var property in File.LoadInventory(this))
                    {
                        AddItem(property.Asset, property.Quantity);
                    }
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
            if (SaveSlot)
            {
                if (SaveSlot is IInventoryData File)
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
        protected sealed override bool AddDefaultItem()
        {
            if (LoadOnAwake)
            {
                if (SaveSlot)
                {
                    return !Load();
                }
                else
                {
                    DataManager Data = Game.FindManager<DataManager>();
                    if (Data)
                    {
                        Data.LoadFile();
                        SaveSlot = Data.File;
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
        public abstract bool CanPickUp(Inventory owner);
        public abstract bool CanDropp(Inventory owner);
    }
    public abstract class ItemAsset<T> : ItemAsset where T : Enum
    {
        [Header("Category")]
        [SerializeField]
        protected T Type;
    }

    public class ItemProperty
    {
        [SerializeField]
        private string ItemName;
        [SerializeField]
        private ItemAsset ItemAsset;
        [SerializeField]
        private int ItemQuantity;
        [SerializeField]
        private int ItemCapacity;
        // Just for Getter Serialize Variable
        public ItemAsset Asset => ItemAsset;
        public int Quantity => ItemQuantity;
        public int Capacity => ItemCapacity;

        public ItemProperty(ItemAsset asset, int quantity, int capacity = 0)
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

    public interface IInventoryData
    {
        ItemProperty[] LoadInventory(Inventory owner);
        void SaveInventory(Inventory owner, ItemProperty[] Data);
        bool IsExistInventoryData(Inventory owner);

    }

}
