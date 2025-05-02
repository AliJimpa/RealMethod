using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static RMSS cachedSettings;

    public static RMSS Settings
    {
        get
        {
            if (cachedSettings == null)
            {
                cachedSettings = Resources.Load<RMSS>("Mustard/GameSettingObj");
                if (cachedSettings == null)
                {
                    Debug.LogError("GlobalSettings asset is missing from Resources folder!");
                }
            }
            return cachedSettings;
        }
    }
}


