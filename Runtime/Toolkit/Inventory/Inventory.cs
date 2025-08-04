using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{

    public class InventoryItemProperty
    {
        [SerializeField]
        private IInventoryItem Item;
        public IInventoryItem provider => Item;
        public string Name => provider.NameID;
        [SerializeField]
        private int ItemQuantity;
        public int Quantity => ItemQuantity;
        [SerializeField]
        private int ItemCapacity;
        public int Capacity => ItemCapacity;

        public InventoryItemProperty(IInventoryItem item, int quantity, int capacity)
        {
            Item = item;
            ItemQuantity = quantity;
            ItemCapacity = capacity;
        }
        public InventoryItemProperty(IInventoryItem item, int quantity)
        {
            Item = item;
            ItemQuantity = quantity;
            ItemCapacity = 0;
        }

        public bool Add(int value = 1)
        {
            ItemQuantity = ItemQuantity + value;
            if (Capacity == 0 || ItemQuantity <= Capacity)
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
        public System.Action<IInventoryItem, int> OnItemAdded;
        public System.Action<IInventoryItem, int> OnItemUpdated;
        public System.Action OnItemRemoved;

        // Private Variable
        private Hictionary<InventoryItemProperty> Items;
        public int Count => Items.IsValid() ? Items.Count : 0;
        protected IInventoryStorage inventoryStorage { get; private set; }



        public IInventoryItem this[string itemName]
        {
            get => Items[itemName].provider;
        }

        // Unity Methods
        private void Awake()
        {
            inventoryStorage = GetStorage();

            if (inventoryStorage == null)
            {
                Debug.LogWarning("Storage is Not Valid");
                enabled = false;
                return;
            }

            if (LoadStorage())
            {
                InventoryItemProperty[] cacheItems = inventoryStorage.GetItems();
                Items = new Hictionary<InventoryItemProperty>(cacheItems.Length);
                foreach (var data in cacheItems)
                {
                    Items.Add(data.Name, data);
                }
            }
            else
            {
                if (DefaultItem != null)
                {
                    Items = new Hictionary<InventoryItemProperty>(DefaultItem.Length);
                    foreach (var itemAsset in DefaultItem)
                    {
                        if (itemAsset is IInventoryItem item)
                        {
                            ItemAdded(item);
                        }
                        else
                        {
                            Debug.LogWarning("ItemAsset {itemAsset} is not implement IInventoryItem");
                        }

                    }
                }
                else
                {
                    Items = new Hictionary<InventoryItemProperty>(5);
                }
            }
        }

        // Public Functions
        public bool TryFindItem(string itemTitle, out IInventoryItem item)
        {
            if (Items.ContainsKey(itemTitle))
            {
                item = Items[itemTitle].provider;
                return true;
            }
            else
            {
                item = default;
                return false;
            }
        }
        public int GetQuantity(IInventoryItem item)
        {
            if (Items.ContainsKey(item.NameID))
            {
                return Items[item.NameID].Quantity;
            }
            else
            {
                return 0;
            }
        }
        public bool IsValidItem(IInventoryItem item)
        {
            return Items.ContainsKey(item.NameID);
        }
        public bool AddNewItem(IInventoryItem item, int quantity, int capacity)
        {
            if (!Items.ContainsKey(item.NameID))
            {
                if (item.CanPickUp(this))
                {
                    InventoryItemProperty NewItem = new InventoryItemProperty(item, quantity, capacity);
                    Items.Add(item.NameID, NewItem);
                    inventoryStorage.CreateItem(NewItem);
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
                Debug.LogWarning($"Item with this Name {item.NameID} already there");
                return false;
            }
        }
        public bool AddItem(IInventoryItem item, int quantity = 1)
        {
            if (_capacity == 0 || Items.Count < _capacity)
            {
                if (Items.ContainsKey(item.NameID))
                {
                    if (item.CanChange(true))
                    {
                        Items[item.NameID].Add(quantity);
                        inventoryStorage.UpdateQuantity(item, quantity);
                        MessageBehavior(ItemState.Update, item, quantity);
                    }
                }
                else
                {
                    if (item.CanPickUp(this))
                    {
                        InventoryItemProperty NewItem = new InventoryItemProperty(item, quantity);
                        Items.Add(item.NameID, NewItem);
                        inventoryStorage.CreateItem(NewItem);
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
        public bool SetItemCapacity(IInventoryItem item, int newCpacity)
        {
            InventoryItemProperty target;
            if (Items.TryGetValue(item.NameID, out target))
            {
                inventoryStorage.UpdateCapacity(item, newCpacity);
                target.NewCpacity(newCpacity);
                return true;
            }
            else
            {
                Debug.LogError($"Can't Find Item With this Name {item.NameID}");
                return false;
            }
        }
        public bool RemoveItem(IInventoryItem item, int quantity = 1)
        {
            InventoryItemProperty target;
            if (Items.TryGetValue(item.NameID, out target))
            {
                if (item.CanChange(true))
                {
                    if (target.Remove(quantity))
                    {
                        inventoryStorage.UpdateQuantity(item, -quantity);
                        MessageBehavior(ItemState.Update, item, target.Quantity);
                        return true;
                    }
                    else
                    {
                        if (item.CanDropp(this))
                        {
                            if (Items.Remove(item.NameID))
                            {
                                inventoryStorage.DestroyItem(item);
                                MessageBehavior(ItemState.Delete, null, 0);
                            }
                            else
                            {
                                Debug.LogError($"Can't Remove Item With this item {item}");
                            }
                            return true;
                        }
                        else
                        {
                            inventoryStorage.UpdateQuantity(item, 0);
                            MessageBehavior(ItemState.Update, item, 0);
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
                Debug.LogWarning($"Ther isnt any Item with this item {item}");
                return false;
            }
        }
        public bool DeleteItem(IInventoryItem item)
        {
            if (Items.ContainsKey(item.NameID))
            {
                bool Result = Items.Remove(item.NameID);
                if (Result)
                {
                    inventoryStorage.DestroyItem(item);
                    MessageBehavior(ItemState.Delete, null, 0);
                }
                return Result;
            }
            else
            {
                Debug.LogWarning($"Ther isnt any Item With this item {item}");
                return false;
            }
        }
        public T[] CopyItemsByClass<T>() where T : IInventoryItem
        {
            List<T> Result = new List<T>();
            foreach (var pack in Items.GetValues())
            {
                if (pack.provider is T finditem)
                {
                    Result.Add(finditem);
                }
            }
            return Result.ToArray();
        }
        public void Clear()
        {
            Items.Clear();
            inventoryStorage.StorageClear();
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
        private void MessageBehavior(ItemState state, IInventoryItem target, int quantity)
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
                    ItemAdded(target);
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
                    ItemUpdated(target, quantity);
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
                    ItemRemoved();
                    break;
            }
        }

        // Abstract Methods 
        protected abstract void ItemAdded(IInventoryItem target);
        protected abstract void ItemUpdated(IInventoryItem target, int Quantity);
        protected abstract void ItemRemoved();
        protected abstract IInventoryStorage GetStorage();
        protected abstract bool LoadStorage();
    }

    public abstract class InventoryStorage : Inventory
    {
        [Header("Save")]
        [SerializeField]
        private StorageFile<IInventoryStorage, InventorySaveFile> storage;
        public SaveFile file => storage.file;

        // override Methods
        protected sealed override IInventoryStorage GetStorage()
        {
            return storage.provider;
        }
        protected sealed override bool LoadStorage()
        {
            return storage.Load(this);
        }
    }

}
