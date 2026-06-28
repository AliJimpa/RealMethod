using UnityEditor;

namespace RealMethod.Editor
{
    class PCGMenu
    {
        private const RealMethodLayer MenuLayer = RealMethodLayer.Toolkit;

        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/PCG/Request", false, RM_Editor.MenuOrder.Toolkit)]
        public static void CreatePCGRequest()
        {
            RM_Editor.CreateScriptTemplate("PCGRequest", MenuLayer);
        }
    }
}