using UnityEditor;

namespace RealMethod.Editor
{
    class PCG_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/PCG/Request", false, 80)]
        public static void CreatePCGRequest()
        {
            string Path = RealMethod.CreateScriptTemplate("PCGRequestTamplate.txt", "MyRequest.cs");
        }
    }
}