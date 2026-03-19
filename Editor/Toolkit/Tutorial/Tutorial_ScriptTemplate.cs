using UnityEditor;

namespace RealMethod.Editor
{
    class Tutorial_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Tutorial/ScreenWidget", false, 80)]
        public static void CreateTutorialScreen()
        {
            string Path = RealMethod.CreateScriptTemplate("TutorialScreenTemplate.txt", "MyTutorialScreen.cs");
        }

        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Tutorial/UIunit", false, 80)]
        public static void CreateTutorialMessage()
        {
            string Path = RealMethod.CreateScriptTemplate("TutorialUnitTemplate.txt", "MyTutorialUnit.cs");
        }

        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Tutorial/Config", false, 80)]
        public static void CreateTutorialConfig()
        {
            string Path = RealMethod.CreateScriptTemplate("TutorialConfigTemplate.txt", "MyTutorialConfig.cs");
        }
    }
}