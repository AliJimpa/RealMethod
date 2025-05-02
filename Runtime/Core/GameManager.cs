using UnityEngine;

namespace RealMethod
{
    public class GameManager : MonoBehaviour
    {
        private static RealMethodSettingAsset cachedSettings;

        public static RealMethodSettingAsset Settings
        {
            get
            {
                if (cachedSettings == null)
                {
                    cachedSettings = Resources.Load<RealMethodSettingAsset>("Mustard/GameSettingObj");
                    if (cachedSettings == null)
                    {
                        Debug.LogError("GlobalSettings asset is missing from Resources folder!");
                    }
                }
                return cachedSettings;
            }
        }
    }
}


