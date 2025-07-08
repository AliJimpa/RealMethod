using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealMethod
{
    public class UpgradeSaveFile : SaveFile
    {
        private List<string> Avalable;
        private List<string> UnAvalibal;


        // Base SaveFile Method
        protected override void OnStable(DataManager manager)
        {
        }
        protected override void OnSaved()
        {
            RM_PlayerPrefs.SetArray("availableUpgrades", Avalable.ToArray());
            RM_PlayerPrefs.SetArray("unlockedUpgrades", UnAvalibal.ToArray());
            RM_PlayerPrefs.SetBool("UpgradeFile", true);
        }
        protected override void OnLoaded()
        {
            Avalable = RM_PlayerPrefs.GetArray<string>("availableUpgrades").ToList();
            UnAvalibal = RM_PlayerPrefs.GetArray<string>("unlockedUpgrades").ToList();
        }
        protected override void OnDeleted()
        {
            PlayerPrefs.DeleteKey("UpgradeFile");
        }


        // Public Functions
        public void Initiate(Upgrade Owner, string[] StartList)
        {
            Avalable = StartList.ToList();
            UnAvalibal = new List<string>(Avalable.Count);
        }
        public bool SwapToUnAvalibal(string target)
        {
            foreach (var Uname in Avalable)
            {
                if (Uname == target)
                {
                    Avalable.Remove(target);
                    UnAvalibal.Add(target);
                    return true;
                }
            }
            return false;
        }
        public bool SwapToAvalibal(string target)
        {
            foreach (var Uname in UnAvalibal)
            {
                if (Uname == target)
                {
                    UnAvalibal.Remove(target);
                    Avalable.Add(target);
                    return true;
                }
            }
            return false;
        }
        public bool IsUnAvalibal(string target)
        {
            return UnAvalibal.Contains(target);
        }
        public bool IsAvalabel(string target)
        {
            return Avalable.Contains(target);
        }
    }
}