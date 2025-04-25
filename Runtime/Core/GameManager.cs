using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameSettingObj cachedSettings;

    public static GameSettingObj Settings
    {
        get
        {
            if (cachedSettings == null)
            {
                cachedSettings = Resources.Load<GameSettingObj>("Mustard/GameSettingObj");
                if (cachedSettings == null)
                {
                    Debug.LogError("GlobalSettings asset is missing from Resources folder!");
                }
            }
            return cachedSettings;
        }
    }
}


