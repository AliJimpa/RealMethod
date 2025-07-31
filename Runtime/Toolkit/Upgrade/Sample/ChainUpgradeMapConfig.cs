using UnityEngine;

namespace RealMethod
{
    public class ChainUpgradeAsset : UpgradeAsset
    {
        private int Price;
        private ChainUpgradeMapConfig MyOwner;

        // UpgradeAsset Methods
        protected override void Initiate()
        {
            if (Owner is ChainUpgradeMapConfig Chainconfig)
            {
                MyOwner = Chainconfig;
            }
            else
            {
                Debug.LogError($"{this} Upgrade Asset should Initiate by {typeof(ChainUpgradeMapConfig)}");
            }
        }
        protected override void Loaded()
        {

        }
        protected override void Apply()
        {
            MyOwner.UpdateIem(this, true);
        }
        protected override void PayCost()
        {
            MyOwner.payment.Disbursement(Price);
        }
        protected override bool CheckDependency(IUpgradeItem[] items)
        {
            return previousItem != null ? previousItem.IsUnlocked : true;
        }
        protected override bool CheckPayment()
        {
            return MyOwner.payment.GetCapital() >= Price;
        }
        protected override void Deny()
        {
            MyOwner.UpdateIem(this, false);
        }


        public void SetPrice(int price)
        {
            Price = price;
        }
    }

    [CreateAssetMenu(fileName = "ChainUpgradeMap", menuName = "RealMethod/Upgrade/ChainMap", order = 1)]
    public class ChainUpgradeMapConfig : UpgradeMapConfig
    {
        [Header("Setting")]
        [SerializeField]
        private SaveFile CoinFile;
        [SerializeField]
        private int[] pricing;
        [Header("Status")]
        [SerializeField, ReadOnly]
        private ChainUpgradeAsset[] items;
        public IPayment payment
        {
            get
            {
                if (CoinFile is IPayment provider)
                {
                    return provider;
                }
                else
                {
                    Debug.LogWarning("Your CoinFile Need Implement IPayment");
                    return null;
                }
            }
        }

        [SerializeField]
        private System.Action<bool> OnUpgrade;

        // UpgradeMapConfig Methods
        protected override void Initiated(Upgrade owner)
        {
            if (pricing != null)
            {
                items = new ChainUpgradeAsset[pricing.Length];
            }
            else
            {
                Debug.LogError("Initiate faild");
            }
        }
        protected override IUpgradeItem GenerateItem(int index, IUpgradeItem previousItem)
        {
            items[index] = CreateInstance<ChainUpgradeAsset>();
            items[index].SetPrice(pricing[index]);
            if (previousItem != null)
            {
                previousItem.AddNextAvailables(items[index].provider);
            }
            items[index].provider.OnPreviousItem(previousItem);
            return items[index].provider;
        }
        protected override IUpgradeItem GetBeginItem()
        {
            return items[0];
        }
        protected override int GetItemCount()
        {
            return pricing.Length;
        }


        // Public Method
        public void BindUpgrade(System.Action<bool> callback)
        {
            OnUpgrade += callback;
        }
        public void UpdateIem(IUpgradeItem item, bool status)
        {
            OnUpgrade?.Invoke(status);
        }
    }

}