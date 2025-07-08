using System;
using System.Collections.Generic;
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
        private UpgradeConfig[] Configs;
        [SerializeField]
        private UpgradeBehavior Behavior;


        public Action<UpgradeItem> OnUnlocked;
        public Action<UpgradeItem> OnLocked;


        private Hictionary<UpgradeItem> Items = new Hictionary<UpgradeItem>(5);
        protected IUpgradeStorage storage { get; private set; }
        public int Count => Items.Count;



        // Unity Methods
        protected virtual void Awake()
        {
            // Create Clone of all UpgradeAssets
            foreach (var conf in Configs)
            {
                conf.OnAwake(this);
                UpgradeItem previousasset = null;
                foreach (var Uasset in conf.line)
                {
                    UpgradeItem NewAsset = Instantiate(Uasset);
                    IUpgradeable IController = NewAsset;
                    if (IController.Initiate(this, conf, previousasset))
                    {
                        Items.Add(Uasset.Title, NewAsset);
                        previousasset = NewAsset;
                    }
                }
            }

            // Create SaveFile
            storage = GetStorage();
            if (storage != null)
            {
                // Load or Create
                if (!IsStorageLoaded())
                {
                    storage.CreateNewItems(Items.GetValues());
                }
            }
            else
            {
                Debug.LogWarning("Storage is Not Valid");
                enabled = false;
                return;
            }
        }

        // Publci Functions
        public bool IsUnlocked(string Title)
        {
            return storage.IsUnAvalibal(FindAsset(Title));
        }
        public bool CanUnlock(string Title)
        {
            return FindAsset(Title).CanUnlock();
        }
        public void Unlock(string Title)
        {
            if (CanUnlock(Title))
            {
                UnlockeAsset(FindAsset(Title), false);
            }
        }
        public void ForceUnlock(string Title)
        {
            UnlockeAsset(FindAsset(Title), true);
        }
        public void Lock(string Title)
        {
            if (IsUnlocked(Title))
            {
                LockAsset(FindAsset(Title));
            }
            else
            {
                Debug.LogWarning("This asset is already Locked");
            }
        }
        public UpgradeItem FindAsset(string Title)
        {
            return Items[Title];
        }
        public T[] CopyItemsByClass<T>() where T : UpgradeItem
        {
            List<T> Result = new List<T>();
            foreach (var item in Items.GetValues())
            {
                if (item is T finditem)
                {
                    Result.Add(finditem);
                }
            }
            return Result.ToArray();
        }
        public void Clear()
        {
            Items.Clear();
            storage.CreateNewItems(Items.GetValues());
        }

        // Private Functions
        private void UnlockeAsset(UpgradeItem item, bool free)
        {
            if (storage.SwapToUnAvalibal(item))
            {
                IUpgradeable IController = item;
                IController.SetUnlock(free);
                MessageBehavior(item, true);
            }
            else
            {
                Debug.LogError("There is issue please Remove UpgradeSavefile");
            }
        }
        private void LockAsset(UpgradeItem item)
        {
            if (storage.SwapToAvalibal(item))
            {
                IUpgradeable IController = item;
                IController.SetLock();
                MessageBehavior(item, false);
            }
            else
            {
                Debug.LogError("There is issue please Remove UpgradeSavefile");
            }
        }
        private void MessageBehavior(UpgradeItem item, bool isunlock)
        {
            if (Behavior == UpgradeBehavior.Action || Behavior == UpgradeBehavior.Both)
            {
                if (isunlock)
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
                if (isunlock)
                {
                    SendMessage("OnUnlocked", SendMessageOptions.RequireReceiver);
                }
                else
                {
                    SendMessage("OnLocked", SendMessageOptions.RequireReceiver);
                }
            }

            if (isunlock)
            {
                OnUnlockedAsset(item);
            }
            else
            {
                OnLockedAsset(item);
            }
        }


        // Abstract Methods
        protected abstract void OnUnlockedAsset(UpgradeItem item);
        protected abstract void OnLockedAsset(UpgradeItem item);
        protected abstract IUpgradeStorage GetStorage();
        protected abstract bool IsStorageLoaded();
    }

    public abstract class UpgradeStorage : Upgrade
    {
        [Header("Save")]
        [SerializeField]
        private bool UseCustomFile = false;
        [SerializeField, ConditionalHide("UseCustomFile", true, false)]
        private SaveFile _SaveFile;
        public SaveFile File => _SaveFile;

        // Upgrade Methods
        protected sealed override IUpgradeStorage GetStorage()
        {
            if (UseCustomFile)
            {
                if (_SaveFile is IUpgradeStorage newstorage)
                {
                    return newstorage;
                }
                else
                {
                    Debug.LogWarning("IUpgradeStorage Interface not implemented in CustomSavefile.");
                    UseCustomFile = false;
                }

            }

            _SaveFile = ScriptableObject.CreateInstance<UpgradeSaveFile>();
            _SaveFile.name = "RMUpgradeSaveFile";
            return _SaveFile as IUpgradeStorage;
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
                    return true;
                }
            }

            return false;
        }


    }

}