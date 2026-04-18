using UnityEditor;

namespace RealMethod.Editor
{
    class InventoryMenu
    {
        private const RealMethodLayer MenuLayer = RealMethodLayer.Toolkit;



        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/Inventory/Item", false, RM_Editor.MenuOrder.Toolkit)]
        public static void CreateItem()
        {
            RM_Editor.CreateScriptTemplate("InventoryItem", MenuLayer);
        }
    }
}