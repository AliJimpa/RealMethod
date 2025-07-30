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
        public System.Action<IUpgradeItem> OnUnlocked;
        public System.Action<IUpgradeItem> OnLocked;

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
                    MapItems[it].Identify(Maps[m], CurrentID);
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
                    }
                }
            }
        }


        // Public functions
        public bool IsUnlocked(IUpgradeItem item)
        {
            return IsUnlocked(item.Label);
        }
        public bool IsUnlocked(string itemLabel)
        {
            return Items[itemLabel].IsUnlocked;
        }
        public bool SetUnlock(IUpgradeItem item, bool free = false)
        {
            return SetUnlock(item.Label, free);
        }
        public bool SetUnlock(string itemLabel, bool free = false)
        {
            IUpgradeItem item = Items[itemLabel];
            if (item.Prerequisites(!free))
            {
                item.Unlock(!free);
                foreach (var nextitem in item.GetNextAvailables())
                {
                    AvailableItems.Add(nextitem);
                    upgradeStorage.AddAvailableItem(nextitem);
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
            IUpgradeItem item = Items[itemLabel];
            if (item.IsUnlocked)
            {
                item.Lock();
                foreach (var nextitem in item.GetNextAvailables())
                {
                    AvailableItems.Remove(nextitem);
                    upgradeStorage.RemoveAvalibelItem(nextitem);
                }
                upgradeStorage.LockItem(item);
                MessageBehavior(item, false);
                return true;
            }
            return false;
        }
        public IUpgradeItem FindItem(string itemLabel)
        {
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
        public IUpgradeItem GetAvailableItem(string configLabel)
        {
            UpgradeMapConfig targetConfig = GetConfigByName(configLabel);
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
            upgradeStorage.StorageClear();
        }

        // Protected Functions
        protected UpgradeMapConfig GetConfigByName(string configLabel)
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

        // Private Functions
        private void MessageBehavior(IUpgradeItem item, bool isUnlock)
        {
            if (Behavior == UpgradeBehavior.Action || Behavior == UpgradeBehavior.Both)
            {
                if (isUnlock)
                {
                    OnUnlocked?.Invoke(item);
                }
                else
                {
                    OnLocked?.Invoke(item);
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