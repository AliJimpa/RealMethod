using UnityEditor;

namespace RealMethod.Editor
{
    class Actor_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Actor/Act", false, 80)]
        public static void CreateActCommand()
        {
            string Path = ScriptCreator.Create("ActTemplate.txt", "MyAct.cs");
        }
    }
}