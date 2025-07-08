using UnityEngine;

namespace RealMethod
{
    public abstract class UpgradeConfig : ConfigAsset
    {
        [Header("Assets")]
        [SerializeField]
        private UpgradeAsset[] Line;
        public UpgradeAsset[] line => Line;
        [Header("Setting")]
        [SerializeField]
        private bool chainDependency = false;
        public bool hasDependency => chainDependency;
    }
}