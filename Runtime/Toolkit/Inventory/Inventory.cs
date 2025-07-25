using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{

    public class InventoryItemProperty
    {
        [SerializeField]
        private ItemAsset ItemAsset;
        public ItemAsset Asset => ItemAsset;
        public string Name => Asset.Title;
        [SerializeField]
        private int ItemQuantity;
        public int Quantity => ItemQuantity;
        [SerializeField]
        private int ItemCapacity;
        public int Capacity => ItemCapacity;

        public InventoryItemProperty(ItemAsset asset, int quantity, int capacity)
        {
            ItemAsset = asset;
            ItemQuantity = quantity;
            ItemCapacity = capacity;
        }
        public InventoryItemProperty(ItemAsset asset, int quantity)
        {
            ItemAsset = asset;
            ItemQuantity = quantity;
            ItemCapacity = 0;
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
        public void NewCpacity(int amount)
        {
            ItemCapacity = amount;
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

        // Actions
        public Action<ItemAsset, int> OnItemAdded;
        public Action<ItemAsset, int> OnItemUpdated;
        public Action OnItemRemoved;

        // Private Variable
        private Hictionary<InventoryItemProperty> Items = new Hictionary<InventoryItemProperty>(5);
        protected IInventoryStorage storage { get; private set; }
        public int Count => Items.Count;


        public ItemAsset this[string itemName]
        {
            get => Items[itemName].Asset;
        }

        // Unity Methods
        protected virtual void Awake()
        {
            storage = GetStorage();
            if (storage != null)
            {
                // Load or Create
                if (!IsStorageLoaded())
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
            else
            {
                Debug.LogWarning("Storage is Not Valid");
                enabled = false;
                return;
            }
        }

        // Public Functions
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
        public bool AddNewItem(ItemAsset item, int quantity, int capacity)
        {
            IInventoryItem invitem = GetInterface(item);
            if (invitem == null)
                return false;

            if (!Items.ContainsKey(item.Title))
            {
                if (invitem.CanPickUp(this))
                {
                    InventoryItemProperty NewItem = new InventoryItemProperty(item, quantity, capacity);
                    Items.Add(item.Title, NewItem);
                    storage.CreateItem(NewItem);
                    MessageBehavior(ItemState.Create, item, quantity);
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
        public bool AddItem(ItemAsset item, int quantity = 1)
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
                        Items[item.Title].Add(quantity);
                        storage.UpdateQuantity(item.Title, quantity);
                        MessageBehavior(ItemState.Update, item, quantity);
                    }
                }
                else
                {
                    if (invitem.CanPickUp(this))
                    {
                        InventoryItemProperty NewItem = new InventoryItemProperty(item, quantity);
                        Items.Add(item.Title, NewItem);
                        storage.CreateItem(NewItem);
                        MessageBehavior(ItemState.Create, item, quantity);
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
        public bool SetItemCapacity(string itemName, int newCpacity)
        {
            InventoryItemProperty target;
            if (Items.TryGetValue(itemName, out target))
            {
                storage.UpdateCapacity(newCpacity);
                target.NewCpacity(newCpacity);
                return true;
            }
            else
            {
                Debug.LogError($"Can't Find Item With this Name {itemName} [Be sure to use ItemTitle]");
                return false;
            }
        }
        public bool RemoveItem(string itemName, int quantity = 1)
        {
            InventoryItemProperty target;
            if (Items.TryGetValue(itemName, out target))
            {
                IInventoryItem invitem = GetInterface(target.Asset);
                if (invitem == null)
                    return false;

                if (invitem.CanChange(true))
                {
                    if (target.Remove(quantity))
                    {
                        storage.UpdateQuantity(itemName, -quantity);
                        MessageBehavior(ItemState.Update, target.Asset, target.Quantity);
                        return true;
                    }
                    else
                    {
                        if (invitem.CanDropp(this))
                        {
                            if (Items.Remove(itemName))
                            {
                                storage.DestroyItem(itemName);
                                MessageBehavior(ItemState.Delete, null, 0);
                            }
                            else
                            {
                                Debug.LogError($"Can't Remove Item With this Name {itemName} [Be sure to use ItemTitle]");
                            }
                            return true;
                        }
                        else
                        {
                            storage.UpdateQuantity(itemName, 0);
                            MessageBehavior(ItemState.Update, target.Asset, 0);
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
                    storage.DestroyItem(itemName);
                    MessageBehavior(ItemState.Delete, null, 0);
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
        public void Clear()
        {
            Items.Clear();
            storage.Clear();
        }

        // Protected Functions
        protected InventoryItemProperty[] GetCopyItemsProperty()
        {
            var values = Items.GetValues();
            var copy = new InventoryItemProperty[Items.Count];
            values.CopyTo(copy, 0);
            return copy;
        }

        // Private Functions
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
        private void MessageBehavior(ItemState state, ItemAsset target, int quantity)
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
        protected abstract void AddItem(ItemAsset target);
        protected abstract void UpdateItem(ItemAsset target, int Quantity);
        protected abstract void RemoveItem();
        protected abstract IInventoryStorage GetStorage();
        protected abstract bool IsStorageLoaded();

    }

    public abstract class InventoryStorage : Inventory
    {
        [Header("Save")]
        [SerializeField]
        private bool UseCustomFile = false;
        [SerializeField, ConditionalHide("UseCustomFile", true, false)]
        private SaveFile _SaveFile;
        public SaveFile file => _SaveFile;


        // override Methods
        protected sealed override IInventoryStorage GetStorage()
        {
            if (UseCustomFile)
            {
                if (_SaveFile is IInventoryStorage newstorage)
                {
                    return newstorage;
                }
                else
                {
                    Debug.LogWarning("IUpgradeStorage Interface not implemented in CustomSavefile.");
                    UseCustomFile = false;
                }

            }

            _SaveFile = ScriptableObject.CreateInstance<InventorySaveFile>();
            _SaveFile.name = "RMInventorySaveFile";
            return _SaveFile as IInventoryStorage;
        }
        protected sealed override bool IsStorageLoaded()
        {
            // Find Data Maanger 
            DataManager savesystem = Game.FindManager<DataManager>();
            if (savesystem != null)
            {
                if (savesystem.IsExistFile(_SaveFile))
                {
                    savesystem.LoadFile(_SaveFile);
                    foreach (var item in storage.GetItems())
                    {
                        AddNewItem(item.Asset, item.Quantity, item.Capacity);
                    }
                    return true;
                }
            }

            return false;
        }

    }

}
