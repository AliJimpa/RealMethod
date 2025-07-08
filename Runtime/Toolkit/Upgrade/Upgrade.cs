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


        public Action<UpgradeAsset> OnUnlocked;
        public Action<UpgradeAsset> OnLocked;


        private Hictionary<UpgradeAsset> UpgradesAsset = new Hictionary<UpgradeAsset>(5);
        public int Count => UpgradesAsset.Count;
        private IUpgradeStorage Sotrage;
        private DataManager SaveManager;


        // Unity Methods
        protected virtual void Awake()
        {
            // Find Data Maanger 
            SaveManager = Game.FindManager<DataManager>();
            if (SaveManager == null)
            {
                Debug.LogError($"Can't find manager [{typeof(DataManager)}] ! ");
                enabled = false;
                return;
            }

            // Create Clone of all UpgradeAssets
            foreach (var conf in Configs)
            {
                conf.OnAwake(this);
                UpgradeAsset previousasset = null;
                foreach (var Uasset in conf.line)
                {
                    UpgradeAsset NewAsset = Instantiate(Uasset);
                    IUpgradeable IController = NewAsset;
                    if (IController.Initiate(this, conf, previousasset))
                    {
                        UpgradesAsset.Add(Uasset.Title, NewAsset);
                        previousasset = NewAsset;
                    }
                }
            }

            // Create SaveFile
            SaveFile File = GetSaveFile(SaveManager);
            if (File == null)
            {
                Debug.LogWarning("SaveFile is Not Valid");
                enabled = false;
                return;
            }

            if (File is IUpgradeStorage IStore)
            {
                Sotrage = IStore;
                // Load File
                if (SaveManager.IsExistFile(File))
                {
                    SaveManager.LoadFile(File);
                }
                else
                {
                    Sotrage.Initiate(this, UpgradesAsset.GetValues());
                }
            }
            else
            {
                Debug.LogError("Save file Should Implement IUpgradeStorage, You can Use UpgradeSaveFile Class");
                enabled = false;
                return;
            }

        }

        // Publci Functions
        public bool IsUnlocked(string Title)
        {
            return Sotrage.IsUnAvalibal(FindAsset(Title));
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
        public UpgradeAsset FindAsset(string Title)
        {
            return UpgradesAsset[Title];
        }


        // Private Functions
        private void UnlockeAsset(UpgradeAsset asset, bool free)
        {
            if (Sotrage.SwapToUnAvalibal(asset))
            {
                IUpgradeable IController = asset;
                IController.SetUnlock(free);
                MessageBehavior(asset, true);
            }
            else
            {
                Debug.LogError("There is issue please Remove UpgradeSavefile");
            }
        }
        private void LockAsset(UpgradeAsset asset)
        {
            if (Sotrage.SwapToAvalibal(asset))
            {
                IUpgradeable IController = asset;
                IController.SetLock();
                MessageBehavior(asset, false);
            }
            else
            {
                Debug.LogError("There is issue please Remove UpgradeSavefile");
            }
        }
        private void MessageBehavior(UpgradeAsset Asset, bool isunlock)
        {
            if (Behavior == UpgradeBehavior.Action || Behavior == UpgradeBehavior.Both)
            {
                if (isunlock)
                {
                    OnUnlocked?.Invoke(Asset);
                }
                else
                {
                    OnLocked?.Invoke(Asset);
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
                OnUnlockedAsset(Asset);
            }
            else
            {
                OnLockedAsset(Asset);
            }

        }


        // Abstract Methods
        protected abstract void OnUnlockedAsset(UpgradeAsset asset);
        protected abstract void OnLockedAsset(UpgradeAsset asset);
        protected abstract SaveFile GetSaveFile(DataManager savesystem);

    }

}