using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "InventorySaveFile", menuName = "RealMethod/Inventory/SaveFile", order = 1)]
    public class UpgradeSaveFile : SaveFile, IUpgradeStorage
    {
        [Header("Setting")]
        [SerializeField]
        private bool UsePlayerPrefs = true;
        [Header("Storage")]
        [SerializeField]
        private List<string> Avalable;
        [SerializeField]
        private List<string> UnAvalibal;

        // Base SaveFile Method
        protected override void OnStable(DataManager manager)
        {
        }
        protected override void OnSaved()
        {
            if (UsePlayerPrefs)
            {
                RM_PlayerPrefs.SetArray("availableUpgrades", Avalable.ToArray());
                RM_PlayerPrefs.SetArray("unlockedUpgrades", UnAvalibal.ToArray());
                RM_PlayerPrefs.SetBool("UpgradeFile", true);
            }
        }
        protected override void OnLoaded()
        {
            if (UsePlayerPrefs)
            {
                Avalable = RM_PlayerPrefs.GetArray<string>("availableUpgrades").ToList();
                UnAvalibal = RM_PlayerPrefs.GetArray<string>("unlockedUpgrades").ToList();
            }
        }
        protected override void OnDeleted()
        {
            if (UsePlayerPrefs)
            {
                PlayerPrefs.DeleteKey("UpgradeFile");
                Avalable = null;
                UnAvalibal = null;
            }

        }


        // Implement IUpgradeStorage Interface
        void IUpgradeStorage.CreateNewItems(UpgradeItem[] list)
        {
            string[] UNames = list.Select(upgrade => upgrade.Title).ToArray();
            Avalable = UNames.ToList();
            UnAvalibal = new List<string>(Avalable.Count);
        }
        bool IUpgradeStorage.SwapToUnAvalibal(UpgradeItem target)
        {
            foreach (var Uname in Avalable)
            {
                if (Uname == target.Title)
                {
                    Avalable.Remove(target.Title);
                    UnAvalibal.Add(target.Title);
                    return true;
                }
            }
            return false;
        }
        bool IUpgradeStorage.SwapToAvalibal(UpgradeItem target)
        {
            foreach (var Uname in UnAvalibal)
            {
                if (Uname == target.Title)
                {
                    UnAvalibal.Remove(target.Title);
                    Avalable.Add(target.Title);
                    return true;
                }
            }
            return false;
        }
        bool IUpgradeStorage.IsUnAvalibal(UpgradeItem target)
        {
            return UnAvalibal.Contains(target.Title);
        }
        bool IUpgradeStorage.IsAvalabel(UpgradeItem target)
        {
            return Avalable.Contains(target.Title);
        }

    }



}