using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "UpgradeSaveFile", menuName = "RealMethod/Upgrade/SaveFile", order = 1)]
    public class UpgradeSaveFile : SaveFile, IUpgradeStorage
    {
        [Header("Setting")]
        public bool UsePlayerPrefs = true;
        [Header("Storage")]
        public List<string> UnlockItems;
        public List<string> AvailableItems;

        // Base SaveFile Method
        protected override void OnStable(DataManager manager)
        {
        }
        protected override void OnSaved()
        {
            if (UsePlayerPrefs)
            {
                RM_PlayerPrefs.SetArray("UnlockItems", UnlockItems.ToArray());
                RM_PlayerPrefs.SetArray("AvailableItems", AvailableItems.ToArray());
            }
        }
        protected override void OnLoaded()
        {
            if (UsePlayerPrefs)
            {
                UnlockItems = RM_PlayerPrefs.GetArray<string>("UnlockItems").ToList();
                AvailableItems = RM_PlayerPrefs.GetArray<string>("AvailableItems").ToList();
            }
        }
        protected override void OnDeleted()
        {
            if (UsePlayerPrefs)
            {
                UnlockItems = null;
                AvailableItems = null;
            }

        }


        // Implement IUpgradeStorage Interface
        void IStorage.StorageCreated(Object author)
        {
            if (author is Upgrade upgrator)
            {
                UnlockItems = new List<string>(upgrator.ItemCount);
                AvailableItems = new List<string>();
            }
            else
            {
                Debug.LogWarning($"{this} Storage Should create by Upgrade Class");
            }
        }
        void IStorage.StorageLoaded(Object author)
        {
        }
        void IUpgradeStorage.UnlockItem(IUpgradeItem item)
        {
            UnlockItems.Add(item.Label);
        }
        void IUpgradeStorage.LockItem(IUpgradeItem item)
        {
            UnlockItems.Remove(item.Label);
        }
        void IUpgradeStorage.AddAvailableItem(IUpgradeItem item)
        {
            AvailableItems.Add(item.Label);
        }
        void IUpgradeStorage.RemoveAvalibelItem(IUpgradeItem item)
        {
            AvailableItems.Remove(item.Label);
        }
        string[] IUpgradeStorage.GetAvailableItems()
        {
            return AvailableItems.ToArray();
        }
        string[] IUpgradeStorage.GetUnlockItems()
        {
            return UnlockItems.ToArray();
        }
        void IStorage.StorageClear()
        {
            if (UnlockItems != null)
                UnlockItems.Clear();
            if (UnlockItems != null)
                AvailableItems.Clear();
        }


#if UNITY_EDITOR
        public override void OnEditorPlay()
        {
            //base.OnEditorPlay();
            ((IStorage)this).StorageClear();
        }
#endif

    }



}