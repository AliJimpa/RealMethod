using UnityEngine;

namespace RealMethod
{
    public abstract class UpgradeAsset : DataAsset, IUpgradeItem
    {
        [Header("UpgradeItem")]
        [SerializeField]
        protected string itemName;
        public string ItemName => itemName;

        protected UpgradeMapConfig Owner { get; private set; }
        public bool isUnlocked { get; private set; }
        protected IUpgradeItem[] NextAvailables { get; private set; }
        protected IUpgradeItem previousItem { get; private set; }
        public IUpgradeItem provider => this;

        // Implement IUpgradeItem Interface
        public string Label => itemName;
        public bool IsUnlocked => isUnlocked;
        public string ConfigLabel => Owner.Label;
        void IUpgradeItem.Identify(UpgradeMapConfig map, int index)
        {
            Owner = map;
            itemName = $"{map.Label}_{index}";
            Initiate();
        }
        void IUpgradeItem.Sync(bool status)
        {
            isUnlocked = status;
            Loaded();
        }
        bool IUpgradeItem.Prerequisites(bool cost)
        {
            bool ValidateConst = cost ? CheckPayment() : true;
            return CheckDependency(previousItem) & ValidateConst;
        }
        void IUpgradeItem.Unlock(bool cost)
        {
            if (cost)
            {
                PayCost();
                Apply();
            }
            else
            {
                Apply();
            }
        }
        void IUpgradeItem.Lock()
        {
            Deny();
        }
        void IUpgradeItem.SetNextAvailables(IUpgradeItem[] items)
        {
            NextAvailables = items;
        }
        IUpgradeItem[] IUpgradeItem.GetNextAvailables()
        {
            return NextAvailables;
        }
        void IUpgradeItem.PreviousItem(IUpgradeItem items)
        {
            previousItem = items;
        }

        // Abstract Methods
        protected abstract void Initiate();
        protected abstract void Loaded();
        protected abstract void Apply();
        protected abstract void PayCost();
        protected abstract void Deny();
        protected abstract bool CheckDependency(IUpgradeItem previousItem);
        protected abstract bool CheckPayment();


    }

}