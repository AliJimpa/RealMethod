using System.Collections.Generic;
using System.Linq;

namespace RealMethod
{
    public abstract class UpgradeSaveFile : SaveFile, IUpgradeStorage
    {
        protected List<string> Avalable;
        protected List<string> UnAvalibal;

        // Implement IUpgradeStorage Interface
        void IUpgradeStorage.Initiate(Upgrade owner, UpgradeAsset[] list)
        {
            string[] UNames = list.Select(upgrade => upgrade.Title).ToArray();
            Avalable = UNames.ToList();
            UnAvalibal = new List<string>(Avalable.Count);
        }
        bool IUpgradeStorage.SwapToUnAvalibal(UpgradeAsset target)
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
        bool IUpgradeStorage.SwapToAvalibal(UpgradeAsset target)
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
        bool IUpgradeStorage.IsUnAvalibal(UpgradeAsset target)
        {
            return UnAvalibal.Contains(target.Title);
        }
        bool IUpgradeStorage.IsAvalabel(UpgradeAsset target)
        {
            return Avalable.Contains(target.Title);
        }

    }



}