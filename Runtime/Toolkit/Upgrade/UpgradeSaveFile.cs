using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RealMethod
{
    [CreateAssetMenu(fileName = "UpgradeSaveFile", menuName = "RealMethod/Upgrade/SaveFile", order = 1)]
    public class UpgradeSaveFile : SaveAsset, IUpgradeStorage
    {
        [Header("Setting")]
        public bool UsePlayerPrefs = true;
        [Header("Storage")]
        public List<string> UnlockItems;
        public List<string> AvailableItems;

        // Base SaveFile Method
        protected override void OnSaved()
        {
            if (UsePlayerPrefs)
            {
                RM_Save.SetArray("UnlockItems", UnlockItems.ToArray());
                RM_Save.SetArray("AvailableItems", AvailableItems.ToArray());
            }
        }
        protected override void OnLoaded()
        {
            if (UsePlayerPrefs)
            {
                UnlockItems = RM_Save.GetArray<string>("UnlockItems").ToList();
                AvailableItems = RM_Save.GetArray<string>("AvailableItems").ToList();
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


        protected override void Reset()
        {
            base.Reset();
            ((IStorage)this).StorageClear();
        }


#if UNITY_EDITOR
        public override bool AutoReset(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
                return true;
            return base.AutoReset(state);
        }
#endif

    }



}