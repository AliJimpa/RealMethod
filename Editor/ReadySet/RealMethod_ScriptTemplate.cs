using UnityEditor;

namespace RealMethod
{
    class RealMethodScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Core/Game", false, 80)]
        public static void CreateGameClass()
        {
            string Path = ScriptCreator.Create("GameTemplate.txt", "MyGame.cs", true);
        }
        // [MenuItem("Assets/Create/Scripting/RealMethod/Core/Game", false, 80)]
        // public static void CreateGameClass()
        // {
        //     string Path = ScriptCreator.Create("GameTemplate.txt", "MyGame.cs", true);
        // }
    }
}