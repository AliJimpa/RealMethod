using UnityEditor;

namespace RealMethod.Editor
{
    class Tutorial_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Tutorial/Message", false, 80)]
        public static void CreateTutorialMessage()
        {
            string Path = RM_Create.Script("TutorialMessageTemplate.txt.txt", "MyTutorialMessage.cs");
        }

        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Tutorial/Config", false, 80)]
        public static void CreateTutorialConfig()
        {
            string Path = RM_Create.Script("TutorialConfigTemplate.txt.txt", "MyTutorialConfig.cs");
        }
    }
}