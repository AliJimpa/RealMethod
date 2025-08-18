using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealMethod
{
    public abstract class Upgrade : MonoBehaviour
    {
        private enum UpgradeBehavior
        {
            Nothing,
            SendMessage,
            Action,
            Both,
        }
        [Header("Setting")]
        [SerializeField]
        private UpgradeMapConfig[] Maps;
        [SerializeField]
        private UpgradeBehavior Behavior;
        public System.Action<IUpgradeItem> OnItemUnlocked;
        public System.Action<IUpgradeItem> OnItemLocked;

        private List<IUpgradeItem> AvailableItems;
        public int AvailableCount => AvailableItems != null ? AvailableItems.Count : 0;
        private Dictionary<string, IUpgradeItem> Items;
        public int ItemCount => Items != null ? Items.Count : 0;
        private IUpgradeStorage upgradeStorage;

        // Unity Methods
        protected void Awake()
        {
            Items = new Dictionary<string, IUpgradeItem>();

            // Identify Items
            int CurrentID = 1;
            for (int m = 0; m < Maps.Length; m++)
            {
                IUpgradeItem[] MapItems = Maps[m].provider.GenerateItems(this);
                for (int it = 0; it < MapItems.Length; it++)
                {
                    MapItems[it].Identify(Maps[m], m, it, CurrentID);
                    if (!Items.ContainsKey(MapItems[it].Label))
                    {
                        Items.Add(MapItems[it].Label, MapItems[it]);
                        CurrentID++;
                    }
                    else
                    {
                        Debug.LogWarning($"Item Can't Add , Item with this name:{MapItems[it].Label} is already exsit");
                    }
                }
            }

            upgradeStorage = GetStorage();
            if (LoadStorage())
            {
                // Load Unlocked Asset
                foreach (var id in upgradeStorage.GetUnlockItems())
                {
                    if (Items.ContainsKey(id))
                    {
                        Items[id].Sync(true);
                    }
                }
                // Load AvailableItems
                string[] itemsName = upgradeStorage.GetAvailableItems();
                AvailableItems = new List<IUpgradeItem>(itemsName.Length);
                foreach (var itemN in itemsName)
                {
                    AvailableItems.Add(Items[itemN]);
                }
            }
            else
            {
                // Create AvailableItems
                AvailableItems = new List<IUpgradeItem>(Maps.Length);
                foreach (var conf in Maps)
                {
                    IUpgradeItem item = conf.provider.GetStartItem();
                    if (item != null)
                    {
                        AvailableItems.Add(item);
                        upgradeStorage.AddAvailableItem(item);
                    }
                }
            }
        }


        // Public functions
        public bool IsValidItem(IUpgradeItem item)
        {
            return Items.ContainsKey(item.Label);
        }
        public bool CanUnlock(IUpgradeItem item, bool free = false)
        {
            if (!Items.ContainsKey(item.Label))
            {
                Debug.LogWarning("Item Not Found");
                return false;
            }
            return Items[item.Label].Prerequisites(!free);
        }
        public bool IsUnlocked(IUpgradeItem item)
        {
            if (!Items.ContainsKey(item.Label))
            {
                Debug.LogWarning("Item Not Found");
                return false;
            }
            return Items[item.Label].IsUnlocked;
        }
        public bool SetUnlock(IUpgradeItem item, bool free = false)
        {
            if (!Items.ContainsKey(item.Label))
            {
                Debug.LogWarning("Item Not Found");
                return false;
            }
            IUpgradeItem TargetItem = Items[item.Label];
            if (TargetItem.IsUnlocked)
            {
                Debug.LogWarning("Item already is unlocked");
                return false;
            }
            if (TargetItem.Prerequisites(!free))
            {
                TargetItem.Unlock(!free);
                AvailableItems.Remove(TargetItem);
                upgradeStorage.RemoveAvalibelItem(TargetItem);
                if (TargetItem.GetNextAvailables() != null)
                {
                    foreach (var nextitem in TargetItem.GetNextAvailables())
                    {
                        if (!nextitem.IsUnlocked)
                        {
                            AvailableItems.Add(nextitem);
                            upgradeStorage.AddAvailableItem(nextitem);
                        }
                    }
                }
                upgradeStorage.UnlockItem(TargetItem);
                MessageBehavior(TargetItem, true);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool SetLock(IUpgradeItem item)
        {
            if (!Items.ContainsKey(item.Label))
            {
                Debug.LogWarning("Item Not Found");
                return false;
            }
            IUpgradeItem TargetItem = Items[item.Label];
            if (TargetItem.IsUnlocked)
            {
                TargetItem.Lock();
                AvailableItems.Add(TargetItem);
                upgradeStorage.AddAvailableItem(TargetItem);
                if (TargetItem.GetNextAvailables() != null)
                {
                    foreach (var nextitem in TargetItem.GetNextAvailables())
                    {
                        AvailableItems.Remove(nextitem);
                        upgradeStorage.RemoveAvalibelItem(nextitem);
                    }
                }
                upgradeStorage.LockItem(TargetItem);
                MessageBehavior(TargetItem, false);
                return true;
            }
            else
            {
                Debug.LogWarning("Item already is locked");
                return false;
            }
        }
        public bool TryFindItem(string label, out IUpgradeItem item)
        {
            if (!Items.ContainsKey(label))
            {
                item = null;
                return false;
            }
            item = Items[label];
            return true;
        }
        public IUpgradeItem[] GetItems()
        {
            return Items.Values.ToArray();
        }
        public IUpgradeItem[] GetConfigItems(UpgradeMapConfig config)
        {
            return GetConfigItems(config.Label);
        }
        public IUpgradeItem[] GetConfigItems(string configLabel)
        {
            List<IUpgradeItem> CacheItems = new List<IUpgradeItem>();
            foreach (var item in Items.Values.ToArray())
            {
                if (item.ConfigLabel == configLabel)
                {
                    CacheItems.Add(item);
                }
            }
            return CacheItems.ToArray();
        }
        public IUpgradeItem GetAvailableItem(int index)
        {
            return AvailableItems[index];
        }
        public IUpgradeItem GetAvailableItem(string configLabel)
        {
            foreach (var avitem in AvailableItems)
            {
                if (avitem.ConfigLabel == configLabel)
                {
                    return avitem;
                }
            }
            return null;
        }
        public bool IsAvailable(IUpgradeItem item)
        {
            foreach (var avItem in AvailableItems)
            {
                if (avItem.Label == item.Label)
                {
                    return true;
                }
            }
            return false;
        }
        public void Clear()
        {
            Items = new Dictionary<string, IUpgradeItem>();

            // Identify Items
            int CurrentID = 1;
            for (int m = 0; m < Maps.Length; m++)
            {
                IUpgradeItem[] MapItems = Maps[m].provider.GenerateItems(this);
                for (int it = 0; it < MapItems.Length; it++)
                {
                    MapItems[it].Identify(Maps[m], m, it, CurrentID);
                    if (!Items.ContainsKey(MapItems[it].Label))
                    {
                        Items.Add(MapItems[it].Label, MapItems[it]);
                        CurrentID++;
                    }
                    else
                    {
                        Debug.LogWarning($"Item Can't Add , Item with this name:{MapItems[it].Label} is already exsit");
                    }
                }
            }
            upgradeStorage.StorageClear();
            // Create AvailableItems
            AvailableItems = new List<IUpgradeItem>(Maps.Length);
            foreach (var conf in Maps)
            {
                IUpgradeItem item = conf.provider.GetStartItem();
                if (item != null)
                {
                    AvailableItems.Add(item);
                    upgradeStorage.AddAvailableItem(item);
                }
            }
        }

        // Private Functions
        private void MessageBehavior(IUpgradeItem item, bool isUnlock)
        {
            if (Behavior == UpgradeBehavior.Action || Behavior == UpgradeBehavior.Both)
            {
                if (isUnlock)
                {
                    OnItemUnlocked?.Invoke(item);
                }
                else
                {
                    OnItemLocked?.Invoke(item);
                }
            }

            if (Behavior == UpgradeBehavior.SendMessage || Behavior == UpgradeBehavior.Both)
            {
                if (isUnlock)
                {
                    SendMessage("OnUnlocked", SendMessageOptions.RequireReceiver);
                }
                else
                {
                    SendMessage("OnLocked", SendMessageOptions.RequireReceiver);
                }
            }

            if (isUnlock)
            {
                UnlockedItem(item);
            }
            else
            {
                lockedItem(item);
            }
        }

        // Abstract Methods
        protected abstract void UnlockedItem(IUpgradeItem item);
        protected abstract void lockedItem(IUpgradeItem item);
        protected abstract IUpgradeStorage GetStorage();
        protected abstract bool LoadStorage();
    }
    public abstract class UpgradeStorage : Upgrade
    {
        [Header("Save")]
        [SerializeField]
        private StorageFile<IUpgradeStorage, UpgradeSaveFile> storage;
        public SaveFile file => storage.file;

        // override Methods
        protected sealed override IUpgradeStorage GetStorage()
        {
            return storage.provider;
        }
        protected sealed override bool LoadStorage()
        {
            return storage.Load(this);
        }
    }

}