using UnityEditor;

namespace RealMethod.Editor
{
    class Ability_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Ability/Ability", false, 80)]
        public static void CreateAbilityCommand()
        {
            string Path = RM_Create.Script("AbilityComponentTemplater.txt", "MyAbility.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Ability/Power", false, 80)]
        public static void CreatePowerCommand()
        {
            string Path = RM_Create.Script("PowerCommandTemplate.txt", "MyPower.cs");
        }
    }
}