using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{

    public class UpgradeData : ScriptableObject
    {
        public string upgradeName;
        public string description;
        public Sprite icon;
        public List<UpgradeData> prerequisites;
        public int cost;
        // any other stats you want
    }


    public class UpgradeManager : MonoBehaviour
    {
        public List<UpgradeData> availableUpgrades;
        public List<UpgradeData> unlockedUpgrades;

        public void Unlock(UpgradeData upgrade)
        {
            if (CanUnlock(upgrade))
            {
                unlockedUpgrades.Add(upgrade);
                ApplyUpgradeEffect(upgrade);
            }
        }

        private bool CanUnlock(UpgradeData upgrade)
        {
            foreach (var prereq in upgrade.prerequisites)
            {
                if (!unlockedUpgrades.Contains(prereq))
                    return false;
            }
            return true;
        }

        private void ApplyUpgradeEffect(UpgradeData upgrade)
        {
            // hook in your own effects here, or fire events to other systems
            Debug.Log($"Upgrade applied: {upgrade.upgradeName}");
        }
    }

}