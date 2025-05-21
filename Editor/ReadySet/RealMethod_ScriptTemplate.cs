using UnityEditor;
using UnityEngine;

namespace RealMethod
{
    class RealMethodScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Core/Game", false, 80)]
        public static void Holay()
        {
            string Path = ScriptCreator.Create("GameTemplate.txt", "MyGame.cs", true);
        }
    }
}