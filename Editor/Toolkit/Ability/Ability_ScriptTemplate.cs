using UnityEditor;

namespace RealMethod.Editor
{
    class Ability_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Ability/AbilityCommand", false, 80)]
        public static void CreateAbilityCommand()
        {
            string Path = ScriptCreator.Create("AbilityCommandTemplate.txt", "MyAbilityCommand.cs");
        }
    }
}