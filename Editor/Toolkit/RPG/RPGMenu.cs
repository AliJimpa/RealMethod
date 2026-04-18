using UnityEditor;

namespace RealMethod.Editor
{
    class RPGMenu
    {
        private const RealMethodLayer MenuLayer = RealMethodLayer.Toolkit;



        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/RPG/StatDefinition", false, 80)]
        public static void CreateStatDefinition()
        {
            RM_Editor.CreateScriptTemplate("StatDefinition", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/RPG/StatProfile", false, 80)]
        public static void CreateStatProfile()
        {
            RM_Editor.CreateScriptTemplate("StatProfile", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/RPG/StatBuff", false, 80)]
        public static void CreateBuffConfig()
        {
            RM_Editor.CreateScriptTemplate("BuffConfig", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/RPG/ResourceData", false, 80)]
        public static void CreateResourceData()
        {
            RM_Editor.CreateScriptTemplate("ResourceData", MenuLayer);
        }

    }
}