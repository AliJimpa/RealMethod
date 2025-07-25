using UnityEngine;

namespace RealMethod
{

    public abstract class UpgradeConfig : ConfigAsset
    {
        [Header("Assets")]
        [SerializeField]
        private UpgradeItem[] Line;
        public UpgradeItem[] line => Line;
        [Header("Setting")]
        [SerializeField]
        private bool chainDependency = false;
        public bool hasDependency => chainDependency;

        //Abstract Methods
        public abstract void OnAwake(Upgrade owner);
    }


    [CreateAssetMenu(fileName = "UpgradeConfig", menuName = "RealMethod/Upgrade/Config", order = 1)]
    public class LineUpgradeConfig : UpgradeConfig
    {
        public override void OnAwake(Upgrade owner)
        {
            
        }
    }


}