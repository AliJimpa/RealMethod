using UnityEditor;

namespace RealMethod.Editor
{
    class Tutorial_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Tutorial/Screen", false, 80)]
        public static void CreateTutorialScreen()
        {
            string Path = RM_Create.Script("TutorialScreenTemplate.txt", "MyScreenTutorial.cs");
        }

        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Tutorial/UI", false, 80)]
        public static void CreateTutorialMessage()
        {
            string Path = RM_Create.Script("TutorialUITemplate.txt.txt", "MyUITutorial.cs");
        }

        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Tutorial/Config", false, 80)]
        public static void CreateTutorialConfig()
        {
            string Path = RM_Create.Script("TutorialConfigTemplate.txt.txt", "MyTutorialConfig.cs");
        }
    }
}