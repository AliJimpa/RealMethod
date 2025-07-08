using System.Collections.Generic;
using System.Linq;

namespace RealMethod
{
    public abstract class UpgradeSaveFile : SaveFile, IUpgradeStorage
    {
        protected List<string> Avalable;
        protected List<string> UnAvalibal;

        // Implement IUpgradeStorage Interface
        void IUpgradeStorage.Initiate(Upgrade Owner, string[] StartList)
        {
            Avalable = StartList.ToList();
            UnAvalibal = new List<string>(Avalable.Count);
        }
        bool IUpgradeStorage.SwapToUnAvalibal(string target)
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
        bool IUpgradeStorage.SwapToAvalibal(string target)
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
        bool IUpgradeStorage.IsUnAvalibal(string target)
        {
            return UnAvalibal.Contains(target);
        }
        bool IUpgradeStorage.IsAvalabel(string target)
        {
            return Avalable.Contains(target);
        }

    }



}