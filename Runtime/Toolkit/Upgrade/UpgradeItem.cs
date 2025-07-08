using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public abstract class UpgradeItem : ItemAsset, IUpgradeable
    {
        [Header("Upgrade")]
        [SerializeField, TextArea]
        private string description;
        [SerializeField]
        private List<UpgradeItem> dependency = new List<UpgradeItem>(1);

        // Actions
        public Action<UpgradeItem> OnUnlocked;

        protected Upgrade Owner { get; private set; }
        private bool IsUnlocked => Owner.IsUnlocked(Title);


        // Implement IUpgradeable Interface
        bool IUpgradeable.Initiate(Upgrade owner, UpgradeConfig config, UpgradeItem previous)
        {
            if (owner == null)
            {
                Debug.LogError($"UpgradeAsset {name} Can't Initiate Owner is NotValid");
                return false;
            }

            if (config.hasDependency)
            {
                if (previous != null)
                {
                    dependency.Add(previous);
                }
            }

            Instantiate(config);
            return true;
        }
        void IUpgradeable.SetUnlock(bool free)
        {
            if (free)
            {
                OnUnlock();
            }
            else
            {
                PayCost();
                OnUnlock();
            }
            OnUnlocked?.Invoke(this);
        }
        void IUpgradeable.SetLock()
        {
            OnReset();
        }

        // Public Functions
        public bool CheckDependency()
        {
            foreach (var Uassetitem in dependency)
            {
                if (!Uassetitem.IsUnlocked)
                {
                    return false;
                }
            }
            return true;
        }
        public bool CanUnlock()
        {
            return CheckDependency() && Prerequisites();
        }
        public bool Unlock()
        {
            if (CanUnlock())
            {
                Owner.Unlock(Title);
                return true;
            }
            else
            {
                return false;
            }
        }


        // Abstract Methods
        protected abstract void Instantiate(UpgradeConfig config);
        protected abstract bool Prerequisites();
        protected abstract void PayCost();
        protected abstract void OnUnlock();
        protected abstract void OnReset();
    }

}