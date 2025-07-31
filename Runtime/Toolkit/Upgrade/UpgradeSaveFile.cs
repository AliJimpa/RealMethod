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
        public void StorageCreated(Object author)
        {
            if (author is Upgrade upgrator)
            {
                for (int i = 0; i < upgrator.AvailableCount; i++)
                {
                    AvailableItems.Add(upgrator.GetAvailableItem(i).Label);
                }
            }
            else
            {
                Debug.LogWarning($"{this} Storage Should create by Upgrade Class");
            }
        }
        public void StorageLoaded(Object author)
        {
        }
        public void UnlockItem(IUpgradeItem item)
        {
            UnlockItems.Add(item.Label);
        }
        public void LockItem(IUpgradeItem item)
        {
            UnlockItems.Remove(item.Label);
        }
        public void AddAvailableItem(IUpgradeItem item)
        {
            AvailableItems.Add(item.Label);
        }
        public void RemoveAvalibelItem(IUpgradeItem item)
        {
            AvailableItems.Remove(item.Label);
        }
        public string[] GetAvailableItems()
        {
            return AvailableItems.ToArray();
        }
        public string[] GetUnlockItems()
        {
            return UnlockItems.ToArray();
        }
        public void StorageClear()
        {
            if (UnlockItems != null)
                UnlockItems.Clear();
            if (UnlockItems != null)
                AvailableItems.Clear();
        }

    }



}