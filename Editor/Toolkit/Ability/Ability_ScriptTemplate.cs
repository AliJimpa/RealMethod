using UnityEditor;

namespace RealMethod.Editor
{
    class Ability_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Ability/Power", false, 80)]
        public static void CreatePowerCommand()
        {
            string Path = ScriptCreator.Create("PowerCommandTemplate.txt", "MyPower.cs");
        }
    }
}