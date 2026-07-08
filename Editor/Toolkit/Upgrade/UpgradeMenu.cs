using UnityEditor;

namespace RealMethod.Editor
{
    class UpgradeMenu
    {
        private const RealMethodLayer MenuLayer = RealMethodLayer.Toolkit;



        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/Upgrade/Item", false, RM_Editor.MenuOrder.Toolkit)]
        public static void CreateItem()
        {
            RM_Editor.CreateScriptTemplate("UpgradeItem", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/Upgrade/Map", false, RM_Editor.MenuOrder.Toolkit)]
        public static void CreateConfig()
        {
            RM_Editor.CreateScriptTemplate("UpgradeConfig", MenuLayer);
        }
    }
}