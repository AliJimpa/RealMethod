using UnityEditor;

namespace RealMethod.Editor
{
    class UpgradeMenu
    {
        private const RealMethodLayer MenuLayer = RealMethodLayer.Toolkit;



        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/Upgrade/Item", false, 80)]
        public static void CreateItem()
        {
            RM_Editor.CreateScriptTemplate("UpgradeItem", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/Upgrade/Map", false, 80)]
        public static void CreateConfig()
        {
            RM_Editor.CreateScriptTemplate("UpgradeConfig", MenuLayer);
        }
    }
}