using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField]
        private BehaviorType Behavior;
        [SerializeField]
        private ItemAsset[] DefaultItem;
        [Header("Save")]
        [SerializeField]
        private bool LoadOnAwake = false;
        [SerializeField, ConditionalHide("LoadOnAwake", true, true)]
        protected SaveFile SaveSlot;

        public int Capacity => _capacity;
        public bool IsEnoughCapacity => _capacity > 0 ? Items.Count < _capacity : true;
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

            if (LoadOnAwake)
            {
                DataManager Data = Game.FindManager<DataManager>();
                if (Data)
                {
                    Data.LoadFile();
                    SaveSlot = Data.File;
                }
                else
                {
                    Debug.LogError("Cant Find 'DataManager' for load");
                }
            }

            if (SaveSlot)
            {
                LoadInventory();
            }
            else
            {
                Debug.LogError($"Save File is Not Valid in Inventory: {gameObject.name}");
            }
        }


        public ItemAsset this[string Name]
        {
            get => Items[Name].Asset;
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
                    Items.AddItem(item.Name, new ItemPack(item, Quantity, ItemCapacity));
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
                        Items.AddItem(item.Name, new ItemPack(item, Quantity));
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
            ItemPack target;
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
        public bool LoadInventory()
        {
            if (SaveSlot is IInventoryData File)
            {
                if (File.IsExistInventoryData(this))
                {
                    LoadItems(File);
                }
                return true;
            }
            else
            {
                Debug.LogWarning("IInventoryData not implemented in Savefile. Load Failed");
                return false;
            }
        }
        public bool SaveInventory()
        {
            if (SaveSlot)
            {
                if (SaveSlot is IInventoryData File)
                {
                    SaveItems(File);
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

        // protected Methods 
        protected virtual void LoadItems(IInventoryData File)
        {
            Clear();
            var allItems = Resources.LoadAll<ItemAsset>("");
            var itemDict = allItems.ToDictionary(item => item.Name, item => item);

            foreach (var propertie in File.LoadInventory(this))
            {
                if (itemDict.TryGetValue(propertie.Name, out var asset))
                {
                    AddItem(asset, propertie.Quantity);
                }
                else
                {
                    Debug.LogWarning($"ItemAsset with name '{propertie.Name}' not found in Resources.");
                }
            }
        }
        protected virtual void SaveItems(IInventoryData File)
        {
            List<IInventoryData.ItemPropertie> itemsProperty = new List<IInventoryData.ItemPropertie>();
            foreach (var item in Items.GetValues())
            {
                itemsProperty.Add(new IInventoryData.ItemPropertie(item.Asset, item.Quantity));
            }
            File.SaveInventory(this, itemsProperty.ToArray());
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
        public abstract bool CanPickUp(Inventory owner);
        public abstract bool CanDropp(Inventory owner);
    }
    public abstract class ItemAsset<T> : ItemAsset where T : Enum
    {
        [Header("Category")]
        [SerializeField]
        protected T Type;
    }

    public interface IInventoryData
    {
        [Serializable]
        public struct ItemPropertie
        {
            [SerializeField]
            private string ItemName;
            public string Name => ItemName;
            public Type ItemType { get; private set; }
            [SerializeField]
            private int ItemQuantity;
            public int Quantity => ItemQuantity;

            public ItemPropertie(ItemAsset Item, int quantity)
            {
                ItemType = Item.GetType();
                ItemName = Item.Name;
                ItemQuantity = quantity;
            }
        }

        ItemPropertie[] LoadInventory(Inventory owner);
        void SaveInventory(Inventory owner, ItemPropertie[] Data);
        bool IsExistInventoryData(Inventory owner);
    }

}
