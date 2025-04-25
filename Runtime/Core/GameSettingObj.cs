using UnityEngine;

public class GameSettingObj : ScriptableObject
{
    [Header("GlobalGameSetting")]
    public GameData GeneralGameData;
    [SerializeField]
    private string GameClass; // Stores class name as string


    public System.Type GetComponentType()
    {
        return System.Type.GetType(GameClass);
    }

    public void SetComponentType(System.Type type)
    {
        GameClass = type.AssemblyQualifiedName; // Store fully qualified name
    }
}
