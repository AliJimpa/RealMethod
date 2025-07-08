using System;
using System.Data.SqlTypes;
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
        public int Count => Items.Count;
        private IUpgradeStorage sotrage;


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
            sotrage = GetStorage();
            if (sotrage != null)
            {
                // Load or Create
                if (!IsStorageLoaded())
                {
                    sotrage.CreateNewItems(Items.GetValues());
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
            return sotrage.IsUnAvalibal(FindAsset(Title));
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


        // Private Functions
        private void UnlockeAsset(UpgradeItem item, bool free)
        {
            if (sotrage.SwapToUnAvalibal(item))
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
            if (sotrage.SwapToAvalibal(item))
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
        private bool AutoSave = true;
        [SerializeField]
        private bool UseCustomFile = false;
        [SerializeField, ConditionalHide("UseCustomFile", true, false)]
        private SaveFile _SaveFile;
        public SaveFile File => _SaveFile;

        private DataManager savesystem;

        // Upgrade Methods
        protected sealed override void OnUnlockedAsset(UpgradeItem item)
        {
            if (AutoSave)
                savesystem.SaveFile(_SaveFile);

            OnAssetUpdate(item, true);
        }
        protected sealed override void OnLockedAsset(UpgradeItem item)
        {
            if (AutoSave)
                savesystem.SaveFile(_SaveFile);

            OnAssetUpdate(item, false);
        }
        protected sealed override bool IsStorageLoaded()
        {
            // Find Data Maanger 
            savesystem = Game.FindManager<DataManager>();
            if (savesystem == null)
            {
                Debug.LogError($"Can't find manager [{typeof(DataManager)}] ! ");
                enabled = false;
                return false;
            }

            if (savesystem.IsExistFile(_SaveFile))
            {
                savesystem.LoadFile(_SaveFile);
                return true;
            }

            return false;

        }
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
                    Debug.LogError("IUpgradeStorage Interface not implemented in CustomSavefile.");
                    return null;
                }

            }

            _SaveFile = ScriptableObject.CreateInstance<UpgradeSaveFile>();
            _SaveFile.name = "RMUpgradeSaveFile";
            return _SaveFile as IUpgradeStorage;
        }


        // Abstract Methods
        protected abstract void OnAssetUpdate(UpgradeItem item, bool unlocked);

    }

}