using UnityEngine;

namespace RealMethod
{
    public sealed class DefaultGameConfig : GameConfig
    {
        public override void Initialized(Game Author)
        {
            Debug.Log("DefaultConfig Loaded");
        }

        protected override void OnEnable()
        {
            if (HasCloneName())
            {
                Debug.LogError($"UniqueAsset Can't Clone at Runtime, NewFile Removed!");
                Destroy(this);
                return;
            }
        }
    }
}
