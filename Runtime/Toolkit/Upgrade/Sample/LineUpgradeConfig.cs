using UnityEngine;

namespace RealMethod
{
    
    [CreateAssetMenu(fileName = "ChainUpgradeMap", menuName = "RealMethod/Upgrade/ChainMap", order = 1)]
    public class ChainUpgradeMapConfig : UpgradeMapConfig
    {
        // [Header("Assets")]
        // [SerializeField]
        // private UpgradeItem[] Line;
        // public UpgradeItem[] line => Line;
        // [Header("Setting")]
        // [SerializeField]
        // private bool chainDependency = false;
        // public bool hasDependency => chainDependency;
        public class SimpleUpgradeItem : IUpgradeItem
        {
            private string MyName;
            private UpgradeMapConfig MyConfig;
            private bool MyStatus;
            private IUpgradeItem MyChainItem;
            private int Price;



            // Implement IUpgradeItem Interface
            public string Label => MyName;
            public bool IsUnlocked => MyStatus;
            public string ConfigLabel => MyConfig.Label;
            void IUpgradeItem.Identify(UpgradeMapConfig map, int index)
            {
                MyConfig = map;
                MyName = $"{map.Label}_{index}";
            }
            public void Sync(bool status)
            {
                MyStatus = status;
            }
            public bool Prerequisites(bool cost)
            {
                bool NoChain = MyChainItem != null ? MyChainItem.IsUnlocked : true;
                bool ValidateConst = cost ? CheckPayment() : true;
                return NoChain & ValidateConst;
            }
            public void Unlock()
            {
                
            }
            public void Lock()
            {

            }
            public IUpgradeItem[] GetNextAvailables()
            {
                return null;
            }


            // Public Functions
            public bool CheckPayment()
            {
                // price
                return true;
            }
            public void SetDependencyItem(IUpgradeItem item)
            {
                MyChainItem = item;
            }

        }



        protected override IUpgradeItem GenerateItem(int index, IUpgradeItem previousItem)
        {
            throw new System.NotImplementedException();
        }
        protected override IUpgradeItem GetBeginItem()
        {
            throw new System.NotImplementedException();
        }
        protected override int GetItemCount()
        {
            throw new System.NotImplementedException();
        }
        protected override void Initiated(Upgrade owner)
        {
            throw new System.NotImplementedException();
        }
    }

}