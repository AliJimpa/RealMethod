using UnityEngine;

namespace RealMethod
{
    public sealed class DefaultGameConfig : GameConfig
    {
        private void OnEnable()
        {
            if (HasCloneName())
            {
                Debug.LogError($"UniqueAsset Can't Clone at Runtime, NewFile Removed!");
                Destroy(this);
                return;
            }
            Debug.Log("DefaultConfig Loaded");
        }
    }
}
