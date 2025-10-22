using UnityEngine;

namespace RealMethod
{
    public class ChainUpgradeAsset : UpgradeAsset
    {
        private int price;
        public int Price => price;

        private ChainUpgradeMapConfig MyOwner;
        private IChainUpgrade Chain => MyOwner;

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
            Chain.UpdateItem(this, true);
        }
        protected override void PayCost()
        {
            MyOwner.payment.Disbursement(price);
        }
        protected override bool CheckDependency(IUpgradeItem[] items)
        {
            return previousItem != null ? previousItem.IsUnlocked : true;
        }
        protected override bool CheckPayment()
        {
            return MyOwner.payment.GetCapital() >= price;
        }
        protected override void Deny()
        {
            Chain.UpdateItem(this, false);
        }


        public void SetPrice(int camount)
        {
            price = camount;
        }
    }


    public interface IChainUpgrade
    {
        delegate void UpgradeHandler(bool status);
        // Only declare the event — no implementation inside the interface.
        void BindUpgrade(UpgradeHandler callback);
        // This must call a protected method in the implementing class
        // because interfaces cannot raise events.
        void UpdateItem(IUpgradeItem item, bool status);
    }

    [CreateAssetMenu(fileName = "ChainUpgradeMap", menuName = "RealMethod/Upgrade/ChainMap", order = 1)]
    public class ChainUpgradeMapConfig : UpgradeMapConfig, IChainUpgrade
    {
        [Header("Setting")]
        [SerializeField]
        private SaveFile CoinFile;
        [SerializeField]
        public int[] pricing;
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


        // Implement IChainUpgrade Interface
        [SerializeField]
        private  IChainUpgrade.UpgradeHandler OnUpgrade;
        void IChainUpgrade.BindUpgrade(IChainUpgrade.UpgradeHandler callback)
        {
            OnUpgrade += callback;
        }
        void IChainUpgrade.UpdateItem(IUpgradeItem item, bool status)
        {
            OnUpgrade?.Invoke(status);
        }


    }

}