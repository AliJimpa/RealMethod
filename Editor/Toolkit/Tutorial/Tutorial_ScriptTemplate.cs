using UnityEditor;

namespace RealMethod.Editor
{
    class Tutorial_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Tutorial/ScreenWidget", false, 80)]
        public static void CreateTutorialScreen()
        {
            string Path = RM_Create.Script("TutorialScreenTemplate.txt", "MyTutorialScreen.cs");
        }

        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Tutorial/UIunit", false, 80)]
        public static void CreateTutorialMessage()
        {
            string Path = RM_Create.Script("TutorialUnitTemplate.txt", "MyTutorialUnit.cs");
        }

        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Tutorial/Config", false, 80)]
        public static void CreateTutorialConfig()
        {
            string Path = RM_Create.Script("TutorialConfigTemplate.txt", "MyTutorialConfig.cs");
        }
    }
}