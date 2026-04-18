using UnityEditor;

namespace RealMethod.Editor
{
    class TutorialMenu
    {
        private const RealMethodLayer MenuLayer = RealMethodLayer.Toolkit;


        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/Tutorial/Screen", false, RM_Editor.MenuOrder.Toolkit)]
        public static void CreateTutorialScreen()
        {
            RM_Editor.CreateScriptTemplate("TutorialScreen", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/Tutorial/UIunit", false, RM_Editor.MenuOrder.Toolkit)]
        public static void CreateTutorialMessage()
        {
            RM_Editor.CreateScriptTemplate("TutorialUnit", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/Tutorial/Config", false, RM_Editor.MenuOrder.Toolkit)]
        public static void CreateTutorialConfig()
        {
            RM_Editor.CreateScriptTemplate("TutorialConfig", MenuLayer);
        }
    }
}