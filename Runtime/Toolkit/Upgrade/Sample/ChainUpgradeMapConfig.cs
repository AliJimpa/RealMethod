using UnityEngine;

namespace RealMethod
{
    public interface IPayment
    {
        int GetCapital();
        void Disbursement(int amount);
    }

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
            Debug.Log($"{itemName} Applyed");
        }
        protected override void PayCost()
        {
            MyOwner.Pay(Price);
        }
        protected override bool CheckDependency(IUpgradeItem previousItem)
        {
            return previousItem.IsUnlocked;
        }
        protected override bool CheckPayment()
        {
            return MyOwner.GetCoin() >= Price;
        }
        protected override void Deny()
        {
            Debug.Log($"{itemName} Denied");
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
        private int Count;
        [SerializeField]
        private SaveFile CoinFile;
        [Header("Status")]
        [SerializeField, ReadOnly]
        private ChainUpgradeAsset[] items;
        private IPayment payment
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
            items = new ChainUpgradeAsset[Count];
        }
        protected override IUpgradeItem GenerateItem(int index, IUpgradeItem previousItem)
        {
            ChainUpgradeAsset MyItem = CreateInstance<ChainUpgradeAsset>();
            MyItem.SetPrice(10);
            if (previousItem != null)
            {
                previousItem.SetNextAvailables(new IUpgradeItem[1] { MyItem.provider });
            }
            MyItem.provider.PreviousItem(previousItem);
            items[index] = MyItem;
            return MyItem.provider;
        }
        protected override IUpgradeItem GetBeginItem()
        {
            return items[0];
        }
        protected override int GetItemCount()
        {
            return Count;
        }


        public int GetCoin()
        {
            return payment != null ? payment.GetCapital() : 0;
        }
        public void Pay(int fee)
        {
            if (payment != null)
            {
                payment.Disbursement(fee);
            }
        }

    }

}