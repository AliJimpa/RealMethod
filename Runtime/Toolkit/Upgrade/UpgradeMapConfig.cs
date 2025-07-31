using UnityEngine;

namespace RealMethod
{
    public abstract class UpgradeMapConfig : ConfigAsset, IUpgradeConfig
    {
        [Header("Identity")]
        [SerializeField]
        private string label;
        public string Label => label;
        [SerializeField]
        private bool AvalableOnBegin = true;
        public IUpgradeConfig provider => this;

        IUpgradeItem[] IUpgradeConfig.GenerateItems(Upgrade owner)
        {
            Initiated(owner);
            int itemcount = GetItemCount();
            if (itemcount > 0)
            {
                IUpgradeItem lastItem = null;
                IUpgradeItem[] result = new IUpgradeItem[itemcount];
                for (int i = 0; i < itemcount; i++)
                {
                    result[i] = GenerateItem(i, lastItem);
                    lastItem = result[i];
                    if (result[i] == null)
                    {
                        Debug.LogWarning($"Item with Index: {i} is not valid");
                    }
                }
                return result;
            }
            else
            {
                Debug.LogError("ItemCount should be more than zero");
                return null;
            }
        }
        IUpgradeItem IUpgradeConfig.GetStartItem()
        {
            if (AvalableOnBegin)
            {
                return GetBeginItem();
            }
            else
            {
                return null;
            }
        }

        protected abstract void Initiated(Upgrade owner);
        protected abstract IUpgradeItem GenerateItem(int index, IUpgradeItem previousItem);
        protected abstract IUpgradeItem GetBeginItem();
        protected abstract int GetItemCount();
    }
}