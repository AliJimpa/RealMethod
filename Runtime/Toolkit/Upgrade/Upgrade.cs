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
                    Items.Add(MapItems[it].Label, MapItems[it]);
                    CurrentID++;
                }
            }

            upgradeStorage = GetStorage();
            if (LoadStorage())
            {
                // Load Unlocked Asset
                foreach (var id in upgradeStorage.GetUnlockItems())
                {
                    Items[id].Sync(true);
                }
                // Load AvailableItems
                string[] items = upgradeStorage.GetAvailableItems();
                AvailableItems = new List<IUpgradeItem>(items.Length);
                foreach (var item in items)
                {
                    AvailableItems.Add(FindItem(item));
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
            return IsValidItem(item.Label);
        }
        public bool IsValidItem(string itemLabel)
        {
            return Items.ContainsKey(itemLabel);
        }
        public bool CanUnlock(IUpgradeItem item, bool free = false)
        {
            return CanUnlock(item.Label, free);
        }
        public bool CanUnlock(string itemLabel, bool free = false)
        {
            if (!Items.ContainsKey(itemLabel))
            {
                Debug.LogWarning("Item Not Found");
                return false;
            }
            return Items[itemLabel].Prerequisites(!free);
        }
        public bool IsUnlocked(IUpgradeItem item)
        {
            return IsUnlocked(item.Label);
        }
        public bool IsUnlocked(string itemLabel)
        {
            if (!Items.ContainsKey(itemLabel))
            {
                Debug.LogWarning("Item Not Found");
                return false;
            }
            return Items[itemLabel].IsUnlocked;
        }
        public bool SetUnlock(IUpgradeItem item, bool free = false)
        {
            return SetUnlock(item.Label, free);
        }
        public bool SetUnlock(string itemLabel, bool free = false)
        {
            if (!Items.ContainsKey(itemLabel))
            {
                Debug.LogWarning("Item Not Found");
                return false;
            }
            IUpgradeItem item = Items[itemLabel];
            if (item.IsUnlocked)
            {
                Debug.LogWarning("Item already is unlocked");
                return false;
            }
            if (item.Prerequisites(!free))
            {
                item.Unlock(!free);
                AvailableItems.Remove(item);
                upgradeStorage.RemoveAvalibelItem(item);
                if (item.GetNextAvailables() != null)
                {
                    foreach (var nextitem in item.GetNextAvailables())
                    {
                        if (!nextitem.IsUnlocked)
                        {
                            AvailableItems.Add(nextitem);
                            upgradeStorage.AddAvailableItem(nextitem);
                        }
                    }
                }
                upgradeStorage.UnlockItem(item);
                MessageBehavior(item, true);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool SetLock(IUpgradeItem item)
        {
            return SetLock(item.Label);
        }
        public bool SetLock(string itemLabel)
        {
            if (!Items.ContainsKey(itemLabel))
            {
                Debug.LogWarning("Item Not Found");
                return false;
            }
            IUpgradeItem item = Items[itemLabel];
            if (item.IsUnlocked)
            {
                item.Lock();
                AvailableItems.Add(item);
                upgradeStorage.AddAvailableItem(item);
                if (item.GetNextAvailables() != null)
                {
                    foreach (var nextitem in item.GetNextAvailables())
                    {
                        AvailableItems.Remove(nextitem);
                        upgradeStorage.RemoveAvalibelItem(nextitem);
                    }
                }
                upgradeStorage.LockItem(item);
                MessageBehavior(item, false);
                return true;
            }
            else
            {
                Debug.LogWarning("Item already is locked");
                return false;
            }

        }
        public IUpgradeItem FindItem(string itemLabel)
        {
            if (!Items.ContainsKey(itemLabel))
            {
                Debug.LogWarning("Item Not Found");
                return null;
            }
            return Items[itemLabel];
        }
        public IUpgradeItem[] GetItems()
        {
            return Items.Values.ToArray();
        }
        public IUpgradeItem[] GetItems(string configLabel)
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
        public IUpgradeItem GetAvailableItem(UpgradeMapConfig config)
        {
            return GetAvailableItem(config.Label);
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
            return IsAvailable(item);
        }
        public bool IsAvailable(string itemLabel)
        {
            foreach (var item in AvailableItems)
            {
                if (item.Label == itemLabel)
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
                    Items.Add(MapItems[it].Label, MapItems[it]);
                    CurrentID++;
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
        public UpgradeMapConfig GetConfig(string configLabel)
        {
            foreach (var conf in Maps)
            {
                if (conf.Label == configLabel)
                {
                    return conf;
                }
            }
            return null;
        }
        public UpgradeMapConfig GetConfig(int index)
        {
            return Maps[index];
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