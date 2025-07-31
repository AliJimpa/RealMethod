using UnityEngine;

namespace RealMethod
{
    public abstract class UpgradeAsset : DataAsset, IUpgradeItem
    {
        [Header("UpgradeItem")]
        [SerializeField]
        private bool OverrideName = false;
        [SerializeField, ConditionalHide("OverrideName", true, false)]
        protected string itemName;
        public string ItemName => itemName;
        [SerializeField]
        private UpgradeAsset[] Dependency;

        protected UpgradeMapConfig Owner { get; private set; }
        public bool isUnlocked { get; private set; }
        public IUpgradeItem[] NextAvailables { get; private set; }
        public IUpgradeItem previousItem { get; private set; }
        public IUpgradeItem provider => this;

        // Implement IUpgradeItem Interface
        public string Label => itemName;
        public bool IsUnlocked => isUnlocked;
        public string ConfigLabel => Owner.Label;
        void IUpgradeItem.Identify(UpgradeMapConfig map, int mapIndex, int itemIndex, int various)
        {
            Owner = map;
            if (!OverrideName)
            {
                itemName = MakeName(map.Label, mapIndex, itemIndex, various);
            }
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
            IUpgradeItem[] alldependeditem;
            if (Dependency != null)
            {
                alldependeditem = new IUpgradeItem[Dependency.Length + 1];
                for (int i = 0; i < Dependency.Length; i++)
                {
                    alldependeditem[i] = Dependency[i];
                }
            }
            else
            {
                alldependeditem = new IUpgradeItem[1];
            }
            alldependeditem[alldependeditem.Length - 1] = previousItem;
            return CheckDependency(alldependeditem) & ValidateConst;
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
            isUnlocked = true;
        }
        void IUpgradeItem.Lock()
        {
            isUnlocked = false;
            Deny();
        }
        void IUpgradeItem.AddNextAvailables(IUpgradeItem items)
        {
            NextAvailables = new IUpgradeItem[1] { items };
        }
        IUpgradeItem[] IUpgradeItem.GetNextAvailables()
        {
            return NextAvailables;
        }
        void IUpgradeItem.OnPreviousItem(IUpgradeItem items)
        {
            previousItem = items;
        }
        T IUpgradeItem.GetClass<T>()
        {
            return this as T;
        }

        // Protected Fucntion
        protected virtual string MakeName(string mapName, int mapIndex, int itemIndex, int various)
        {
            return $"{mapName}_{itemIndex}";
        }


        // Abstract Methods
        protected abstract void Initiate();
        protected abstract void Loaded();
        protected abstract void Apply();
        protected abstract void PayCost();
        protected abstract void Deny();
        protected abstract bool CheckDependency(IUpgradeItem[] dependedItems);
        protected abstract bool CheckPayment();
    }

}