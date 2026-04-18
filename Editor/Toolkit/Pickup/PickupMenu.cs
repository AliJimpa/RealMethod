using UnityEditor;

namespace RealMethod.Editor
{
    class PickupMenu
    {
        private const RealMethodLayer MenuLayer = RealMethodLayer.Toolkit;


        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/Pickup/Pickup3D", false, 80)]
        public static void CreatePickup3D()
        {
            RM_Editor.CreateScriptTemplate("Pickup3D", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/Pickup/Pickup2D", false, 80)]
        public static void CreatePickup2D()
        {
            RM_Editor.CreateScriptTemplate("Pickup2D", MenuLayer);
        }
    }
}